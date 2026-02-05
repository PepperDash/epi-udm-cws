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
            return new MockOccupancySensor(dc.Key, dc.Name);
        }
    }
}
