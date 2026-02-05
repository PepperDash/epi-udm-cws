using System;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Converts between State objects and SIMPL+ signal arrays
    /// Library handles JSON, SIMPL+ only sees signals
    /// </summary>
    public class StateSignalConverter
    {
        // Device signals (20 devices max)
        private readonly string[] _deviceLabels = new string[20];
        private readonly string[] _deviceStatuses = new string[20];
        private readonly string[] _deviceDescriptions = new string[20];
        private readonly string[] _deviceVideoSources = new string[20];
        private readonly string[] _deviceAudioSources = new string[20];
        private readonly ushort[] _deviceUsages = new ushort[20];
        private readonly string[] _deviceErrors = new string[20];

        // Custom property signals (20 properties max)
        private readonly string[] _customLabels = new string[20];
        private readonly string[] _customValues = new string[20];

        // Standard property signals
        private string _standardVersion = string.Empty;
        private string _standardState = string.Empty;
        private string _standardError = string.Empty;
        private bool _standardOccupancy;
        private string _standardHelpRequest = string.Empty;
        private string _standardActivity = string.Empty;

        /// <summary>
        /// Builds a State object from current signal values (for GET requests)
        /// </summary>
        public State BuildState()
        {
            var state = new State();

            // Populate standard properties
            state.Standard.Version = _standardVersion;
            state.Standard.State = _standardState;
            state.Standard.Error = _standardError;
            state.Standard.Occupancy = _standardOccupancy;
            state.Standard.HelpRequest = _standardHelpRequest;
            state.Standard.Activity = _standardActivity;

            // Populate device status (device1 to device20)
            for (int i = 0; i < 20; i++)
            {
                var deviceKey = $"device{i + 1}";
                state.Status.Devices[deviceKey] = new DeviceStatus
                {
                    Label = _deviceLabels[i] ?? string.Empty,
                    Status = _deviceStatuses[i] ?? string.Empty,
                    Description = _deviceDescriptions[i] ?? string.Empty,
                    VideoSource = _deviceVideoSources[i] ?? string.Empty,
                    AudioSource = _deviceAudioSources[i] ?? string.Empty,
                    Usage = _deviceUsages[i],
                    Error = _deviceErrors[i] ?? string.Empty
                };
            }

            // Populate custom properties (property1 to property20)
            for (int i = 0; i < 20; i++)
            {
                var propertyKey = $"property{i + 1}";
                state.Custom[propertyKey] = new CustomProperties
                {
                    Label = _customLabels[i] ?? string.Empty,
                    Value = _customValues[i] ?? string.Empty
                };
            }

            return state;
        }

        /// <summary>
        /// Extracts the desired room state (standard.state) from a PATCH request
        /// Per API spec, standard.state and standard.activity are writable
        /// </summary>
        public string ExtractDesiredRoomState(State desiredState)
        {
            if (desiredState?.Standard?.State != null)
            {
                return desiredState.Standard.State;
            }
            return null;
        }

        /// <summary>
        /// Extracts the desired room activity (standard.activity) from a PATCH request
        /// Per API spec, standard.state and standard.activity are writable
        /// </summary>
        public string ExtractDesiredRoomActivity(State desiredState)
        {
            if (desiredState?.Standard?.Activity != null)
            {
                return desiredState.Standard.Activity;
            }
            return null;
        }

        // Device property setters (called from SIMPL+ when signals change)
        public void SetDeviceLabel(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _deviceLabels[index - 1] = value ?? string.Empty;
        }

        public void SetDeviceStatus(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _deviceStatuses[index - 1] = value ?? string.Empty;
        }

        public void SetDeviceDescription(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _deviceDescriptions[index - 1] = value ?? string.Empty;
        }

        public void SetDeviceVideoSource(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _deviceVideoSources[index - 1] = value ?? string.Empty;
        }

        public void SetDeviceAudioSource(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _deviceAudioSources[index - 1] = value ?? string.Empty;
        }

        public void SetDeviceUsage(int index, ushort value)
        {
            if (index >= 1 && index <= 20)
                _deviceUsages[index - 1] = value;
        }

        public void SetDeviceError(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _deviceErrors[index - 1] = value ?? string.Empty;
        }

        // Custom property setters (called from SIMPL+ when signals change)
        public void SetCustomLabel(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _customLabels[index - 1] = value ?? string.Empty;
        }

        public void SetCustomValue(int index, string value)
        {
            if (index >= 1 && index <= 20)
                _customValues[index - 1] = value ?? string.Empty;
        }

        // Standard property setters (called from SIMPL+ when signals change)
        public void SetStandardVersion(string value)
        {
            _standardVersion = value ?? string.Empty;
        }

        public void SetStandardState(string value)
        {
            _standardState = value ?? string.Empty;
        }

        public void SetStandardError(string value)
        {
            _standardError = value ?? string.Empty;
        }

        public void SetStandardOccupancy(bool value)
        {
            _standardOccupancy = value;
        }

        public void SetStandardHelpRequest(string value)
        {
            _standardHelpRequest = value ?? string.Empty;
        }

        public void SetStandardActivity(string value)
        {
            _standardActivity = value ?? string.Empty;
        }
    }
}
