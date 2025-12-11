using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace PepperDash.Essentials.Plugin.UdmCws
{
    public class UdmCwsStateManagerFactory
    {
        public static void LoadPlugin()
        {
            DeviceFactory.AddFactoryForType("udmcwsstatemanager", BuildDevice);
        }

        private static EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            return new UdmCwsStateManager(dc.Key);
            // DeviceFactory handles adding to DeviceManager
        }
    }
}
