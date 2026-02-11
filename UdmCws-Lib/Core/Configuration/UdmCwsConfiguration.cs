using System.Collections.Generic;
using PepperDash.Plugin.UdmCws.Configuration;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Complete configuration for UDM-CWS behavior
    /// </summary>
    public class UdmCwsConfiguration
    {
        /// <summary>
        /// API version to use (supports multiple versions in production)
        /// </summary>
        public string ApiVersion { get; set; } = "1.0.0";

        /// <summary>
        /// Pre-shared key for request validation
        /// Leave empty or null to disable PSK validation
        /// </summary>
        public string Psk { get; set; } = string.Empty;

        /// <summary>
        /// Feedback mode for PATCH requests (deferred or immediate)
        /// </summary>
        public FeedbackMode FeedbackMode { get; set; } = FeedbackMode.Deferred;

        /// <summary>
        /// Optional route prefix/room identifier for multi-room support
        /// Example: "room1" creates route "/app01/udmcws/room1/roomstatus"
        /// Leave empty for default "/app01/udmcws/roomstatus"
        /// </summary>
        public string RoutePrefix { get; set; } = string.Empty;

        /// <summary>
        /// Device mappings for auto-population from Essentials devices
        /// </summary>
        public List<DeviceMapping> DeviceMappings { get; set; }

        /// <summary>
        /// Standard properties configuration
        /// </summary>
        public StandardPropertiesConfig StandardProperties { get; set; }

        public UdmCwsConfiguration()
        {
            DeviceMappings = new List<DeviceMapping>();
            StandardProperties = new StandardPropertiesConfig();
        }
    }

    /// <summary>
    /// Maps an Essentials device key to a device index (1-20) in the state
    /// Library auto-detects device interfaces (IPower, IVolume, etc.)
    /// </summary>
    public class DeviceMapping
    {
        /// <summary>
        /// Essentials device key from DeviceManager
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// Device index (1-20) - maps to device1, device2, etc. in state
        /// </summary>
        public int DeviceIndex { get; set; }

        /// <summary>
        /// Optional custom label override (default: uses device.Key)
        /// </summary>
        public string CustomLabel { get; set; }

        /// <summary>
        /// Optional description (e.g., "Front projector", "Confidence monitor")
        /// Config-driven only - no device interface provides this
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Standard properties configuration (room-level, not device-level)
    /// </summary>
    public class StandardPropertiesConfig
    {
        /// <summary>
        /// Static version string for standard.version
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Device key for room (virtual device) - provides room state (on/off)
        /// Room is a virtual device in Essentials device manager
        /// </summary>
        public string RoomDeviceKey { get; set; }

        /// <summary>
        /// Device key for occupancy sensor (must implement IOccupancyStatusProvider)
        /// </summary>
        public string OccupancyDeviceKey { get; set; }

        /// <summary>
        /// Device key for help request source (codec, call device, etc.)
        /// TODO: Need to determine which interface provides help request
        /// </summary>
        public string HelpRequestDeviceKey { get; set; }

        /// <summary>
        /// Device key for activity tracking (call state, sharing, etc.)
        /// TODO: Need to determine which interface provides activity
        /// </summary>
        public string ActivityDeviceKey { get; set; }

        /// <summary>
        /// Activity mapping with get property and set functions
        /// </summary>
        public ActivityMapping ActivityMapping { get; set; }

        /// <summary>
        /// Help request property mapping (read-only)
        /// </summary>
        public PropertyMapping HelpRequestMapping { get; set; }

        /// <summary>
        /// Custom property mappings (property1-20, read-only)
        /// </summary>
        public List<CustomPropertyMapping> CustomPropertyMappings { get; set; }
    }
}
