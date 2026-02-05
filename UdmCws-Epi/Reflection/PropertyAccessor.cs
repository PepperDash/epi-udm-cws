using System;
using System.Collections.Generic;
using System.Reflection;
using PepperDash.Core;
using Serilog.Events;

namespace PepperDash.Plugin.UdmCws.Epi.Reflection
{
    /// <summary>
    /// Manages property accessors and method delegates using reflection
    /// Reflects once at startup, caches delegates for fast access
    /// </summary>
    public class PropertyAccessor
    {
        private readonly Dictionary<string, Func<object, object>> _propertyGetters;
        private readonly Dictionary<string, MethodInfo> _methods;

        public PropertyAccessor()
        {
            _propertyGetters = new Dictionary<string, Func<object, object>>();
            _methods = new Dictionary<string, MethodInfo>();
        }

        /// <summary>
        /// Compiles a property getter at startup for fast access
        /// </summary>
        /// <param name="cacheKey">Unique key for this accessor</param>
        /// <param name="device">Device instance to reflect on</param>
        /// <param name="propertyPath">Property path (e.g., "InCallFeedback.BoolValue")</param>
        public void CompilePropertyGetter(string cacheKey, object device, string propertyPath)
        {
            if (device == null || string.IsNullOrEmpty(propertyPath))
            {
                Debug.LogMessage(LogEventLevel.Warning, "PropertyAccessor: Cannot compile null device or empty path for key {Key}", cacheKey);
                return;
            }

            try
            {
                // Build a simple reflection-based getter
                // For .NET Framework 4.7.2, we use simple reflection rather than expression trees
                var getter = BuildPropertyGetter(device.GetType(), propertyPath);
                _propertyGetters[cacheKey] = getter;

                Debug.LogMessage(LogEventLevel.Debug, "PropertyAccessor: Compiled getter for {Key} -> {Path}", cacheKey, propertyPath);
            }
            catch (Exception ex)
            {
                Debug.LogMessage(LogEventLevel.Error, "PropertyAccessor: Failed to compile getter for {Key}: {Message}", cacheKey, ex.Message);
            }
        }

        /// <summary>
        /// Caches a method for fast invocation
        /// </summary>
        /// <param name="cacheKey">Unique key for this method</param>
        /// <param name="device">Device instance to reflect on</param>
        /// <param name="methodName">Method name to call</param>
        public void CacheMethod(string cacheKey, object device, string methodName)
        {
            if (device == null || string.IsNullOrEmpty(methodName))
            {
                Debug.LogMessage(LogEventLevel.Warning, "PropertyAccessor: Cannot cache null device or empty method for key {Key}", cacheKey);
                return;
            }

            try
            {
                var method = device.GetType().GetMethod(methodName);
                if (method == null)
                {
                    Debug.LogMessage(LogEventLevel.Warning, "PropertyAccessor: Method {Method} not found on {Type}", methodName, device.GetType().Name);
                    return;
                }

                _methods[cacheKey] = method;
                Debug.LogMessage(LogEventLevel.Debug, "PropertyAccessor: Cached method {Key} -> {Method}", cacheKey, methodName);
            }
            catch (Exception ex)
            {
                Debug.LogMessage(LogEventLevel.Error, "PropertyAccessor: Failed to cache method for {Key}: {Message}", cacheKey, ex.Message);
            }
        }

        /// <summary>
        /// Gets a property value using cached accessor
        /// </summary>
        /// <param name="cacheKey">Cache key for the accessor</param>
        /// <param name="device">Device instance</param>
        /// <returns>Property value or null if not found</returns>
        public object GetPropertyValue(string cacheKey, object device)
        {
            if (!_propertyGetters.TryGetValue(cacheKey, out var getter))
            {
                return null;
            }

            try
            {
                return getter(device);
            }
            catch (Exception ex)
            {
                Debug.LogMessage(LogEventLevel.Warning, "PropertyAccessor: Error getting property for {Key}: {Message}", cacheKey, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Invokes a cached method
        /// </summary>
        /// <param name="cacheKey">Cache key for the method</param>
        /// <param name="device">Device instance</param>
        /// <param name="parameters">Method parameters (optional)</param>
        public void InvokeMethod(string cacheKey, object device, params object[] parameters)
        {
            if (!_methods.TryGetValue(cacheKey, out var method))
            {
                Debug.LogMessage(LogEventLevel.Warning, "PropertyAccessor: Method not found for key {Key}", cacheKey);
                return;
            }

            try
            {
                method.Invoke(device, parameters);
                Debug.LogMessage(LogEventLevel.Debug, "PropertyAccessor: Invoked method {Key}", cacheKey);
            }
            catch (Exception ex)
            {
                Debug.LogMessage(LogEventLevel.Error, "PropertyAccessor: Error invoking method for {Key}: {Message}", cacheKey, ex.Message);
            }
        }

        /// <summary>
        /// Builds a property getter function using reflection
        /// </summary>
        private Func<object, object> BuildPropertyGetter(Type deviceType, string propertyPath)
        {
            var parts = propertyPath.Split('.');

            return (device) =>
            {
                object current = device;
                foreach (var part in parts)
                {
                    if (current == null)
                        return null;

                    var property = current.GetType().GetProperty(part);
                    if (property == null)
                        return null;

                    current = property.GetValue(current, null);
                }
                return current;
            };
        }
    }
}
