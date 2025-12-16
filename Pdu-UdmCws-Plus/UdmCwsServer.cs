using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Standalone CWS server for SIMPL+ integration
    /// Serves room status at /cws/app01/roomStatus
    /// </summary>
    public class UdmCwsServer
    {
        private HttpCwsServer _api;
        private GetStateDelegate _getStateDelegate;
        private bool _isRunning;

        public UdmCwsServer()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Initialize and start the CWS server
        /// </summary>
        /// <param name="getStateDelegate">Delegate that returns the current state</param>
        public void Start(GetStateDelegate getStateDelegate)
        {
            if (_isRunning)
            {
                CrestronConsole.PrintLine("UdmCwsServer: Server already running");
                return;
            }

            try
            {
                _getStateDelegate = getStateDelegate;

                // Get the current app number dynamically
                var appNumber = InitialParametersClass.ApplicationNumber;
                var serverPrefix = string.Format("/app{0:D2}/api", appNumber);
                var appRoute = "roomstatus";

                CrestronConsole.PrintLine("UdmCwsServer: App number detected: {0}", appNumber);
                CrestronConsole.PrintLine("UdmCwsServer: Creating server with prefix '{0}'", serverPrefix);

                // Create the server instance
                _api = new HttpCwsServer(serverPrefix);
                CrestronConsole.PrintLine("UdmCwsServer: Server instance created");

                // Add event handler for received requests
                CrestronConsole.PrintLine("UdmCwsServer: Adding ReceivedRequestEvent handler");
                _api.ReceivedRequestEvent += DefaultRequestHandler;

                // Create and register the route for app[XX]/roomStatus
                CrestronConsole.PrintLine("UdmCwsServer: Creating route '{0}'", appRoute);
                var route = new HttpCwsRoute(appRoute)
                {
                    Name = "UdmCWSRoomStatus",
                    RouteHandler = new UdmCwsActionPathsHandler(_getStateDelegate)
                };
                CrestronConsole.PrintLine("UdmCwsServer: Route object created");

                CrestronConsole.PrintLine("UdmCwsServer: Adding route using AddRoute() method");
                _api.AddRoute(route);
                CrestronConsole.PrintLine("UdmCwsServer: Route added, total routes: {0}", _api.Routes.Count);

                // Register the server
                CrestronConsole.PrintLine("UdmCwsServer: Calling _api.Register()...");
                _api.Register();
                CrestronConsole.PrintLine("UdmCwsServer: _api.Register() completed");

                _isRunning = true;

                // Get the actual IP address
                var ipAddress = CrestronEthernetHelper.GetEthernetParameter(
                    CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, 0);

                CrestronConsole.PrintLine("UdmCwsServer: Server started successfully");
                CrestronConsole.PrintLine("UdmCwsServer: Route registered: /cws{0}/{1}", serverPrefix, appRoute);
                CrestronConsole.PrintLine("UdmCwsServer: Access at http(s)://{0}/cws{1}/{2}", ipAddress, serverPrefix, appRoute);

                // Print all routes
                CrestronConsole.PrintLine("UdmCwsServer: ===== Registered Routes =====");
                foreach (var r in _api.Routes)
                {
                    CrestronConsole.PrintLine("UdmCwsServer:   - Name: {0}, Path: {1}, Handler: {2}", r.Name, r.Url, r.RouteHandler.GetType().Name);
                }
                CrestronConsole.PrintLine("UdmCwsServer: ============================");
            }
            catch (System.Exception ex)
            {
                CrestronConsole.PrintLine("UdmCwsServer: Error starting server - {0}", ex.Message);
                ErrorLog.Error("UdmCwsServer: Error starting server - {0}", ex.Message);
                _isRunning = false;
            }
        }

        /// <summary>
        /// Default request handler that logs all incoming requests
        /// </summary>
        private void DefaultRequestHandler(object sender, HttpCwsRequestEventArgs args)
        {
            try
            {
                CrestronConsole.PrintLine("UdmCwsServer: ReceivedRequestEvent - Path: {0}, Method: {1}",
                    args.Context.Request.Path, args.Context.Request.HttpMethod);
            }
            catch (System.Exception ex)
            {
                ErrorLog.Error("UdmCwsServer: Error in DefaultRequestHandler - {0}", ex.Message);
            }
        }

        /// <summary>
        /// Stop the CWS server
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                CrestronConsole.PrintLine("UdmCwsServer: Server is not running");
                return;
            }

            try
            {
                if (_api != null)
                {
                    _api.ReceivedRequestEvent -= DefaultRequestHandler;
                    _api.Unregister();
                    _api.Dispose();
                    _api = null;
                }

                _isRunning = false;
                CrestronConsole.PrintLine("UdmCwsServer: Server stopped successfully");
            }
            catch (System.Exception ex)
            {
                CrestronConsole.PrintLine("UdmCwsServer: Error stopping server - {0}", ex.Message);
                ErrorLog.Error("UdmCwsServer: Error stopping server - {0}", ex.Message);
            }
        }

        /// <summary>
        /// Check if server is running
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }
    }
}
