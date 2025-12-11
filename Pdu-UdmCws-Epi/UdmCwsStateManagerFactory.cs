using PepperDash.Core;
using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdu_UdmCws_Epi
{
    public class UdmCwsStateManagerFactory : EssentialsPluginDeviceFactory<UdmCwsStateManager>
    {
        /// <summary>
        /// Plugin device factory constructor
        /// </summary>
        /// <remarks>
        /// Update the MinimumEssentialsFrameworkVersion & TypeNames as needed when creating a plugin
        /// </remarks>
        /// <example>
        /// Set the minimum Essentials Framework Version
        /// <code>
        /// MinimumEssentialsFrameworkVersion = "2.0.0;
        /// </code>
        /// In the constructor we initialize the list with the typenames that will build an instance of this device
        /// <code>
        /// TypeNames = new List<string>() { "UdmCwsStateManager" };
        /// </code>
        /// </example>
        public UdmCwsStateManagerFactory()
        {
            // Set the minimum Essentials Framework Version
            // TODO [ ] Update the Essentials minimum framework version which this plugin has been tested against
            MinimumEssentialsFrameworkVersion = "2.0.0";

            // In the constructor we initialize the list with the typenames that will build an instance of this device
            // TODO [ ] Update the TypeNames for the plugin being developed
            TypeNames = new List<string>() { "UdmCwsStateManager" };
        }

        /// <summary>
        /// Builds and returns an instance of UdmCwsStateManager
        /// </summary>
        /// <param name="dc">device configuration</param>
        /// <returns>UdmCwsStateManager instance</returns>
        public override EssentialsDevice BuildDevice(PepperDash.Essentials.Core.Config.DeviceConfig dc)
        {
            Debug.Console(1, "[{0}] Factory: Creating UdmCwsStateManager", dc.Key);
            return new UdmCwsStateManager(dc.Key);
        }

    }
}
