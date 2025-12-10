using Crestron.SimplSharp.WebScripting;
using PepperDash.Core.Web.RequestHandlers;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.WebApiHandlers;
using Pdu_UdmCws_Epi;
using System;


namespace PepperDash.Essentials.Core
{
    public class UdmCwsActionPathsHandler : WebApiBaseRequestHandler
    {
        private readonly UdmCwsController _controller;

        /// <summary>
        /// Creates a new action paths handler for the UDM CWS API
        /// </summary>
        /// <param name="controller">The controller instance that owns the handler data</param>
        public UdmCwsActionPathsHandler(UdmCwsController controller)
        {
            _controller = controller;
        }

        protected override void HandleGet(HttpCwsContext context)
        {
            // Get the current room response from the controller's handler
            var roomResponse = _controller.Handler.GetRoomResponse();
            var response = JsonConvert.SerializeObject(roomResponse);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Content-Type", "application/json");
            context.Response.Write(response, false);
            context.Response.End();
        }
    }
}
