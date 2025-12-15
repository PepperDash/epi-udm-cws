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

        public UdmCwsActionPathsHandler(GetStateDelegate getStateDelegate)
        {
            getState = getStateDelegate;
        }


        private GetStateDelegate getState;

        protected override void HandleGet(HttpCwsContext context)
        {
            if(getState == null)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                context.Response.Headers.Add("Content-Type", "application/json");
                context.Response.Write("{\"error\":\"State delegate not set.\"}", false);
                context.Response.End();
                return;
            }
            var roomResponse = getState();
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
