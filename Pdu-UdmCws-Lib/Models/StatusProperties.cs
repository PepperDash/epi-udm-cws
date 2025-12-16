using Newtonsoft.Json;
using System.Collections.Generic;

namespace PepperDash.Plugin.UdmCws
{
    public class StatusProperties
    {
        /// <summary>
        /// Dictionary of device statuses keyed by device identifier
        /// </summary>
        [JsonProperty("devices")]
        public Dictionary<string, DeviceStatus> Devices { get; set; }

        public StatusProperties()
        {
            Devices = new Dictionary<string, DeviceStatus>();

            // Pre-populate with device1-device20
            for (int i = 1; i <= 20; i++)
            {
                Devices[$"device{i}"] = new DeviceStatus();
            }
        }
    }
}