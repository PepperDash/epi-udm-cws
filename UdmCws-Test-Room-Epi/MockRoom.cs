using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;

namespace UdmCws.Test.Room
{
    /// <summary>
    /// Simple mock room implementing IEssentialsRoom directly
    /// For testing UDM-CWS with 20 custom properties, help request, and activity
    /// State changes are instant (no delays) for fast testing
    /// </summary>
    public class MockRoom : EssentialsDevice, IEssentialsRoom
    {
        private readonly MockRoomConfig _config;

        // Expose config for custom property access
        public MockRoomConfig RoomConfig => _config;

        // Expose individual properties for PropertyAccessor (doesn't handle nested paths)
        public string Activity
        {
            get
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockRoom: Activity property accessed, returning: {Value}", _currentActivity);
                return _currentActivity;
            }
        }
        public string HelpRequest
        {
            get
            {
                var value = _config?.HelpRequestMessage ?? "";
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockRoom: HelpRequest property accessed, returning: {Value}", value);
                return value;
            }
        }
        public string Property1 => _config?.Property1 ?? "";
        public string Property2 => _config?.Property2 ?? "";
        public string Property3
        {
            get
            {
                var value = _config?.Property3 ?? "";
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockRoom: Property3 property accessed, returning: {Value}", value);
                return value;
            }
        }
        public string Property4 => _config?.Property4 ?? "";
        public string Property5 => _config?.Property5 ?? "";
        public string Property6 => _config?.Property6 ?? "";
        public string Property7 => _config?.Property7 ?? "";
        public string Property8 => _config?.Property8 ?? "";
        public string Property9 => _config?.Property9 ?? "";
        public string Property10 => _config?.Property10 ?? "";
        public string Property11 => _config?.Property11 ?? "";
        public string Property12 => _config?.Property12 ?? "";
        public string Property13 => _config?.Property13 ?? "";
        public string Property14 => _config?.Property14 ?? "";
        public string Property15 => _config?.Property15 ?? "";
        public string Property16 => _config?.Property16 ?? "";
        public string Property17 => _config?.Property17 ?? "";
        public string Property18 => _config?.Property18 ?? "";
        public string Property19 => _config?.Property19 ?? "";
        public string Property20 => _config?.Property20 ?? "";

        // IEssentialsRoom required properties
        public BoolFeedback OnFeedback { get; private set; }
        public BoolFeedback IsWarmingUpFeedback { get; private set; }
        public BoolFeedback IsCoolingDownFeedback { get; private set; }
        public bool IsMobileControlEnabled => false;
        public IMobileControlRoomMessenger MobileControlRoomBridge => null;
        public string SourceListKey => string.Empty;
        public string DestinationListKey => string.Empty;
        public string AudioControlPointListKey => string.Empty;
        public string CameraListKey => string.Empty;
        public SecondsCountdownTimer ShutdownPromptTimer => null;
        public int ShutdownPromptSeconds => 0;
        public int ShutdownVacancySeconds => 0;
        public eShutdownType ShutdownType => eShutdownType.None;
        public string LogoUrlLightBkgnd => string.Empty;
        public string LogoUrlDarkBkgnd => string.Empty;

        // Testing properties
        public StringFeedback ActivityFeedback { get; private set; }
        public string HelpMessage { get; private set; }
        public Dictionary<string, string> CustomProperties { get; private set; }

        private bool _isOn;
        private bool _isWarming;
        private bool _isCooling;
        private string _currentActivity;

