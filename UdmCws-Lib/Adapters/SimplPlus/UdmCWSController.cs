using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Static controller for SIMPL+ integration
    /// Bridges SIMPL+ signals to UDM-CWS library
    /// </summary>
    public static class UdmCWSController
    {
        private static StateSignalConverter _signalConverter;
        private static HttpCwsServer _server;
        private static UdmCwsActionPathsHandler _handler;
        private static UdmCwsConfig _config;
        private static bool _isInitialized;
        private static readonly string _prefix;
        private static string _routePrefix = string.Empty;

        // Delegates for SIMPL+ callbacks
        public delegate void ReportStateRequestDelegate(ushort dummy);
        public delegate void RoomStateChangeDelegate(SimplSharpString desiredState);
        public delegate void RoomActivityChangeDelegate(SimplSharpString desiredActivity);

        public static ReportStateRequestDelegate ReportStateRequest { get; set; }
        public static RoomStateChangeDelegate RoomStateChange { get; set; }
        public static RoomActivityChangeDelegate RoomActivityChange { get; set; }

        static UdmCWSController()
        {
            // Get application number for URL prefix (specific to UDM-CWS to avoid conflicts)
            var appNum = InitialParametersClass.ApplicationNumber;
            _prefix = string.Format("/app{0:D2}/udmcws", appNum);
        }

        /// <summary>
        /// Sets the feedback mode (0=Deferred, 1=Immediate)
        /// </summary>
        public static void SetFeedbackMode(ushort mode)
        {
            if (_config == null)
                _config = new UdmCwsConfig();

            _config.FeedbackMode = (FeedbackMode)mode;
            CrestronConsole.PrintLine("UdmCWSController: Feedback mode set to {0}", _config.FeedbackMode);
        }

        /// <summary>
        /// Sets the API version for validation
        /// Call this before Initialize()
        /// </summary>
        public static void SetApiVersion(SimplSharpString version)
        {
            if (_config == null)
                _config = new UdmCwsConfig();

            _config.ApiVersion = version.ToString();
            CrestronConsole.PrintLine("UdmCWSController: API version set to '{0}'", _config.ApiVersion);
        }

        /// <summary>
        /// Sets the pre-shared key for request authentication
        /// Leave empty to disable PSK validation
        /// Call this before Initialize()
        /// </summary>
        public static void SetPsk(SimplSharpString psk)
        {
            if (_config == null)
                _config = new UdmCwsConfig();

            _config.Psk = psk.ToString();
            var pskConfigured = !string.IsNullOrEmpty(_config.Psk);
            CrestronConsole.PrintLine("UdmCWSController: PSK {0}", pskConfigured ? "configured" : "not configured (no security)");
        }

        /// <summary>
        /// Sets the route prefix for multi-room support
        /// Example: "room1" creates route "/app01/udmcws/room1/roomstatus"
        /// Call this before Initialize()
        /// </summary>
        public static void SetRoutePrefix(SimplSharpString prefix)
        {
            _routePrefix = prefix.ToString();
            CrestronConsole.PrintLine("UdmCWSController: Route prefix set to '{0}'", _routePrefix);
        }

        /// <summary>
        /// Initializes the UDM-CWS server
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                CrestronConsole.PrintLine("UdmCWSController: Already initialized");
                return;
            }

            try
            {
                // Build full route path for logging
                var routePath = string.IsNullOrEmpty(_routePrefix)
                    ? "roomstatus"
                    : string.Format("{0}/roomstatus", _routePrefix.Trim('/'));
                CrestronConsole.PrintLine("UdmCWSController: Initializing on {0}/{1}", _prefix, routePath);

                // Create signal converter
                _signalConverter = new StateSignalConverter();

                // Create config if not set
                if (_config == null)
                    _config = new UdmCwsConfig { FeedbackMode = FeedbackMode.Deferred };

                // Create CWS server
                _server = new HttpCwsServer(_prefix);
                _server.Register();

                // Create and configure handler
                _handler = new UdmCwsActionPathsHandler(_config);

                // Subscribe to handler events
                _handler.ReportStateEvent += OnReportStateEvent;
                _handler.DesiredStateEvent += OnDesiredStateEvent;

                // Add route with optional prefix
                _handler.AddRoute(_server, _routePrefix);

                _isInitialized = true;
                CrestronConsole.PrintLine("UdmCWSController: Initialized successfully");
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("UdmCWSController: Initialization error: {0}", ex.Message);
                ErrorLog.Error("UdmCWSController initialization failed: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Stops the CWS server
        /// </summary>
        public static void Shutdown()
        {
            if (!_isInitialized)
                return;

            try
            {
                CrestronConsole.PrintLine("UdmCWSController: Shutting down");

                if (_server != null)
                {
                    _server.Unregister();
                    _server.Dispose();
                    _server = null;
                }

                _isInitialized = false;
                CrestronConsole.PrintLine("UdmCWSController: Shutdown complete");
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("UdmCWSController: Shutdown error: {0}", ex.Message);
                ErrorLog.Error("UdmCWSController shutdown failed: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Handle GET requests - build state from signals
        /// </summary>
        private static void OnReportStateEvent(object sender, ReportStateEventArgs args)
        {
            try
            {
                CrestronConsole.PrintLine("UdmCWSController: Building state from signals");

                // Notify SIMPL+ (optional - signals are already current)
                if (ReportStateRequest != null)
                    ReportStateRequest(1);

                // Build state from current signal values
                var state = _signalConverter.BuildState();

                // Populate the reported state
                args.ReportedState.ApiVersion = state.ApiVersion;
                args.ReportedState.Standard = state.Standard;
                args.ReportedState.Status = state.Status;
                args.ReportedState.Custom = state.Custom;

                CrestronConsole.PrintLine("UdmCWSController: State populated successfully");
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("UdmCWSController: Error building state: {0}", ex.Message);
                ErrorLog.Error("UdmCWSController state building failed: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Handle PATCH requests - extract desired state and activity
        /// </summary>
        private static void OnDesiredStateEvent(object sender, DesiredStateEventArgs args)
        {
            try
            {
                CrestronConsole.PrintLine("UdmCWSController: Processing desired state");

                // Extract desired room state
                var desiredState = _signalConverter.ExtractDesiredRoomState(args.DesiredState);
                if (!string.IsNullOrEmpty(desiredState))
                {
                    CrestronConsole.PrintLine("UdmCWSController: Desired state: {0}", desiredState);
                    if (RoomStateChange != null)
                        RoomStateChange(new SimplSharpString(desiredState));
                }

                // Extract desired room activity
                var desiredActivity = _signalConverter.ExtractDesiredRoomActivity(args.DesiredState);
                if (!string.IsNullOrEmpty(desiredActivity))
                {
                    CrestronConsole.PrintLine("UdmCWSController: Desired activity: {0}", desiredActivity);
                    if (RoomActivityChange != null)
                        RoomActivityChange(new SimplSharpString(desiredActivity));
                }

                args.Success = true;
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("UdmCWSController: Error processing desired state: {0}", ex.Message);
                ErrorLog.Error("UdmCWSController desired state processing failed: {0}", ex.Message);
                args.Success = false;
                args.ErrorMessage = ex.Message;
            }
        }

        // Device property setters (called from SIMPL+ when signals change)
        public static void SetDeviceLabel(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetDeviceLabel(index, value.ToString());
        }

        public static void SetDeviceStatus(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetDeviceStatus(index, value.ToString());
        }

        public static void SetDeviceDescription(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetDeviceDescription(index, value.ToString());
        }

        public static void SetDeviceVideoSource(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetDeviceVideoSource(index, value.ToString());
        }

        public static void SetDeviceAudioSource(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetDeviceAudioSource(index, value.ToString());
        }

        public static void SetDeviceUsage(ushort index, ushort value)
        {
            _signalConverter?.SetDeviceUsage(index, value);
        }

        public static void SetDeviceError(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetDeviceError(index, value.ToString());
        }

        // Custom property setters (called from SIMPL+ when signals change)
        public static void SetCustomLabel(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetCustomLabel(index, value.ToString());
        }

        public static void SetCustomValue(ushort index, SimplSharpString value)
        {
            _signalConverter?.SetCustomValue(index, value.ToString());
        }

        // Aliases for backward compatibility with existing SIMPL modules
        public static void SetPropertyLabel(ushort index, SimplSharpString value)
        {
            SetCustomLabel(index, value);
        }

        public static void SetPropertyValue(ushort index, SimplSharpString value)
        {
            SetCustomValue(index, value);
        }

        // Standard property setters (called from SIMPL+ when signals change)
        public static void SetStandardVersion(SimplSharpString value)
        {
            _signalConverter?.SetStandardVersion(value.ToString());
        }

        public static void SetStandardState(SimplSharpString value)
        {
            _signalConverter?.SetStandardState(value.ToString());
        }

        public static void SetStandardError(SimplSharpString value)
        {
            _signalConverter?.SetStandardError(value.ToString());
        }

        public static void SetStandardOccupancy(ushort value)
        {
            _signalConverter?.SetStandardOccupancy(value == 1);
        }

        public static void SetStandardHelpRequest(SimplSharpString value)
        {
            _signalConverter?.SetStandardHelpRequest(value.ToString());
        }

        public static void SetStandardActivity(SimplSharpString value)
        {
            _signalConverter?.SetStandardActivity(value.ToString());
        }
    }
}
