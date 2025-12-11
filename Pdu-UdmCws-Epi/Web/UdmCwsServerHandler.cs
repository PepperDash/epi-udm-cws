using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;
using PepperDash.Core;
using PepperDash.Core.Web;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Web;
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
        EssentialsWebApi apiServer;
        public UdmCwsServerHandler()
        {
            route = new HttpCwsRoute("roomstatus")
            {
                Name = "UdmCWSRoomStatus",
                RouteHandler = new UdmCwsActionPathsHandler(MockState.GetMockState)
            };
            apiServer = DeviceManager
                .AllDevices.OfType<EssentialsWebApi>()
                .FirstOrDefault(d => d.Key == "essentialsWebApi");
        }

        public void AddRoute()
        {
            apiServer.AddRoute(route);
        }







    }
}
