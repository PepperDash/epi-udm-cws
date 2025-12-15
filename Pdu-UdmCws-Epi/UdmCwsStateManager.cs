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
            AddPreActivationAction(AddWebApiPaths);
        }


        public State GetRoomResponse => State;

        private void AddWebApiPaths()
        {
            this.LogDebug("Adding UdmCws Web API Handler");
            var serverHandler = new UdmCwsServerHandler(() => GetRoomResponse);
            serverHandler.AddRoute();

        }

    }
}
