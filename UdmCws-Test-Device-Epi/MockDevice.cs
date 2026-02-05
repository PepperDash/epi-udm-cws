using System;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace UdmCws.Test.Device
{
    /// <summary>
    /// Comprehensive mock device for testing all UDM-CWS features
    /// Implements: Power, Warming/Cooling, Communication Monitor, Usage Tracking, Routing
    /// </summary>
    public class MockDevice : EssentialsDevice,
        IHasPowerControlWithFeedback,
        IWarmingCooling,
        ICommunicationMonitor,
        IUsageTracking,
        IRoutingSink
    {
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

        public MockDevice(string key, string name)
            : base(key, name)
        {
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

            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockDevice created: {Key}", Key);
        }

        public void PowerOn()
        {
            if (!_powerIsOn)
            {
                _isWarming = true;
                IsWarmingUpFeedback.FireUpdate();

                // Simulate warming
                new CTimer(_ =>
                {
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

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockDevice powered on: {Key}", Key);
                }, 2000);
            }
        }

        public void PowerOff()
        {
            if (_powerIsOn)
            {
                _isCooling = true;
                IsCoolingDownFeedback.FireUpdate();

                // Simulate cooling
                new CTimer(_ =>
                {
                    _isCooling = false;
                    _powerIsOn = false;
                    IsCoolingDownFeedback.FireUpdate();
                    PowerIsOnFeedback.FireUpdate();

                    // Stop usage tracking
                    UsageTracker.EndDeviceUsage();

                    // Clear source when powered off
                    CurrentSourceInfo = null;
                    CurrentSourceInfoKey = string.Empty;

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "MockDevice powered off: {Key}", Key);
                }, 2000);
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
