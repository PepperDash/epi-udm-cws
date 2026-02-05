using PepperDash.Essentials.Room.Config;

namespace UdmCws.Test.Room
{
    /// <summary>
    /// Configuration for mock room with 20 custom properties
    /// </summary>
    public class MockRoomConfig : EssentialsRoomPropertiesConfig
    {
        // 20 custom properties for testing
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
        public string Property4 { get; set; }
        public string Property5 { get; set; }
        public string Property6 { get; set; }
        public string Property7 { get; set; }
        public string Property8 { get; set; }
        public string Property9 { get; set; }
        public string Property10 { get; set; }
        public string Property11 { get; set; }
        public string Property12 { get; set; }
        public string Property13 { get; set; }
        public string Property14 { get; set; }
        public string Property15 { get; set; }
        public string Property16 { get; set; }
        public string Property17 { get; set; }
        public string Property18 { get; set; }
        public string Property19 { get; set; }
        public string Property20 { get; set; }

        // Help request text
        public string HelpRequestMessage { get; set; }

        // Activity
        public string DefaultActivity { get; set; }

        // Device keys to control when room powers on/off
        public string[] DeviceKeys { get; set; }
    }
}
