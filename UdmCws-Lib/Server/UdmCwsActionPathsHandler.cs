using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;
using System;
using System.Text;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// HTTP handler for UDM-CWS /roomstatus endpoint
    /// Uses event-driven architecture - no state storage
    /// </summary>
    public class UdmCwsActionPathsHandler : WebRequestHandlerBase
    {
        /// <summary>
        /// Event fired when a GET request is received and state needs to be reported
        /// </summary>
        public event ReportStateEventHandler ReportStateEvent;

        /// <summary>
        /// Event fired when a PATCH request is received with desired state changes
        /// </summary>
        public event DesiredStateEventHandler DesiredStateEvent;

        /// <summary>
        /// Configuration for feedback mode behavior
        /// </summary>
        public UdmCwsConfig Config { get; set; }

        public UdmCwsActionPathsHandler(UdmCwsConfig config)
        {
            Config = config ?? new UdmCwsConfig();
        }

        public UdmCwsActionPathsHandler()
            : this(new UdmCwsConfig())
        {
        }

        /// <summary>
        /// Adds the /roomstatus route to the specified CWS server
        /// </summary>
        /// <param name="server">CWS server to add route to</param>
        /// <param name="routePrefix">Optional route prefix (e.g., "room1" creates "room1/roomstatus")</param>
        public void AddRoute(HttpCwsServer server, string routePrefix = "")
        {
            if (server == null)
                throw new ArgumentNullException("server");

            // Build route path with optional prefix
            var routePath = string.IsNullOrEmpty(routePrefix)
                ? "roomstatus"
                : string.Format("{0}/roomstatus", routePrefix.Trim('/'));

            var route = new HttpCwsRoute(routePath)
            {
                Name = "UdmCws_RoomStatus",
                RouteHandler = this
            };

            server.AddRoute(route);

            CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Route added to server at path: {0}", routePath);
        }

        /// <summary>
        /// Handles GET requests - builds state from events
        /// </summary>
        protected override void HandleGet(HttpCwsContext context)
        {
            try
            {
                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: GET request received from {0}", context.Request.UserHostAddress);

                // Validate headers (PSK and API version)
                if (!ValidateRequestHeaders(context))
                    return;

                // Create event args with empty state
                var eventArgs = new ReportStateEventArgs();

                // Fire event to allow subscribers to populate state
                ReportStateEvent?.Invoke(this, eventArgs);

                // Serialize state to JSON
                var jsonResponse = JsonConvert.SerializeObject(eventArgs.ReportedState, Formatting.Indented);

                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Sending response, {0} characters", jsonResponse.Length);

                // Return 200 OK with state
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                context.Response.Headers.Add("Content-Type", "application/json");
                context.Response.Write(jsonResponse, false);
                context.Response.End();
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Error handling GET request: {0}", ex.Message);
                SendErrorResponse(context, 500, "Internal server error");
            }
        }

        /// <summary>
        /// Handles PATCH requests - fires events for desired state changes
        /// Per API spec: standard.state and standard.activity are writable
        /// </summary>
        protected override void HandlePatch(HttpCwsContext context)
        {
            try
            {
                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: PATCH request received from {0}", context.Request.UserHostAddress);

                // Validate headers (PSK and API version)
                if (!ValidateRequestHeaders(context))
                    return;

                // Read request body
                var requestBody = ReadRequestBody(context);
                if (string.IsNullOrEmpty(requestBody))
                {
                    SendErrorResponse(context, 400, "Request body is empty");
                    return;
                }

                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: PATCH body: {0}", requestBody);

                // Parse and validate PATCH request
                var desiredState = PatchRequestParser.ParsePatchRequest(requestBody, out var errorMessage);
                if (desiredState == null)
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: PATCH validation failed: {0}", errorMessage);
                    SendErrorResponse(context, 400, errorMessage);
                    return;
                }

                // Fire event to notify subscribers of desired state change
                var eventArgs = new DesiredStateEventArgs(desiredState);
                DesiredStateEvent?.Invoke(this, eventArgs);

                // Check if event was handled successfully
                if (!eventArgs.Success)
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: PATCH handling failed: {0}", eventArgs.ErrorMessage);
                    SendErrorResponse(context, 500, eventArgs.ErrorMessage ?? "Failed to apply state change");
                    return;
                }

                // Respond based on feedback mode
                if (Config.FeedbackMode == FeedbackMode.Immediate)
                {
                    // Immediate mode: Return 200 OK with updated state
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Immediate mode - returning updated state");

                    // Fire ReportStateEvent to get current state
                    var reportEventArgs = new ReportStateEventArgs();
                    ReportStateEvent?.Invoke(this, reportEventArgs);

                    var jsonResponse = JsonConvert.SerializeObject(reportEventArgs.ReportedState, Formatting.Indented);

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.Add("Content-Type", "application/json");
                    context.Response.Write(jsonResponse, false);
                    context.Response.End();
                }
                else
                {
                    // Deferred mode: Return 202 Accepted
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Deferred mode - returning 202 Accepted");

                    context.Response.StatusCode = 202;
                    context.Response.StatusDescription = "Accepted";
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.Add("Content-Type", "application/json");
                    context.Response.Write("{\"message\":\"State change accepted\"}", false);
                    context.Response.End();
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Error handling PATCH request: {0}", ex.Message);
                SendErrorResponse(context, 500, "Internal server error");
            }
        }

        /// <summary>
        /// Reads the request body as a string
        /// </summary>
        private string ReadRequestBody(HttpCwsContext context)
        {
            try
            {
                if (context.Request.ContentLength > 0)
                {
                    var buffer = new byte[context.Request.ContentLength];
                    context.Request.InputStream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Error reading request body: {0}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Sends an error response
        /// </summary>
        private void SendErrorResponse(HttpCwsContext context, int statusCode, string errorMessage)
        {
            var errorResponse = new { error = errorMessage };
            var jsonResponse = JsonConvert.SerializeObject(errorResponse);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Content-Type", "application/json");
            context.Response.Write(jsonResponse, false);
            context.Response.End();
        }

        /// <summary>
        /// Validates request headers (PSK and API version)
        /// Returns true if validation passes, false if request should be rejected
        /// </summary>
        private bool ValidateRequestHeaders(HttpCwsContext context)
        {
            // Get headers
            var requestApiVersion = context.Request.Headers["PDT-API-VERSION"];
            var requestPsk = context.Request.Headers["PDT-PSK"];

            // Check API version (log warning only, don't reject)
            if (!string.IsNullOrEmpty(requestApiVersion))
            {
                if (string.IsNullOrEmpty(Config.ApiVersion))
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Warning - API version in request but not configured");
                    ErrorLog.Warn("UdmCwsActionPathsHandler: API version in request but not configured");
                }
                else if (requestApiVersion != Config.ApiVersion)
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Warning - API version mismatch - Expected: {0}, Received: {1}",
                        Config.ApiVersion, requestApiVersion);
                    ErrorLog.Warn("UdmCwsActionPathsHandler: API version mismatch - Expected: {0}, Received: {1}",
                        Config.ApiVersion, requestApiVersion);
                }
            }
            else
            {
                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Warning - No API version in request headers");
                ErrorLog.Warn("UdmCwsActionPathsHandler: No API version in request headers");
            }

            // Validate PSK (only if configured and not empty)
            var pskConfigured = !string.IsNullOrEmpty(Config.Psk);
            var pskInRequest = !string.IsNullOrEmpty(requestPsk);

            if (pskConfigured)
            {
                // PSK is configured, so we require it in the request
                if (!pskInRequest)
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Authentication failed");
                    ErrorLog.Error("UdmCwsActionPathsHandler: PSK validation failed - PSK required but not provided");
                    SendErrorResponse(context, 401, "Authentication failed");
                    return false;
                }

                if (requestPsk != Config.Psk)
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Authentication failed");
                    ErrorLog.Error("UdmCwsActionPathsHandler: PSK validation failed - Invalid PSK");
                    SendErrorResponse(context, 401, "Authentication failed");
                    return false;
                }

                CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Authentication succeeded");
            }
            else
            {
                // No PSK configured - accept request with or without PSK header
                if (pskInRequest)
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: Warning - PSK in request but not configured (treating as no security)");
                }
                else
                {
                    CrestronConsole.PrintLine("UdmCwsActionPathsHandler: No PSK validation (no PSK configured)");
                }
            }

            return true;
        }
    }
}
