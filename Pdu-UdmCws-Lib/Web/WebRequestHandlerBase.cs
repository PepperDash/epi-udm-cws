using Crestron.SimplSharp.WebScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperDash.Plugin.UdmCws
{
    public class WebRequestHandlerBase : IHttpCwsHandler
    {
        private readonly Dictionary<string, Action<HttpCwsContext>> _handlers;

        protected readonly bool EnableCors;

        //
        // Summary:
        //     Constructor
        protected WebRequestHandlerBase(bool enableCors)
        {
            EnableCors = enableCors;
            _handlers = new Dictionary<string, Action<HttpCwsContext>>
        {
            { "CONNECT", HandleConnect },
            { "DELETE", HandleDelete },
            { "GET", HandleGet },
            { "HEAD", HandleHead },
            { "OPTIONS", HandleOptions },
            { "PATCH", HandlePatch },
            { "POST", HandlePost },
            { "PUT", HandlePut },
            { "TRACE", HandleTrace }
        };
        }

        //
        // Summary:
        //     Constructor
        protected WebRequestHandlerBase()
            : this(enableCors: false)
        {
        }

        //
        // Summary:
        //     Handles CONNECT method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandleConnect(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles DELETE method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandleDelete(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles GET method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandleGet(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles HEAD method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandleHead(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles OPTIONS method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandleOptions(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles PATCH method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandlePatch(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles POST method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandlePost(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles PUT method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandlePut(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Handles TRACE method requests
        //
        // Parameters:
        //   context:
        protected virtual void HandleTrace(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
            context.Response.StatusDescription = "Not Implemented";
            context.Response.End();
        }

        //
        // Summary:
        //     Process request
        //
        // Parameters:
        //   context:
        public void ProcessRequest(HttpCwsContext context)
        {
            if (_handlers.TryGetValue(context.Request.HttpMethod, out var value))
            {
                if (EnableCors)
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                }

                value(context);
            }
        }
    }
}
