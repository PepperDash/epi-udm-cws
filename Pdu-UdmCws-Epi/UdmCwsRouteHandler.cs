using PepperDash.Core.Web.RequestHandlers;
using PepperDash.Essentials.Core.Web;
using Pdu_UdmCws_Epi;

namespace PepperDash.Essentials.Core
{
    internal class UdmCwsRouteHandler
    {
        private readonly UdmCwsController _controller;

        /// <summary>
        /// Creates a new route handler for the UDM CWS API
        /// </summary>
        /// <param name="controller">The controller instance that owns the handler data</param>
        public UdmCwsRouteHandler(UdmCwsController controller)
        {
            _controller = controller;
            //Note: Should likely redo this!!! I can get the type from essentials, rather than passing it around

        }

        public void AddCwsRoute()
        {
            //Not yet implemented
        }
    }
}
