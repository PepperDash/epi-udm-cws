
using PepperDash.Essentials.Core;


namespace PepperDash.Essentials.Plugin.UdmCws
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