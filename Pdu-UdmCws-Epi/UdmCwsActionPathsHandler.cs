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