        public MockRoom(string key, string name, MockRoomConfig config)
            : base(key, name)
        {
            _config = config;

            Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "========================================");
            Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "MOCKROOM V2.0 LOADED - CHECK THIS");
            Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "Key: {Key}", key);
            Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "HelpRequestMessage: {Help}", config?.HelpRequestMessage ?? "NULL");
            Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "Property3: {Prop3}", config?.Property3 ?? "NULL");
            Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "========================================");

            // Initialize feedbacks
            OnFeedback = new BoolFeedback(() => _isOn);
            IsWarmingUpFeedback = new BoolFeedback(() => _isWarming);
            IsCoolingDownFeedback = new BoolFeedback(() => _isCooling);
            ActivityFeedback = new StringFeedback(() => _currentActivity);

            HelpMessage = _config?.HelpRequestMessage ?? "Mock room help message";

            // Initialize 20 custom properties
            CustomProperties = new Dictionary<string, string>();
            if (_config != null)
            {
                CustomProperties["property1"] = _config.Property1 ?? GenerateRandomValue();
                CustomProperties["property2"] = _config.Property2 ?? GenerateRandomValue();
                CustomProperties["property3"] = _config.Property3 ?? GenerateRandomValue();
                CustomProperties["property4"] = _config.Property4 ?? GenerateRandomValue();
                CustomProperties["property5"] = _config.Property5 ?? GenerateRandomValue();
                CustomProperties["property6"] = _config.Property6 ?? GenerateRandomValue();
                CustomProperties["property7"] = _config.Property7 ?? GenerateRandomValue();
                CustomProperties["property8"] = _config.Property8 ?? GenerateRandomValue();
                CustomProperties["property9"] = _config.Property9 ?? GenerateRandomValue();
                CustomProperties["property10"] = _config.Property10 ?? GenerateRandomValue();
                CustomProperties["property11"] = _config.Property11 ?? GenerateRandomValue();
                CustomProperties["property12"] = _config.Property12 ?? GenerateRandomValue();
                CustomProperties["property13"] = _config.Property13 ?? GenerateRandomValue();
                CustomProperties["property14"] = _config.Property14 ?? GenerateRandomValue();
                CustomProperties["property15"] = _config.Property15 ?? GenerateRandomValue();
                CustomProperties["property16"] = _config.Property16 ?? GenerateRandomValue();
                CustomProperties["property17"] = _config.Property17 ?? GenerateRandomValue();
                CustomProperties["property18"] = _config.Property18 ?? GenerateRandomValue();
                CustomProperties["property19"] = _config.Property19 ?? GenerateRandomValue();
                CustomProperties["property20"] = _config.Property20 ?? GenerateRandomValue();

                _currentActivity = _config.DefaultActivity ?? "idle";
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                    "MockRoom: Initial activity set to: {Activity}", _currentActivity);
            }
        }

        private string GenerateRandomValue()
        {
            var random = new Random();
            return "TestValue" + random.Next(1, 1000);
        }

        public void StartShutdown(eShutdownType type)
        {
            Shutdown();
        }

        public void Shutdown()
        {
            if (_isOn)
            {
                // Set activity to off when room shuts down
                SetActivity("off");

                // Power off devices
                PowerOffDevices();

                // Show cooling state first (for feedback visibility)
                _isCooling = true;
                _isOn = true;
                IsCoolingDownFeedback.FireUpdate();
                OnFeedback.FireUpdate();

                // Then immediately transition to off state (instant for mock)
                _isCooling = false;
                _isOn = false;
                IsCoolingDownFeedback.FireUpdate();
                OnFeedback.FireUpdate();
            }
        }

        public void PowerOnToDefaultOrLastSource()
        {
            if (!_isOn)
            {
                // Show warming state first (for feedback visibility)
                _isWarming = true;
                _isOn = false;
                IsWarmingUpFeedback.FireUpdate();
                OnFeedback.FireUpdate();

                // Then immediately transition to on state (instant for mock)
                _isWarming = false;
                _isOn = true;
                IsWarmingUpFeedback.FireUpdate();
                OnFeedback.FireUpdate();

                // Set default activity to "present" if not already set or if currently "off"
                if (string.IsNullOrEmpty(_currentActivity) ||
                    _currentActivity.Equals("idle", StringComparison.OrdinalIgnoreCase) ||
                    _currentActivity.Equals("off", StringComparison.OrdinalIgnoreCase))
                {
                    SetActivity("present");
                }
                else
                {
                    // Power on devices based on current activity
                    HandleActivityChange(_currentActivity);
                }
            }
        }

        private void PowerOnDevices()
        {
            if (_config?.DeviceKeys == null) return;

            foreach (var deviceKey in _config.DeviceKeys)
            {
                var device = DeviceManager.GetDeviceForKey(deviceKey);
                if (device is IHasPowerControlWithFeedback powerDevice)
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                        "MockRoom: Powering on device {DeviceKey}", deviceKey);
                    powerDevice.PowerOn();
                }
            }
        }

        private void PowerOffDevices()
        {
            if (_config?.DeviceKeys == null) return;

            foreach (var deviceKey in _config.DeviceKeys)
            {
                var device = DeviceManager.GetDeviceForKey(deviceKey);
                if (device is IHasPowerControlWithFeedback powerDevice)
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                        "MockRoom: Powering off device {DeviceKey}", deviceKey);
                    powerDevice.PowerOff();
                }
            }
        }

        public void SetActivity(string activity)
        {
            var previousActivity = _currentActivity;
            _currentActivity = activity;
            ActivityFeedback.FireUpdate();

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockRoom: Activity changed from {Previous} to {Current}, Room is {RoomState}",
                previousActivity, activity, _isOn ? "ON" : "OFF");

            // If activity is set to "off", shut down the room
            if (activity?.Equals("off", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (_isOn)
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                        "MockRoom: Activity set to OFF - shutting down room");

                    // Power off devices
                    PowerOffDevices();

                    // Show cooling state first (for feedback visibility)
                    _isCooling = true;
                    _isOn = true;
                    IsCoolingDownFeedback.FireUpdate();
                    OnFeedback.FireUpdate();

                    // Then immediately transition to off state (instant for mock)
                    _isCooling = false;
                    _isOn = false;
                    IsCoolingDownFeedback.FireUpdate();
                    OnFeedback.FireUpdate();
                }
                return;
            }

            // Handle activity-based device control when room is on
            if (_isOn)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                    "MockRoom: Calling HandleActivityChange for activity {Activity}", activity);
                HandleActivityChange(activity);
            }
            else
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                    "MockRoom: Skipping HandleActivityChange - room is OFF");
            }
        }

        private void HandleActivityChange(string activity)
        {
            if (_config?.DeviceKeys == null)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Warning,
                    "MockRoom: No device keys configured - cannot handle activity change");
                return;
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockRoom: Handling activity change to {Activity} for {DeviceCount} devices",
                activity, _config.DeviceKeys.Length);

            // Simple demo logic for common activities
            var lowerActivity = activity?.ToLower() ?? "";

            foreach (var deviceKey in _config.DeviceKeys)
            {
                var device = DeviceManager.GetDeviceForKey(deviceKey);
                if (device == null)
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Warning,
                        "MockRoom: Device not found: {DeviceKey}", deviceKey);
                    continue;
                }

                if (device is IHasPowerControlWithFeedback powerDevice)
                {
                    var shouldBeOn = ShouldDeviceBeOnForActivity(deviceKey, lowerActivity);
                    var isOn = powerDevice.PowerIsOnFeedback.BoolValue;
                    var isWarming = (device is IWarmingCooling wc) && wc.IsWarmingUpFeedback.BoolValue;
                    var isCooling = (device is IWarmingCooling wc2) && wc2.IsCoolingDownFeedback.BoolValue;

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                        "MockRoom: Device {DeviceKey} - ShouldBeOn={ShouldBeOn}, IsOn={IsOn}, IsWarming={IsWarming}, IsCooling={IsCooling}",
                        deviceKey, shouldBeOn, isOn, isWarming, isCooling);

                    if (shouldBeOn && !isOn && !isWarming)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                            "MockRoom: Powering ON {DeviceKey} for activity {Activity}", deviceKey, activity);
                        powerDevice.PowerOn();

                        // Set source immediately for mock - no warm up delay
                        SetDeviceSourceForActivity(device, deviceKey, lowerActivity);
                    }
                    else if (!shouldBeOn && isOn && !isCooling)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                            "MockRoom: Powering OFF {DeviceKey} for activity {Activity}", deviceKey, activity);
                        powerDevice.PowerOff();
                    }
                    else if (shouldBeOn && isOn)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                            "MockRoom: Device {DeviceKey} already on, switching source for activity {Activity}",
                            deviceKey, activity);
                        // Device already on, just switch source if needed
                        SetDeviceSourceForActivity(device, deviceKey, lowerActivity);
                    }
                }
                else
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Warning,
                        "MockRoom: Device {DeviceKey} does not implement IHasPowerControlWithFeedback", deviceKey);
                }
            }
        }

        private bool ShouldDeviceBeOnForActivity(string deviceKey, string activity)
        {
            var key = deviceKey.ToLower();

            // idle or off = everything off
            if (activity == "idle" || activity == "off")
                return false;

            // call = displays, codec, camera, audio, lighting, switcher, mics
            if (activity == "call")
            {
                return key.Contains("display-front") || key.Contains("codec") || key.Contains("camera") ||
                       key.Contains("dsp") || key.Contains("audio") || key.Contains("mic") ||
                       key.Contains("lighting") || key.Contains("switcher");
            }

            // present = all displays, projector, doc camera, audio, lighting, switcher, mics
            if (activity == "present")
            {
                return key.Contains("display") || key.Contains("projector") || key.Contains("doc-camera") ||
                       key.Contains("dsp") || key.Contains("audio") || key.Contains("mic") ||
                       key.Contains("lighting") || key.Contains("switcher");
            }

            // Unknown activity - keep devices on
            return true;
        }

        private void SetDeviceSourceForActivity(IKeyed device, string deviceKey, string activity)
        {
            var key = deviceKey.ToLower();

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockRoom: SetDeviceSourceForActivity - DeviceKey={DeviceKey}, Activity={Activity}",
                deviceKey, activity);

            // Only set sources for displays
            if (!key.Contains("display"))
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                    "MockRoom: Device {DeviceKey} is not a display - skipping source change", deviceKey);
                return;
            }

            string videoSource = null;
            string audioSource = null;

            // call = codec source on displays
            if (activity == "call")
            {
                videoSource = "Codec";
                audioSource = "Codec";
            }
            // present = Apple TV source on displays
            else if (activity == "present")
            {
                videoSource = "Apple TV";
                audioSource = "Apple TV";
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockRoom: Selected source for activity {Activity}: Video={Video}, Audio={Audio}",
                activity, videoSource ?? "null", audioSource ?? "null");

            // Use reflection to call SetSource on mock devices (keeps plugins independent)
            if (videoSource != null)
            {
                var setSourceMethod = device.GetType().GetMethod("SetSource");
                if (setSourceMethod != null)
                {
                    try
                    {
                        setSourceMethod.Invoke(device, new object[] { videoSource, audioSource });
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                            "MockRoom: Successfully set {DeviceKey} source to {Source} for activity {Activity}",
                            deviceKey, videoSource, activity);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Warning,
                            "MockRoom: Could not set source on {DeviceKey}: {Error}", deviceKey, ex.Message);
                    }
                }
                else
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Warning,
                        "MockRoom: Device {DeviceKey} does not have SetSource method", deviceKey);
                }
            }
        }

        public string GetCustomProperty(string propertyKey)
        {
            return CustomProperties.ContainsKey(propertyKey.ToLower())
                ? CustomProperties[propertyKey.ToLower()]
                : null;
        }

        // IReconfigurableDevice members
        public DeviceConfig Config { get; private set; }
        public event EventHandler<EventArgs> ConfigChanged;
        public void SetConfig(DeviceConfig config)
        {
            Config = config;
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        // IRunDefaultPresentRoute members
        public bool RunDefaultPresentRoute() { return false; }

        // IRunRouteAction members
        public void RunRouteAction(string routeKey, string sourceListKey) { }

        // IRunDefaultCallRoute members
        public void RunDefaultCallRoute() { }

        // IEnvironmentalControls members
        public List<EssentialsDevice> EnvironmentalControlDevices => new List<EssentialsDevice>();
        public bool HasEnvironmentalControlDevices => false;
    }
}
