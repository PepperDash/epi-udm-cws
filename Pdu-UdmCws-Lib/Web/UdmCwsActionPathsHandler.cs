using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;


namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsActionPathsHandler : WebRequestHandlerBase
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
