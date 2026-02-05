using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Room.Config;
using Serilog.Events;

namespace PepperDash.Plugin.UdmCws.Epi.StateBuilder
{
    /// <summary>
    /// Populates standard (room-level) properties using Essentials interfaces
    /// Handles: version, state, occupancy
    /// Does NOT handle: activity, helpRequest (those are configurable)
    /// </summary>
    public class StandardPropertyPopulator
    {
        private readonly UdmCwsConfiguration _config;

        public StandardPropertyPopulator(UdmCwsConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Populates all standard properties
        /// </summary>
        public void PopulateStandardProperties(StandardProperties standard)
        {
            PopulateVersion(standard);
            PopulateRoomState(standard);
            PopulateOccupancy(standard);
        }

        #region Version (Config-Driven)

        /// <summary>
        /// Populates API version from config
        /// </summary>
        private void PopulateVersion(StandardProperties standard)
        {
            standard.Version = _config.StandardProperties?.Version ?? "1.0.0";
        }

        #endregion

        #region Room State (IEssentialsRoom, Room)

        /// <summary>
        /// Populates room state from IEssentialsRoom or Room base class
        /// Returns: "On" or "Off" (warming/cooling states are internal only)
        /// </summary>
        private void PopulateRoomState(StandardProperties standard)
        {
            if (string.IsNullOrEmpty(_config.StandardProperties?.RoomDeviceKey))
            {
                Debug.LogMessage(LogEventLevel.Warning, "StandardPropertyPopulator: No room device key configured");
                standard.State = null;
                return;
            }

            Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: Looking for room device key: {RoomKey}", _config.StandardProperties.RoomDeviceKey);

            var roomDevice = DeviceManager.GetDeviceForKey(_config.StandardProperties.RoomDeviceKey);
            if (roomDevice == null)
            {
                Debug.LogMessage(LogEventLevel.Warning, "StandardPropertyPopulator: Room device not found: {RoomKey}", _config.StandardProperties.RoomDeviceKey);
                standard.State = null;
                return;
            }

            Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: Room device found: {RoomKey}, Type: {RoomType}",
                _config.StandardProperties.RoomDeviceKey, roomDevice.GetType().Name);

            // Try IEssentialsRoom interface first
            if (roomDevice is IEssentialsRoom essentialsRoom)
            {
                var onFeedbackValue = essentialsRoom.OnFeedback?.BoolValue;
                Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: Room implements IEssentialsRoom, OnFeedback value: {OnFeedback}", onFeedbackValue);
                standard.State = onFeedbackValue == true ? "On" : "Off";
                return;
            }

            // Fallback to Room base class
            if (roomDevice is Room room)
            {
                var roomIsOnValue = room.RoomIsOnFeedback?.BoolValue;
                Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: Room is Room base class, RoomIsOnFeedback value: {RoomIsOn}", roomIsOnValue);
                standard.State = roomIsOnValue == true ? "On" : "Off";

                // Also extract help message if available
                standard.HelpRequest = room.HelpMessage ?? "";
                return;
            }

            // No room interface found
            Debug.LogMessage(LogEventLevel.Warning, "StandardPropertyPopulator: Room device {RoomKey} does not implement IEssentialsRoom or Room",
                _config.StandardProperties.RoomDeviceKey);
            standard.State = null;
        }

        #endregion

        #region Occupancy (IOccupancyStatusProvider)

        /// <summary>
        /// Populates occupancy status from occupancy sensor interface
        /// </summary>
        private void PopulateOccupancy(StandardProperties standard)
        {
            if (string.IsNullOrEmpty(_config.StandardProperties?.OccupancyDeviceKey))
            {
                Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: No occupancy device key configured");
                standard.Occupancy = null;
                return;
            }

            Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: Looking for occupancy device key: {OccupancyKey}", _config.StandardProperties.OccupancyDeviceKey);

            var occupancyDevice = DeviceManager.GetDeviceForKey(_config.StandardProperties.OccupancyDeviceKey);
            if (occupancyDevice == null)
            {
                Debug.LogMessage(LogEventLevel.Warning, "StandardPropertyPopulator: Occupancy device not found: {OccupancyKey}", _config.StandardProperties.OccupancyDeviceKey);
                standard.Occupancy = null;
                return;
            }

            Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: Occupancy device found: {OccupancyKey}, Type: {OccupancyType}",
                _config.StandardProperties.OccupancyDeviceKey, occupancyDevice.GetType().Name);

            if (occupancyDevice is IOccupancyStatusProvider occupancy)
            {
                var occupiedValue = occupancy.RoomIsOccupiedFeedback?.BoolValue;
                Debug.LogMessage(LogEventLevel.Debug, "StandardPropertyPopulator: Occupancy implements IOccupancyStatusProvider, RoomIsOccupiedFeedback value: {Occupied}", occupiedValue);
                standard.Occupancy = occupiedValue ?? false;
            }
            else
            {
                Debug.LogMessage(LogEventLevel.Warning, "StandardPropertyPopulator: Occupancy device {OccupancyKey} does not implement IOccupancyStatusProvider",
                    _config.StandardProperties.OccupancyDeviceKey);
                standard.Occupancy = null;
            }
        }

        #endregion
    }
}
