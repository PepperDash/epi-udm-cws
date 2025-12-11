

using System.Collections.Generic;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials.Plugin.UdmCws
{
    public class UdmCwsHandler : IUdmApi
    {

        

        public void SetDeviceProperty(DeviceKeys key, DeviceStatus device)
        {

            var temp = PepperDash.Essentials.Core.DeviceManager.GetDeviceForKey(key);
            if (_roomResponse.Status.Devices == null)
            {
                _roomResponse.Status.Devices = new Dictionary<string, DeviceStatus>();
            }
            if(_roomResponse.Status.Devices.ContainsKey(key.ToString()))
            {
                _roomResponse.Status.Devices[key.ToString()] = device;
                return;
            }
            _roomResponse.Status.Devices.Add(key.ToString(), device);
        }

        /// <summary>
        /// Gets the current room response object
        /// </summary>
       
    }
}
