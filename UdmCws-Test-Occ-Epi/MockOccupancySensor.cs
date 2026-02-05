using System;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace UdmCws.Test.Occupancy
{
    /// <summary>
    /// Simple mock occupancy sensor for testing UDM-CWS
    /// Toggles occupied state every 30 seconds for testing
    /// </summary>
    public class MockOccupancySensor : EssentialsDevice, IOccupancyStatusProvider
    {
        public BoolFeedback RoomIsOccupiedFeedback { get; private set; }

        private bool _isOccupied;
        private CTimer _toggleTimer;

        public MockOccupancySensor(string key, string name)
            : base(key, name)
        {
            RoomIsOccupiedFeedback = new BoolFeedback(key + "-OccupiedFb", () => _isOccupied);
            _isOccupied = false;

            // Toggle every 30 seconds for testing
            _toggleTimer = new CTimer(_ => ToggleOccupancy(), null, 30000, 30000);

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockOccupancySensor created: {Key}", Key);
        }

        private void ToggleOccupancy()
        {
            _isOccupied = !_isOccupied;
            RoomIsOccupiedFeedback.FireUpdate();
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockOccupancySensor occupancy changed: {Occupied}", _isOccupied);
        }

        /// <summary>
        /// Manually set occupancy for testing
        /// </summary>
        public void SetOccupied(bool occupied)
        {
            _isOccupied = occupied;
            RoomIsOccupiedFeedback.FireUpdate();
        }
    }
}
