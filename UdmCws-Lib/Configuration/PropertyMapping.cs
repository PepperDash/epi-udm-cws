using System.Collections.Generic;

namespace PepperDash.Plugin.UdmCws.Configuration
{
    /// <summary>
    /// Maps a device property or function to a UDM-CWS property
    /// Uses reflection at startup to build fast accessors
    /// </summary>
    public class PropertyMapping
    {
        /// <summary>
        /// Device key to read from
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// Property path for reading (e.g., "InCallFeedback.BoolValue")
        /// </summary>
        public string PropertyPath { get; set; }

        /// <summary>
        /// Optional: Map raw values to display values (e.g., "laptop" → "presentation")
        /// </summary>
        public Dictionary<string, string> ValueMap { get; set; }

        /// <summary>
        /// Optional: Format string for output (e.g., "{0}°F")
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Default value if property not found or null
        /// </summary>
        public string DefaultValue { get; set; }
    }

    /// <summary>
    /// Maps activity state with read property and write functions
    /// </summary>
    public class ActivityMapping
    {
        /// <summary>
        /// Property mapping for reading current activity
        /// </summary>
        public PropertyMapping GetProperty { get; set; }

        /// <summary>
        /// Function mappings for setting activity
        /// Key = activity value (e.g., "call", "present"), Value = function name to call
        /// </summary>
        public Dictionary<string, string> SetFunctions { get; set; }

        /// <summary>
        /// Device key for set functions (if different from get property)
        /// </summary>
        public string SetDeviceKey { get; set; }
    }

    /// <summary>
    /// Custom property mapping with label
    /// </summary>
    public class CustomPropertyMapping
    {
        /// <summary>
        /// Property key (property1-20)
        /// </summary>
        public string PropertyKey { get; set; }

        /// <summary>
        /// Display label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Property mapping
        /// </summary>
        public PropertyMapping Mapping { get; set; }
    }
}
