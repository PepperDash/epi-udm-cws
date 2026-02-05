using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Plugin.UdmCws.Epi.StateBuilder;
using Serilog.Events;

namespace PepperDash.Plugin.UdmCws.Epi.ActionExecutor
{
    /// <summary>
    /// Executes room actions from PATCH requests
    /// Handles state changes (on/off) and activity changes
    /// </summary>
    public class RoomActionExecutor
    {
        private readonly UdmCwsConfiguration _config;
        private readonly ConventionBasedStateBuilder _stateBuilder;

        public RoomActionExecutor(UdmCwsConfiguration config, ConventionBasedStateBuilder stateBuilder)
        {
            _config = config;
            _stateBuilder = stateBuilder;
        }

        /// <summary>
        /// Executes state and activity changes from PATCH request
        /// </summary>
        public void ExecuteStateChange(string desiredState, string desiredActivity)
        {
            // Handle state changes (shutdown, active, standby)
            if (!string.IsNullOrEmpty(desiredState))
            {
                Debug.LogMessage(LogEventLevel.Information, "RoomActionExecutor: Executing state change: {DesiredState}", desiredState);
                ExecuteRoomState(desiredState);
            }

            // Handle activity changes (if provided)
            if (!string.IsNullOrEmpty(desiredActivity))
            {
                Debug.LogMessage(LogEventLevel.Information, "RoomActionExecutor: Activity change requested: {DesiredActivity}", desiredActivity);
                ExecuteActivity(desiredActivity);
            }
        }

        /// <summary>
        /// Executes room state change by calling room methods
        /// Accepts: "on"/"off", "true"/"false", or boolean values
        /// Uses IEssentialsRoom.Shutdown() and PowerOnToDefaultOrLastSource()
        /// </summary>
        private void ExecuteRoomState(string state)
        {
            // Get the room device
            if (string.IsNullOrEmpty(_config.StandardProperties?.RoomDeviceKey))
            {
                Debug.LogMessage(LogEventLevel.Error, "RoomActionExecutor: No room device key configured");
                return;
            }

            var roomDevice = DeviceManager.GetDeviceForKey(_config.StandardProperties.RoomDeviceKey);
            if (roomDevice == null)
            {
                Debug.LogMessage(LogEventLevel.Error, "RoomActionExecutor: Room device not found: {RoomKey}",
                    _config.StandardProperties.RoomDeviceKey);
                return;
            }

            if (!(roomDevice is IEssentialsRoom room))
            {
                Debug.LogMessage(LogEventLevel.Error, "RoomActionExecutor: Device {RoomKey} does not implement IEssentialsRoom",
                    _config.StandardProperties.RoomDeviceKey);
                return;
            }

            // Parse state to boolean (on/off only)
            bool turnOn = false;
            var stateLower = state.ToLower();

            if (stateLower == "on")
            {
                turnOn = true;
            }
            else if (stateLower == "off")
            {
                turnOn = false;
            }
            else
            {
                Debug.LogMessage(LogEventLevel.Warning, "RoomActionExecutor: Invalid state value: {State}. Expected: on or off", state);
                return;
            }

            // Call room methods
            if (turnOn)
            {
                Debug.LogMessage(LogEventLevel.Information, "RoomActionExecutor: Powering on room");
                room.PowerOnToDefaultOrLastSource();
            }
            else
            {
                Debug.LogMessage(LogEventLevel.Information, "RoomActionExecutor: Shutting down room");
                room.Shutdown();
            }
        }

        /// <summary>
        /// Executes activity change by invoking configured function
        /// Uses reflection-based property mapping from state builder
        /// </summary>
        private void ExecuteActivity(string activity)
        {
            Debug.LogMessage(LogEventLevel.Information, "RoomActionExecutor: Activity change requested: {Activity}", activity);

            // Delegate to state builder's configurable property handler
            _stateBuilder.SetActivity(activity);
        }
    }
}
