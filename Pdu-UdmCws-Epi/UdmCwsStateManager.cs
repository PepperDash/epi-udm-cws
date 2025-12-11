using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsStateManager : EssentialsDevice, IHasState
    {
        public State State { get; private set; }
        public UdmCwsStateManager(string key) : base(key)
        {
            State = new State();
        }


        public State GetRoomResponse => State;

    }
}
