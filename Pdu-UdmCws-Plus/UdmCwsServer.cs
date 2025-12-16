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
        private HttpCwsServer _server;
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
                var appRoute = string.Format("app{0:D2}/roomstatus", appNumber);

                CrestronConsole.PrintLine("UdmCwsServer: App number detected: {0}", appNumber);
                CrestronConsole.PrintLine("UdmCwsServer: Creating server with prefix '/cws'");

                // Create the server instance
                _server = new HttpCwsServer("/cws");
                CrestronConsole.PrintLine("UdmCwsServer: Server instance created");

                // Create and register the route for app[XX]/roomStatus
                CrestronConsole.PrintLine("UdmCwsServer: Creating route '{0}'", appRoute);
                var route = new HttpCwsRoute(appRoute)
                {
                    Name = "UdmCWSRoomStatus",
                    RouteHandler = new UdmCwsActionPathsHandler(_getStateDelegate)
                };
                CrestronConsole.PrintLine("UdmCwsServer: Route object created");

                CrestronConsole.PrintLine("UdmCwsServer: Adding route to server.Routes collection");
                _server.Routes.Add(route);
                CrestronConsole.PrintLine("UdmCwsServer: Route added, total routes: {0}", _server.Routes.Count);

                // Register the server
                CrestronConsole.PrintLine("UdmCwsServer: Calling _server.Register()...");
                _server.Register();
                CrestronConsole.PrintLine("UdmCwsServer: _server.Register() completed");

                _isRunning = true;

                // Get the actual IP address
                var ipAddress = CrestronEthernetHelper.GetEthernetParameter(
                    CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, 0);

                CrestronConsole.PrintLine("UdmCwsServer: Server started successfully");
                CrestronConsole.PrintLine("UdmCwsServer: Route registered: /cws/{0}", appRoute);
                CrestronConsole.PrintLine("UdmCwsServer: Access at {0}/cws/{1}", ipAddress, appRoute);

                // Print all routes
                CrestronConsole.PrintLine("UdmCwsServer: ===== Registered Routes =====");
                foreach (var r in _server.Routes)
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
                if (_server != null)
                {
                    _server.Unregister();
                    _server.Dispose();
                    _server = null;
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
