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
        /// <param name="port">Port number (default 80)</param>
        public void Start(GetStateDelegate getStateDelegate, int port = 80)
        {
            if (_isRunning)
            {
                CrestronConsole.PrintLine("UdmCwsServer: Server already running");
                return;
            }

            try
            {
                _getStateDelegate = getStateDelegate;

                // Create the server instance
                _server = new HttpCwsServer("/cws");

                // Create and register the route for app01/roomStatus
                var route = new HttpCwsRoute("app01/roomstatus")
                {
                    Name = "UdmCWSRoomStatus",
                    RouteHandler = new UdmCwsActionPathsHandler(_getStateDelegate)
                };

                _server.Routes.Add(route);

                // Register the server
                _server.Register();

                _isRunning = true;
                CrestronConsole.PrintLine("UdmCwsServer: Server started successfully on port {0}", port);
                CrestronConsole.PrintLine("UdmCwsServer: Access room status at [ip]/cws/app01/roomstatus");
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
