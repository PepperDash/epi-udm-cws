using PepperDash.Plugin.UdmCws.Epi.Reflection;
using System.Collections.Generic;

namespace PepperDash.Plugin.UdmCws.Epi.StateBuilder
{
    /// <summary>
    /// Populates configurable properties using reflection-based property mapping
    /// Handles: activity, helpRequest, custom properties (property1-20)
    /// Uses: ConfigurablePropertyHandler with PropertyAccessor for cached reflection
    /// </summary>
    public class ConfigurablePropertyPopulator
    {
        private readonly UdmCwsConfiguration _config;
        private readonly ConfigurablePropertyHandler _propertyHandler;

        public ConfigurablePropertyPopulator(UdmCwsConfiguration config, ConfigurablePropertyHandler propertyHandler)
        {
            _config = config;
            _propertyHandler = propertyHandler;
        }

        /// <summary>
        /// Populates activity from configured property mapping (read-only)
        /// </summary>
        public void PopulateActivity(StandardProperties standard)
        {
            if (_config.StandardProperties?.ActivityMapping?.GetProperty == null)
            {
                standard.Activity = "";
                return;
            }

            var activity = _propertyHandler.GetMappedValue(
                "activity_get",
                _config.StandardProperties.ActivityMapping.GetProperty
            );

            standard.Activity = activity ?? "";
        }

        /// <summary>
        /// Populates help request from configured property mapping (read-only)
        /// </summary>
        public void PopulateHelpRequest(StandardProperties standard)
        {
            if (_config.StandardProperties?.HelpRequestMapping == null)
            {
                standard.HelpRequest = "";
                return;
            }

            var helpRequest = _propertyHandler.GetMappedValue(
                "help_request",
                _config.StandardProperties.HelpRequestMapping
            );

            standard.HelpRequest = helpRequest ?? "";
        }

        /// <summary>
        /// Populates custom properties from configured property mappings (read-only)
        /// </summary>
        public void PopulateCustomProperties(Dictionary<string, CustomProperties> customProps)
        {
            if (_config.StandardProperties?.CustomPropertyMappings == null)
            {
                return;
            }

            foreach (var mapping in _config.StandardProperties.CustomPropertyMappings)
            {
                var cacheKey = $"custom_{mapping.PropertyKey}";
                var value = _propertyHandler.GetMappedValue(cacheKey, mapping.Mapping);

                customProps[mapping.PropertyKey] = new CustomProperties
                {
                    Label = mapping.Label ?? "",
                    Value = value ?? ""
                };
            }
        }

        /// <summary>
        /// Sets activity by invoking configured function (write operation)
        /// </summary>
        public void SetActivity(string desiredActivity)
        {
            if (_config.StandardProperties?.ActivityMapping == null)
            {
                return;
            }

            _propertyHandler.SetActivity(_config.StandardProperties.ActivityMapping, desiredActivity);
        }
    }
}
