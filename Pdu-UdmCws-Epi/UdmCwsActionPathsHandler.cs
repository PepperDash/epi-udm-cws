using Crestron.SimplSharp.WebScripting;
using PepperDash.Core.Web.RequestHandlers;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.WebApiHandlers;
using System;


namespace PepperDash.Essentials.Plugin.UdmCws
{
    public class UdmCwsActionPathsHandler : WebApiBaseRequestHandler
    {
        protected override void HandleGet(HttpCwsContext context)
        {
            var response = JsonConvert.SerializeObject(roomResponse);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Content-Type", "application/json");
            context.Response.Write(response, false);
            context.Response.End();
        }
    }
}
