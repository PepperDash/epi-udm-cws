using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Plugin.UdmCws.Epi.Reflection;
using Serilog.Events;

namespace PepperDash.Plugin.UdmCws.Epi.StateBuilder
{
    /// <summary>
    /// Convention-based state builder for EPI
    /// Orchestrates multiple populators to build complete state
    /// </summary>
    public class ConventionBasedStateBuilder
    {
        private readonly UdmCwsConfiguration _config;
        private readonly PropertyAccessor _propertyAccessor;
        private readonly ConfigurablePropertyHandler _propertyHandler;

        // Populators (Single Responsibility Principle)
        private readonly DevicePropertyPopulator _devicePopulator;
        private readonly StandardPropertyPopulator _standardPopulator;
        private readonly RoutingSourcePopulator _routingPopulator;
        private readonly ConfigurablePropertyPopulator _configurablePopulator;

        public ConventionBasedStateBuilder(UdmCwsConfiguration config)
        {
            _config = config;

            // Initialize reflection system
            _propertyAccessor = new PropertyAccessor();
            _propertyHandler = new ConfigurablePropertyHandler(_propertyAccessor);

            // Initialize all property mappings at startup (reflect once)
            _propertyHandler.Initialize(config);

            // Create populators
            _devicePopulator = new DevicePropertyPopulator();
            _standardPopulator = new StandardPropertyPopulator(config);
            _routingPopulator = new RoutingSourcePopulator();
            _configurablePopulator = new ConfigurablePropertyPopulator(config, _propertyHandler);

            Debug.LogMessage(LogEventLevel.Information, "ConventionBasedStateBuilder: Initialized with {DeviceCount} device mappings",
                config.DeviceMappings?.Count ?? 0);
        }

        #region Public API

        /// <summary>
        /// Populates complete state from configuration with convention-based auto-detection
        /// </summary>
        public void PopulateState(State state)
        {
            // Set API version
            state.ApiVersion = _config.ApiVersion;

            // Populate standard properties (room-level)
            PopulateStandardPropertiesSection(state.Standard);

            // Populate devices (device-level)
            PopulateDevicesSection(state.Status.Devices);

            // Populate custom properties (room-level)
            _configurablePopulator.PopulateCustomProperties(state.Custom);
        }

        #endregion

        #region Standard Properties (Room-Level)

        /// <summary>
        /// Populates all standard (room-level) properties
        /// Combines interface-based and configurable properties
        /// </summary>
        private void PopulateStandardPropertiesSection(StandardProperties standard)
        {
            // Interface-based properties (version, state, occupancy)
            _standardPopulator.PopulateStandardProperties(standard);

            // Configurable properties (activity, helpRequest)
            _configurablePopulator.PopulateActivity(standard);
            _configurablePopulator.PopulateHelpRequest(standard);

            // Error is populated separately if needed
            standard.Error = standard.Error ?? "";
        }

        #endregion

        #region Device Properties (Device-Level)

        /// <summary>
        /// Populates all device properties from device mappings
        /// </summary>
        private void PopulateDevicesSection(System.Collections.Generic.Dictionary<string, DeviceStatus> devices)
        {
            if (_config.DeviceMappings == null)
            {
                return;
            }

            foreach (var mapping in _config.DeviceMappings)
            {
                PopulateDeviceFromMapping(devices, mapping);
            }
        }

        /// <summary>
        /// Populates a single device from a device mapping
        /// </summary>
        private void PopulateDeviceFromMapping(System.Collections.Generic.Dictionary<string, DeviceStatus> devices, DeviceMapping mapping)
        {
            // Get device from DeviceManager
            var device = DeviceManager.GetDeviceForKey(mapping.DeviceKey);
            if (device == null)
            {
                Debug.LogMessage(LogEventLevel.Error, "ConventionBasedStateBuilder: Device not found: {DeviceKey}", mapping.DeviceKey);
                return;
            }

            // Get or create device status
            var deviceKey = $"device{mapping.DeviceIndex}";
            var deviceStatus = devices[deviceKey];

            // Populate all device properties
            PopulateAllDeviceProperties(device, deviceStatus, mapping);
        }

        /// <summary>
        /// Populates all properties for a single device
        /// Orchestrates all device-level populators
        /// </summary>
        private void PopulateAllDeviceProperties(object device, DeviceStatus status, DeviceMapping mapping)
        {
            // Interface-based properties (label, status, description, usage, error)
            _devicePopulator.PopulateDeviceProperties(device, status, mapping);

            // Routing sources (videoSource, audioSource)
            _routingPopulator.PopulateRoutingSources(device, status);

            // Custom properties would be populated here if device-specific custom properties were supported
            // Currently custom properties are room-level only
        }

        #endregion

        #region Activity Set (Write Operation)

        /// <summary>
        /// Sets activity by invoking configured function
        /// Called from RoomActionExecutor on PATCH requests
        /// </summary>
        public void SetActivity(string desiredActivity)
        {
            _configurablePopulator.SetActivity(desiredActivity);
        }

        #endregion
    }
}
