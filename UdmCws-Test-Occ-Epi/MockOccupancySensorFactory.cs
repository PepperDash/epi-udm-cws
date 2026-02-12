using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace UdmCws.Test.Occupancy
{
    /// <summary>
    /// Factory for creating MockOccupancySensor instances
    /// </summary>
    public class MockOccupancySensorFactory : EssentialsPluginDeviceFactory<MockOccupancySensor>
    {
        public MockOccupancySensorFactory()
        {
            TypeNames = new List<string> { "mockOccupancy", "mockoccupancy", "MockOccupancy" };
            MinimumEssentialsFrameworkVersion = "2.24.0";
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Building MockOccupancySensor device: {Key}", dc.Key);

            var config = dc.Properties.ToObject<MockOccupancySensorConfig>();
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockOccupancySensor {Key} - RoomDeviceKey: {RoomKey}",
                dc.Key,
                config?.RoomDeviceKey ?? "none");

            return new MockOccupancySensor(dc.Key, dc.Name, config);
        }
    }
}
