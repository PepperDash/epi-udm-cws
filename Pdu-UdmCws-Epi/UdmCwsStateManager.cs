using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdu_UdmCws_Epi
{
    public class UdmCwsStateManager : EssentialsDevice
    {
        public RoomResponse State { get; private set; }
        public UdmCwsStateManager(string key) : base(key)
        {
            State = new RoomResponse();
        }

        public RoomResponse GetState() => State;
    }
}
