
using PepperDash.Essentials.Core;


namespace PepperDash.Essentials.Plugin.UdmCws
{
    public class UdmCwsStateManager
        : EssentialsDevice
    {
        /// <summary>
        /// The UDM CWS handler instance that manages room response data
        /// </summary>
        public UdmCwsHandler Handler { get; private set; }

        public UdmCwsStateManager(string key) : base(key)
        {
            Handler = new UdmCwsHandler();
            Handler.Initialize();
        }
    }
}