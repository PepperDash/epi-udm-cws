using System;
using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Plugin.UdmCws.Configuration;
using Serilog.Events;

namespace PepperDash.Plugin.UdmCws.Epi.Reflection
{
    /// <summary>
    /// Handles configurable property mappings with value transformation
    /// </summary>
    public class ConfigurablePropertyHandler
    {
        private readonly PropertyAccessor _accessor;

        public ConfigurablePropertyHandler(PropertyAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// Initializes all property mappings at startup
        /// </summary>
        public void Initialize(UdmCwsConfiguration config)
        {
            Debug.LogMessage(LogEventLevel.Information, "ConfigurablePropertyHandler: Initializing property mappings");

            // Initialize activity get property
            if (config.StandardProperties?.ActivityMapping?.GetProperty != null)
            {
                InitializePropertyMapping("activity_get", config.StandardProperties.ActivityMapping.GetProperty);
            }

            // Initialize activity set functions
            if (config.StandardProperties?.ActivityMapping?.SetFunctions != null)
            {
                var setDeviceKey = config.StandardProperties.ActivityMapping.SetDeviceKey
                    ?? config.StandardProperties.ActivityMapping.GetProperty?.DeviceKey;

                if (!string.IsNullOrEmpty(setDeviceKey))
                {
                    var device = DeviceManager.GetDeviceForKey(setDeviceKey);
                    if (device != null)
                    {
                        foreach (var kvp in config.StandardProperties.ActivityMapping.SetFunctions)
                        {
                            var cacheKey = $"activity_set_{kvp.Key}";
                            _accessor.CacheMethod(cacheKey, device, kvp.Value);
                        }
                    }
                }
            }

            // Initialize help request
            if (config.StandardProperties?.HelpRequestMapping != null)
            {
                InitializePropertyMapping("help_request", config.StandardProperties.HelpRequestMapping);
            }

            // Initialize custom properties
            if (config.StandardProperties?.CustomPropertyMappings != null)
            {
                foreach (var customProp in config.StandardProperties.CustomPropertyMappings)
                {
                    var cacheKey = $"custom_{customProp.PropertyKey}";
                    InitializePropertyMapping(cacheKey, customProp.Mapping);
                }
            }

            Debug.LogMessage(LogEventLevel.Information, "ConfigurablePropertyHandler: Initialization complete");
        }

        /// <summary>
        /// Initializes a single property mapping
        /// </summary>
        private void InitializePropertyMapping(string cacheKey, PropertyMapping mapping)
        {
            if (mapping == null || string.IsNullOrEmpty(mapping.DeviceKey))
                return;

            var device = DeviceManager.GetDeviceForKey(mapping.DeviceKey);
            if (device == null)
            {
                Debug.LogMessage(LogEventLevel.Warning,
                    "ConfigurablePropertyHandler: Device not found for mapping {Key}: {DeviceKey}",
                    cacheKey, mapping.DeviceKey);
                return;
            }

            _accessor.CompilePropertyGetter(cacheKey, device, mapping.PropertyPath);
        }

        /// <summary>
        /// Gets a mapped property value with transformations
        /// Uses lazy compilation - compiles property getter on first access
        /// </summary>
        public string GetMappedValue(string cacheKey, PropertyMapping mapping)
        {
            if (mapping == null)
                return null;

            var device = DeviceManager.GetDeviceForKey(mapping.DeviceKey);
            if (device == null)
                return mapping.DefaultValue;

            // Lazy compilation - compile property getter on first access if not already cached
            var rawValue = _accessor.GetPropertyValue(cacheKey, device);
            if (rawValue == null)
            {
                // Try lazy compilation - device may not have existed during initialization
                _accessor.CompilePropertyGetter(cacheKey, device, mapping.PropertyPath);
                rawValue = _accessor.GetPropertyValue(cacheKey, device);

                if (rawValue == null)
                    return mapping.DefaultValue;
            }

            // Apply value map if specified
            var stringValue = rawValue.ToString();
            if (mapping.ValueMap != null && mapping.ValueMap.ContainsKey(stringValue))
            {
                stringValue = mapping.ValueMap[stringValue];
            }

            // Apply format if specified
            if (!string.IsNullOrEmpty(mapping.Format))
            {
                try
                {
                    stringValue = string.Format(mapping.Format, stringValue);
                }
                catch (Exception ex)
                {
                    Debug.LogMessage(LogEventLevel.Warning,
                        "ConfigurablePropertyHandler: Format error for {Key}: {Message}", cacheKey, ex.Message);
                }
            }

            return stringValue;
        }

        /// <summary>
        /// Sets activity by invoking the mapped function
        /// </summary>
        public void SetActivity(ActivityMapping mapping, string desiredActivity)
        {
            if (mapping?.SetFunctions == null || !mapping.SetFunctions.ContainsKey(desiredActivity))
            {
                Debug.LogMessage(LogEventLevel.Warning,
                    "ConfigurablePropertyHandler: No set function configured for activity: {Activity}", desiredActivity);
                return;
            }

            var setDeviceKey = mapping.SetDeviceKey ?? mapping.GetProperty?.DeviceKey;
            if (string.IsNullOrEmpty(setDeviceKey))
            {
                Debug.LogMessage(LogEventLevel.Error, "ConfigurablePropertyHandler: No device key for activity set");
                return;
            }

            var device = DeviceManager.GetDeviceForKey(setDeviceKey);
            if (device == null)
            {
                Debug.LogMessage(LogEventLevel.Error,
                    "ConfigurablePropertyHandler: Device not found for activity set: {DeviceKey}", setDeviceKey);
                return;
            }

            var cacheKey = $"activity_set_{desiredActivity}";
            _accessor.InvokeMethod(cacheKey, device);
        }
    }
}
