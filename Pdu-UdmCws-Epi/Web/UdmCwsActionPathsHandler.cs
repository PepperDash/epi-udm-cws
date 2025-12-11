using Crestron.SimplSharp;
using Crestron.SimplSharp.Net;
using Crestron.SimplSharp.WebScripting;
using Independentsoft.Exchange;
using Newtonsoft.Json;
using PepperDash.Core.Web;
using PepperDash.Core.Web.RequestHandlers;
using Renci.SshNet.Security;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsActionPathsHandler : WebApiBaseRequestHandler
    {
       

        protected override void HandleGet(HttpCwsContext context)
        {
            var roomResponse = new UdmCwsHandler().GetRoomResponse();
            var jsonResponse = JsonConvert.SerializeObject(roomResponse);
            //We are going to make the roomResponse here, the lib library will ONLY provide state structure
            //and state building methods
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Content-Type", "application/json");
            context.Response.Write(jsonResponse, false);
            context.Response.End();
        }

        
    }
}
