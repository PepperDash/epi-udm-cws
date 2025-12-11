using Crestron.SimplSharp.WebScripting;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Web;
using PepperDash.Essentials.WebApiHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsStateManager : EssentialsDevice
    {

        
        public State State { get; private set; } = MockState.GetMockState();
        public UdmCwsStateManager(string key) : base(key)
        {
            State = new State();
            AddPreActivationAction(AddWebApiPaths);
        }


        public State GetRoomResponse => State;

        private void AddWebApiPaths()
        {
            var apiServer = DeviceManager
                .AllDevices.OfType<EssentialsWebApi>()
                .FirstOrDefault(d => d.Key == "essentialsWebApi");

            if (apiServer == null)
            {
                this.LogWarning("No API Server available");
                return;
            }

            // TODO: Add routes for the rest of the MC console commands
            
            var route = new HttpCwsRoute("/roomstatus")
            {
                Name = "UdmCWSRoomStatus",
                RouteHandler = new UdmCwsActionPathsHandler(() => GetRoomResponse)
            };
            apiServer.AddRoute(route);
        }

    }
}
