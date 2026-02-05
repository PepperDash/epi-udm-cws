using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Serilog.Events;
using System.Collections.Generic;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Properties configuration for UDM-CWS from Essentials config JSON
    /// Maps directly to the properties section of the device config
    /// </summary>
    public class UdmCwsPropertiesConfig
    {
        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonProperty("psk")]
        public string Psk { get; set; }

        [JsonProperty("feedbackMode")]
        public string FeedbackMode { get; set; }

        [JsonProperty("routePrefix")]
        public string RoutePrefix { get; set; }

        [JsonProperty("deviceMappings")]
        public List<DeviceMapping> DeviceMappings { get; set; }

        [JsonProperty("roomStateActions")]
        public RoomStateActions RoomStateActions { get; set; }

        [JsonProperty("standardProperties")]
        public StandardPropertiesConfig StandardProperties { get; set; }

        public UdmCwsPropertiesConfig()
        {
            ApiVersion = "1.0.0";
            Psk = string.Empty;
            FeedbackMode = "deferred";
            RoutePrefix = string.Empty;
            DeviceMappings = new List<DeviceMapping>();
            RoomStateActions = new RoomStateActions();
            StandardProperties = new StandardPropertiesConfig();
        }
    }

    /// <summary>
    /// Factory for creating UdmCwsStateManager instances
    /// </summary>
    public class UdmCwsStateManagerFactory : EssentialsPluginDeviceFactory<UdmCwsStateManager>
    {
        public UdmCwsStateManagerFactory()
        {
            // Set the minimum Essentials Framework Version
            MinimumEssentialsFrameworkVersion = "2.0.0";

            // TypeNames that will build an instance of this device
            TypeNames = new List<string>() { "UdmCwsStateManager", "udmcws" };
        }

        /// <summary>
        /// Builds and returns an instance of UdmCwsStateManager
        /// </summary>
        /// <param name="dc">device configuration</param>
        /// <returns>UdmCwsStateManager instance</returns>
        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            Debug.LogMessage(LogEventLevel.Information, "[{Key}] Factory: Creating UdmCwsStateManager", dc.Key);

            // Parse properties config from JSON
            var propertiesConfig = dc.Properties.ToObject<UdmCwsPropertiesConfig>();

            // Create full UdmCwsConfiguration
            var configuration = new UdmCwsConfiguration
            {
                ApiVersion = propertiesConfig?.ApiVersion ?? "1.0.0",
                Psk = propertiesConfig?.Psk ?? string.Empty,
                RoutePrefix = propertiesConfig?.RoutePrefix ?? string.Empty,
                DeviceMappings = propertiesConfig?.DeviceMappings ?? new List<DeviceMapping>(),
                RoomStateActions = propertiesConfig?.RoomStateActions ?? new RoomStateActions(),
                StandardProperties = propertiesConfig?.StandardProperties ?? new StandardPropertiesConfig()
            };

            // Parse feedback mode
            if (propertiesConfig != null && !string.IsNullOrEmpty(propertiesConfig.FeedbackMode))
            {
                configuration.FeedbackMode = propertiesConfig.FeedbackMode.ToLower() == "immediate"
                    ? FeedbackMode.Immediate
                    : FeedbackMode.Deferred;

                Debug.LogMessage(LogEventLevel.Information, "[{Key}] Factory: Feedback mode: {FeedbackMode}", dc.Key, configuration.FeedbackMode);
            }

            // Log configuration summary
            Debug.LogMessage(LogEventLevel.Information, "[{Key}] Factory: API Version: {ApiVersion}", dc.Key, configuration.ApiVersion);
            Debug.LogMessage(LogEventLevel.Debug, "[{Key}] Factory: PSK Configured: {PskConfigured}", dc.Key, !string.IsNullOrEmpty(configuration.Psk));
            Debug.LogMessage(LogEventLevel.Information, "[{Key}] Factory: Route Prefix: {RoutePrefix}", dc.Key,
                string.IsNullOrEmpty(configuration.RoutePrefix) ? "(default)" : configuration.RoutePrefix);
            Debug.LogMessage(LogEventLevel.Information, "[{Key}] Factory: Device mappings: {Count}", dc.Key, configuration.DeviceMappings.Count);

            if (configuration.RoomStateActions != null)
            {
                Debug.LogMessage(LogEventLevel.Information, "[{Key}] Factory: Shutdown actions: {ShutdownCount}",
                    dc.Key, configuration.RoomStateActions.Shutdown?.Count ?? 0);
                Debug.LogMessage(LogEventLevel.Information, "[{Key}] Factory: Active actions: {ActiveCount}",
                    dc.Key, configuration.RoomStateActions.Active?.Count ?? 0);
            }

            return new UdmCwsStateManager(dc.Key, configuration);
        }
    }
}
