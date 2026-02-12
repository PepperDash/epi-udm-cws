using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace UdmCws.Test.Device
{
    /// <summary>
    /// Configuration for MockDevice
    /// </summary>
    public class MockDeviceConfig
    {
        public List<string> ActivityDevices { get; set; }
        public string CustomProperty1 { get; set; }
        public string CustomProperty2 { get; set; }
        public string CustomProperty3 { get; set; }
        public string CustomProperty4 { get; set; }
        public string CustomProperty5 { get; set; }
        public string VideoSourceOn { get; set; }
        public string AudioSourceOn { get; set; }
    }

    /// <summary>
    /// Comprehensive mock device for testing all UDM-CWS features
    /// Implements: Power, Warming/Cooling, Communication Monitor, Usage Tracking, Routing
    /// State changes are instant (no delays) for fast testing
    /// </summary>
    public class MockDevice : EssentialsDevice,
        IHasPowerControlWithFeedback,
        IWarmingCooling,
        ICommunicationMonitor,
        IUsageTracking,
        IRoutingSink
    {
        // Configuration
        private readonly MockDeviceConfig _config;

        // Power control
        public BoolFeedback PowerIsOnFeedback { get; private set; }
        private bool _powerIsOn;

        // Warming/Cooling
        public BoolFeedback IsWarmingUpFeedback { get; private set; }
        public BoolFeedback IsCoolingDownFeedback { get; private set; }
        private bool _isWarming;
        private bool _isCooling;

        // Communication Monitor
        public StatusMonitorBase CommunicationMonitor { get; private set; }

        // Usage Tracking
        public UsageTracking UsageTracker { get; set; }

        // Routing
        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }
        public string CurrentSourceInfoKey { get; set; }
        public SourceListItem CurrentSourceInfo { get; set; }
        public event SourceInfoChangeHandler CurrentSourceChange;
        private string _currentSourceKey;

        // Additional properties for UDM-CWS reporting
        public StringFeedback ErrorFeedback { get; private set; }
        public StringFeedback VideoSourceFeedback { get; private set; }
        public StringFeedback AudioSourceFeedback { get; private set; }
        private string _error;
        private string _videoSource;
        private string _audioSource;

        // Custom properties from config
        public string CustomProperty1 => _config?.CustomProperty1 ?? "";
        public string CustomProperty2 => _config?.CustomProperty2 ?? "";
        public string CustomProperty3 => _config?.CustomProperty3 ?? "";
        public string CustomProperty4 => _config?.CustomProperty4 ?? "";
        public string CustomProperty5 => _config?.CustomProperty5 ?? "";

        public MockDevice(string key, string name, MockDeviceConfig config = null)
            : base(key, name)
        {
            _config = config ?? new MockDeviceConfig { ActivityDevices = new List<string>() };

            // Initialize power
            PowerIsOnFeedback = new BoolFeedback(key + "-PowerFb", () => _powerIsOn);
            _powerIsOn = false;

            // Initialize warming/cooling
            IsWarmingUpFeedback = new BoolFeedback(key + "-WarmingFb", () => _isWarming);
            IsCoolingDownFeedback = new BoolFeedback(key + "-CoolingFb", () => _isCooling);
            _isWarming = false;
            _isCooling = false;

            // Initialize communication monitor (simplified for mock)
            CommunicationMonitor = new MockCommunicationMonitor(this);

            // Initialize usage tracking
            UsageTracker = new UsageTracking(this);

            // Initialize routing
            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();
            CurrentSourceInfoKey = string.Empty;
            CurrentSourceInfo = null;
            _currentSourceKey = string.Empty;

            // Add a mock input port
            InputPorts.Add(new RoutingInputPort("hdmiIn1", eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Hdmi, new Action(SelectHdmiIn1), this));

            // Initialize additional properties
            _error = "No Error";  // Always no error for demo
            _videoSource = "None";
            _audioSource = "None";
            ErrorFeedback = new StringFeedback(key + "-ErrorFb", () => _error);
            VideoSourceFeedback = new StringFeedback(key + "-VideoSrcFb", () => _videoSource);
            AudioSourceFeedback = new StringFeedback(key + "-AudioSrcFb", () => _audioSource);

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockDevice created: {Key}", Key);
        }

        public void PowerOn()
        {
            if (!_powerIsOn)
            {
                // Show warming state first (for feedback visibility)
                _isWarming = true;
                _powerIsOn = false;
                IsWarmingUpFeedback.FireUpdate();
                PowerIsOnFeedback.FireUpdate();

                // Then immediately transition to on state (instant for mock)
                _isWarming = false;
                _powerIsOn = true;
                IsWarmingUpFeedback.FireUpdate();
                PowerIsOnFeedback.FireUpdate();

                // Start usage tracking
                UsageTracker.StartDeviceUsage();

                // Update communication status
                CommunicationMonitor.IsOnline = true;

                // Set default source to HDMI 1
                SelectHdmiIn1();

                // Set default sources when powered on
                _videoSource = _config?.VideoSourceOn ?? "HDMI 1";
                VideoSourceFeedback.FireUpdate();

                _audioSource = _config?.AudioSourceOn ?? "HDMI 1";
                AudioSourceFeedback.FireUpdate();

                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockDevice powered on: {Key}", Key);
            }
        }

        public void PowerOff()
        {
            if (_powerIsOn)
            {
                // Show cooling state first (for feedback visibility)
                _isCooling = true;
                _powerIsOn = true;
                IsCoolingDownFeedback.FireUpdate();
                PowerIsOnFeedback.FireUpdate();

                // Then immediately transition to off state (instant for mock)
                _isCooling = false;
                _powerIsOn = false;
                IsCoolingDownFeedback.FireUpdate();
                PowerIsOnFeedback.FireUpdate();

                // Stop usage tracking
                UsageTracker.EndDeviceUsage();

                // Clear source when powered off
                CurrentSourceInfo = null;
                CurrentSourceInfoKey = string.Empty;

                // Clear sources when powered off
                _videoSource = "None";
                VideoSourceFeedback.FireUpdate();

                _audioSource = "None";
                AudioSourceFeedback.FireUpdate();

                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockDevice powered off: {Key}", Key);
            }
        }

        public void PowerToggle()
        {
            if (_powerIsOn)
                PowerOff();
            else
                PowerOn();
        }

        private void SelectHdmiIn1()
        {
            _currentSourceKey = "hdmiIn1";
            CurrentSourceInfoKey = _currentSourceKey;

            // Create mock source info - only set public settable properties
            CurrentSourceInfo = new SourceListItem
            {
                Name = "HDMI 1",
                Type = eSourceListItemType.Route,
                Icon = ""
            };

            CurrentSourceChange?.Invoke(CurrentSourceInfo, ChangeType.WillChange);
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockDevice source changed: {Source}", _currentSourceKey);
        }

        public override bool CustomActivate()
        {
            // Set mock device as online
            CommunicationMonitor.IsOnline = true;
            CommunicationMonitor.Start();
            return true;
        }

        /// <summary>
        /// Determines if this device should be on for the given activity
        /// </summary>
        public bool ShouldBeOnForActivity(string activity)
        {
            if (string.IsNullOrEmpty(activity) || activity.Equals("idle", StringComparison.OrdinalIgnoreCase) || activity.Equals("off", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // If no activity devices specified, device should be on for all activities
            if (_config?.ActivityDevices == null || !_config.ActivityDevices.Any())
            {
                return true;
            }

            // Check if current activity is in the list of activities for this device
            return _config.ActivityDevices.Any(a => a.Equals(activity, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Set the current video/audio source for demo purposes
        /// </summary>
        public void SetSource(string videoSource, string audioSource = null)
        {
            if (!string.IsNullOrEmpty(videoSource))
            {
                _videoSource = videoSource;
                VideoSourceFeedback.FireUpdate();
            }

            if (!string.IsNullOrEmpty(audioSource))
            {
                _audioSource = audioSource;
                AudioSourceFeedback.FireUpdate();
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information,
                "MockDevice {Key}: Source set to Video={Video}, Audio={Audio}", Key, _videoSource, _audioSource);
        }
    }

    /// <summary>
    /// Simple mock communication monitor for testing
    /// </summary>
    public class MockCommunicationMonitor : StatusMonitorBase
    {
        public MockCommunicationMonitor(IKeyed parent)
            : base(parent, 120000, 300000)  // warning at 2min, error at 5min
        {
            IsOnline = true;
        }

        public override void Start()
        {
            IsOnline = true;
        }

        public override void Stop()
        {
            IsOnline = false;
        }
    }
}
