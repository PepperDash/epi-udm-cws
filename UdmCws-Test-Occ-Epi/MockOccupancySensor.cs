using System;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;

namespace UdmCws.Test.Occupancy
{
    public class MockOccupancySensorConfig
    {
        public string RoomDeviceKey { get; set; }
    }

    /// <summary>
    /// Simple mock occupancy sensor for testing UDM-CWS
    /// Responds to room state changes - occupied when room on, vacant when room off
    /// State changes are instant (no delays) for fast testing
    /// </summary>
    public class MockOccupancySensor : EssentialsDevice, IOccupancyStatusProvider
    {
        public BoolFeedback RoomIsOccupiedFeedback { get; private set; }

        private bool _isOccupied;
        private CTimer _toggleTimer;
        private bool _autoToggleEnabled;
        private readonly MockOccupancySensorConfig _config;
        private CTimer _roomStateDelayTimer;

        public MockOccupancySensor(string key, string name, MockOccupancySensorConfig config = null)
            : base(key, name)
        {
            _config = config ?? new MockOccupancySensorConfig();
            RoomIsOccupiedFeedback = new BoolFeedback(key + "-OccupiedFb", () => _isOccupied);
            _isOccupied = false;  // Start vacant
            _autoToggleEnabled = false;  // Disable auto-toggle by default

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockOccupancySensor created: {Key}, RoomKey: {RoomKey}",
                Key, _config.RoomDeviceKey ?? "none");
        }

        public override bool CustomActivate()
        {
            // Subscribe to room state changes if room device key is configured
            if (!string.IsNullOrEmpty(_config.RoomDeviceKey))
            {
                var room = DeviceManager.GetDeviceForKey(_config.RoomDeviceKey) as IEssentialsRoom;
                if (room != null)
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                        "MockOccupancySensor: Subscribing to room {RoomKey} state changes", _config.RoomDeviceKey);

                    room.OnFeedback.OutputChange += (sender, args) =>
                    {
                        HandleRoomStateChange(args.BoolValue);
                    };

                    // Set initial state based on current room state
                    HandleRoomStateChange(room.OnFeedback.BoolValue);
                }
                else
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Warning,
                        "MockOccupancySensor: Could not find room device {RoomKey}", _config.RoomDeviceKey);
                }
            }

            return base.CustomActivate();
        }

        private void HandleRoomStateChange(bool roomIsOn)
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockOccupancySensor: Room state changed to {State}", roomIsOn ? "ON" : "OFF");

            // Cancel any pending timer
            if (_roomStateDelayTimer != null)
            {
                _roomStateDelayTimer.Stop();
                _roomStateDelayTimer.Dispose();
                _roomStateDelayTimer = null;
            }

            // Instant state change for mock - no delay
            _isOccupied = roomIsOn;
            RoomIsOccupiedFeedback.FireUpdate();
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockOccupancySensor: Occupancy set to {Occupied} (based on room state)", _isOccupied);
        }

        private void ToggleOccupancy()
        {
            if (_autoToggleEnabled)
            {
                _isOccupied = !_isOccupied;
                RoomIsOccupiedFeedback.FireUpdate();
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockOccupancySensor occupancy auto-toggled: {Occupied}", _isOccupied);
            }
        }

        /// <summary>
        /// Manually set occupancy for testing
        /// </summary>
        public void SetOccupied(bool occupied)
        {
            _isOccupied = occupied;
            RoomIsOccupiedFeedback.FireUpdate();
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockOccupancySensor occupancy manually set: {Occupied}", _isOccupied);
        }

        /// <summary>
        /// Enable or disable auto-toggle behavior
        /// </summary>
        public void SetAutoToggle(bool enabled)
        {
            _autoToggleEnabled = enabled;

            if (enabled && _toggleTimer == null)
            {
                _toggleTimer = new CTimer(_ => ToggleOccupancy(), null, 30000, 30000);
            }
            else if (!enabled && _toggleTimer != null)
            {
                _toggleTimer.Stop();
                _toggleTimer.Dispose();
                _toggleTimer = null;
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockOccupancySensor auto-toggle: {Enabled}", enabled);
        }
    }
}
