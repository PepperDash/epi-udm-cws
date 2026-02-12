using System.Collections.Generic;
using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace UdmCws.Test.Device
{
    /// <summary>
    /// Factory for creating MockDevice instances
    /// </summary>
    public class MockDeviceFactory : EssentialsPluginDeviceFactory<MockDevice>
    {
        public MockDeviceFactory()
        {
            TypeNames = new List<string> { "mockDevice", "mockdevice", "MockDevice" };
            MinimumEssentialsFrameworkVersion = "2.24.0";
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Building MockDevice: {Key}", dc.Key);

            var config = dc.Properties.ToObject<MockDeviceConfig>();
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockDevice {Key} - ActivityDevices: {Activities}",
                dc.Key,
                config?.ActivityDevices != null ? string.Join(", ", config.ActivityDevices) : "none");

            return new MockDevice(dc.Key, dc.Name, config);
        }
    }
}
