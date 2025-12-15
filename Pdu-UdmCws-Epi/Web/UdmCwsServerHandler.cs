using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;
using PepperDash.Core;
using PepperDash.Core.Web;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Web;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core.Web.RequestHandlers;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsServerHandler
    {
        HttpCwsRoute route;
        public EssentialsWebApi apiServer;
        public UdmCwsServerHandler(GetStateDelegate getStateDelegate)
        {
            route = new HttpCwsRoute("roomstatus")
            {
                Name = "UdmCWSRoomStatus",
                RouteHandler = new UdmCwsActionPathsHandler(getStateDelegate)
            };
            apiServer = DeviceManager
                .AllDevices.OfType<EssentialsWebApi>()
                .FirstOrDefault(d => d.Key == "essentialsWebApi");
        }

        public void AddRoute()
        {
            if (apiServer == null)
              {
                  Debug.Console(0, "UdmCwsServerHandler: No API Server available");
                  return;
              }
            apiServer.AddRoute(route);
        }
    }
}
