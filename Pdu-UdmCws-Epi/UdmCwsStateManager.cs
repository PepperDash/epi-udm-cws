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
        //create an event to notify when state changes
        private State _state;

        public State GetRoomResponse => _state;


        public void UpdateState(State newState) //this will get called when the state is updated from the device
        {
            //validate the new state as needed and update
            _state = newState;
        }

        public void PatchRequested(State desiredState) //this will get called when a PATCH is made to the web api -ie., from setDesired
        {
            //fire an event to notify listeners that the state has changed
            //the consumers will need to handle the desired change

        }


        public UdmCwsStateManager(string key) : base(key)
        {            
            AddPreActivationAction(AddWebApiPaths);
        }



        private void AddWebApiPaths()
        {
            this.LogDebug("Adding UdmCws Web API Handler");
            var serverHandler = new UdmCwsServerHandler(() => GetRoomResponse);
            serverHandler.AddRoute();

        }

    }
}
