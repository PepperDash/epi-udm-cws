using PepperDash.Plugin.UdmCws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperDash.Plugin.UdmCws
{
    public interface IHasState
    {
        State GetRoomResponse();
    }
}
