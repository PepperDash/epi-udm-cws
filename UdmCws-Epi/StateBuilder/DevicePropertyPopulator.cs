using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Devices.Common.AudioCodec;
using PepperDash.Essentials.Devices.Common.VideoCodec;
using System;

namespace PepperDash.Plugin.UdmCws.Epi.StateBuilder
{
    /// <summary>
    /// Populates device-level properties using Essentials interfaces
    /// Handles: label, status, description, usage, error
    /// </summary>
    public class DevicePropertyPopulator
    {
        /// <summary>
        /// Populates all auto-detected device properties
        /// </summary>
        public void PopulateDeviceProperties(object device, DeviceStatus status, DeviceMapping mapping)
        {
            PopulateLabel(device, status, mapping);
            PopulateDescription(status, mapping);
            PopulatePowerStatus(device, status);
            PopulateUsage(device, status);
            PopulateCommunicationError(device, status);
        }

        #region Label and Description (Config-Driven)

        /// <summary>
        /// Populates device label from config or device key
        /// </summary>
        private void PopulateLabel(object device, DeviceStatus status, DeviceMapping mapping)
        {
            // Use custom label if provided, otherwise use device key
            status.Label = !string.IsNullOrEmpty(mapping.CustomLabel)
                ? mapping.CustomLabel
                : (device as IKeyName)?.Key ?? "Unknown";
        }

        /// <summary>
        /// Populates device description from config (config-only, no interface)
        /// </summary>
        private void PopulateDescription(DeviceStatus status, DeviceMapping mapping)
        {
            status.Description = mapping.Description ?? "";
        }

        #endregion

        #region Power Status (IHasPowerControlWithFeedback, IWarmingCooling)

        /// <summary>
        /// Populates device power status from power and warming/cooling interfaces
        /// Priority: Warming/Cooling states override on/off states
        /// </summary>
        private void PopulatePowerStatus(object device, DeviceStatus status)
        {
            // Default status
            status.Status = "Off";

            // Check basic power control
            if (device is IHasPowerControlWithFeedback powerDevice)
            {
                status.Status = powerDevice.PowerIsOnFeedback?.BoolValue == true ? "On" : "Off";
            }

            // Check warming/cooling (overrides on/off)
            if (device is IWarmingCooling warmingDevice)
            {
                if (warmingDevice.IsWarmingUpFeedback?.BoolValue == true)
                {
                    status.Status = "Warming";
                }
                else if (warmingDevice.IsCoolingDownFeedback?.BoolValue == true)
                {
                    status.Status = "Cooling";
                }
            }

            // Check in-call status for video codecs
            if (device is VideoCodecBase videoCodec)
            {
                // Video codecs in standby are "on" - if online/communicating, treat as on
                if (device is ICommunicationMonitor commMonitor && commMonitor.CommunicationMonitor?.IsOnline == true)
                {
                    if (status.Status == "Off")
                    {
                        status.Status = "On"; // Standby mode - codec is on but displays off
                    }
                }

                if (videoCodec.IsInCall)
                {
                    status.Status = "In Call";
                }
            }
            // Check in-call status for audio codecs
            else if (device is AudioCodecBase audioCodec)
            {
                // Audio codecs in standby are "on" - if online/communicating, treat as on
                if (device is ICommunicationMonitor commMonitor && commMonitor.CommunicationMonitor?.IsOnline == true)
                {
                    if (status.Status == "Off")
                    {
                        status.Status = "On"; // Standby mode - codec is on
                    }
                }

                if (audioCodec.IsInCall)
                {
                    status.Status = "In Call";
                }
            }
        }

        #endregion

        #region Usage Tracking (IUsageTracking, IDisplayUsage)

        /// <summary>
        /// Populates device usage from usage tracking interfaces
        /// Priority: IUsageTracking (session minutes) → IDisplayUsage (lamp hours) → null
        /// </summary>
        private void PopulateUsage(object device, DeviceStatus status)
        {
            // Priority 1: IUsageTracking (current session minutes)
            // Tracks active usage session - resets each time device goes in/out of use
            if (device is IUsageTracking usageDevice && usageDevice.UsageTracker != null)
            {
                if (usageDevice.UsageTracker.UsageTrackingStarted)
                {
                    var elapsed = DateTime.Now - usageDevice.UsageTracker.UsageStartTime;
                    status.Usage = (int)elapsed.TotalMinutes;
                }
                else
                {
                    status.Usage = null; // Not currently in use
                }
                return;
            }

            // Priority 2: IDisplayUsage (total lamp hours)
            // Provides cumulative device runtime across all sessions
            if (device is IDisplayUsage displayDevice)
            {
                status.Usage = displayDevice.LampHours?.IntValue;
                return;
            }

            // Default: No usage tracking available
            status.Usage = null;
        }

        #endregion

        #region Communication Monitoring (ICommunicationMonitor, IOnline)

        /// <summary>
        /// Populates device communication error status
        /// Checks ICommunicationMonitor and IOnline interfaces
        /// </summary>
        private void PopulateCommunicationError(object device, DeviceStatus status)
        {
            // Default: no error
            status.Error = null;

            // Check ICommunicationMonitor
            if (device is ICommunicationMonitor commDevice)
            {
                if (commDevice.CommunicationMonitor?.Message != "")
                {
                    status.Error = commDevice.CommunicationMonitor.Message;
                }
                return;
            }

            // Fallback: Check IOnline
            if (device is IOnline onlineDevice)
            {
                if (onlineDevice.IsOnline?.BoolValue == false)
                {
                    status.Error = "Device Offline";
                }
            }
        }

        #endregion
    }
}
