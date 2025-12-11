using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;
using PepperDash.Core.Web.RequestHandlers;

namespace PepperDash.Plugin.UdmCws
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
