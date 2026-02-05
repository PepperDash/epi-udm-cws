using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace UdmCws.Test.Room
{
    /// <summary>
    /// Factory for creating MockRoom instances
    /// </summary>
    public class MockRoomFactory : EssentialsPluginDeviceFactory<MockRoom>
    {
        public MockRoomFactory()
        {
            TypeNames = new List<string> { "mockroom", "mockRoom", "MockRoom" };
            MinimumEssentialsFrameworkVersion = "2.24.0";
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Building MockRoom device: {Key}", dc.Key);

            var config = dc.Properties.ToObject<MockRoomConfig>();
            return new MockRoom(dc.Key, dc.Name, config);
        }
    }
}
