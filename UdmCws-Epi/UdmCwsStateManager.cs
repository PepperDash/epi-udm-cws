using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Plugin.UdmCws.Epi.StateBuilder;
using PepperDash.Plugin.UdmCws.Epi.ActionExecutor;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// UDM-CWS State Manager - Configuration-driven with auto-population
    /// Uses Crestron's HttpCwsServer with our library's handler
    /// </summary>
    public class UdmCwsStateManager : EssentialsDevice
    {
        private readonly UdmCwsConfiguration _configuration;
        private HttpCwsServer _server;
        private UdmCwsActionPathsHandler _handler;
        private ConventionBasedStateBuilder _stateBuilder;
        private RoomActionExecutor _actionExecutor;

        /// <summary>
        /// Constructor with full configuration
        /// </summary>
        public UdmCwsStateManager(string key, UdmCwsConfiguration configuration) : base(key)
        {
            _configuration = configuration ?? new UdmCwsConfiguration();

            // Create state builder and action executor
            _stateBuilder = new ConventionBasedStateBuilder(_configuration);
            _actionExecutor = new RoomActionExecutor(_configuration, _stateBuilder);

            AddPreActivationAction(AddWebApiPaths);
        }

        /// <summary>
        /// Constructor with simple config (for backward compatibility)
        /// </summary>
        public UdmCwsStateManager(string key, UdmCwsConfig config)
            : this(key, new UdmCwsConfiguration { FeedbackMode = config.FeedbackMode })
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public UdmCwsStateManager(string key)
            : this(key, new UdmCwsConfiguration())
        {
        }

        /// <summary>
        /// Creates Crestron CWS server with our library's handler
        /// Uses Crestron's HttpCwsServer (same as SIMPL+ and EssentialsWebApi)
        /// </summary>
        private void AddWebApiPaths()
        {
            this.LogDebug("Creating UDM-CWS server using Crestron HttpCwsServer");

            try
            {
                // Get application number for URL prefix (specific to UDM-CWS to avoid conflicts)
                var appNum = InitialParametersClass.ApplicationNumber;
                var prefix = string.Format("/app{0:D2}/udmcws", appNum);

                // Create Crestron's HttpCwsServer (same as EssentialsWebApi uses)
                _server = new HttpCwsServer(prefix);
                _server.Register();

                this.LogDebug("Crestron CWS server registered on {0}", prefix);

                // Create config for handler
                var handlerConfig = new UdmCwsConfig { FeedbackMode = _configuration.FeedbackMode };

                // Create handler from Lib (single CWS implementation)
                _handler = new UdmCwsActionPathsHandler(handlerConfig);

                // Subscribe to handler events - use state builder and action executor
                _handler.ReportStateEvent += OnReportStateEvent;
                _handler.DesiredStateEvent += OnDesiredStateEvent;

                // Add our route to the server with optional route prefix
                _handler.AddRoute(_server, _configuration.RoutePrefix);

                // Log the full endpoint path
                var routePath = string.IsNullOrEmpty(_configuration.RoutePrefix)
                    ? "roomstatus"
                    : string.Format("{0}/roomstatus", _configuration.RoutePrefix.Trim('/'));
                this.LogDebug("UDM-CWS endpoint available at {0}/{1}", prefix, routePath);
            }
            catch (Exception ex)
            {
                this.LogError("Failed to create UDM-CWS server: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Handle GET requests - auto-populate state from configuration
        /// </summary>
        private void OnReportStateEvent(object sender, ReportStateEventArgs args)
        {
            try
            {
                this.LogDebug("UdmCwsStateManager: Auto-populating state from configuration");
                _stateBuilder.PopulateState(args.ReportedState);
            }
            catch (Exception ex)
            {
                this.LogError("UdmCwsStateManager: Error populating state: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Handle PATCH requests - execute actions from configuration
        /// </summary>
        private void OnDesiredStateEvent(object sender, DesiredStateEventArgs args)
        {
            try
            {
                var desiredState = args.DesiredState.Standard?.State;
                var desiredActivity = args.DesiredState.Standard?.Activity;

                this.LogDebug("UdmCwsStateManager: Executing state change - State: {0}, Activity: {1}",
                    desiredState ?? "null", desiredActivity ?? "null");

                _actionExecutor.ExecuteStateChange(desiredState, desiredActivity);
                args.Success = true;
            }
            catch (Exception ex)
            {
                this.LogError("UdmCwsStateManager: Error executing state change: {0}", ex.Message);
                args.Success = false;
                args.ErrorMessage = ex.Message;
            }
        }
    }
}
