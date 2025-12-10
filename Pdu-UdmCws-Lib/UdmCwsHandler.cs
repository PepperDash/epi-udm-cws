

using System.Collections.Generic;

namespace PepperDash.Essentials.Core
{
    public class UdmCwsHandler : IUdmApi
    {
        private RoomResponse _roomResponse;

        public void Initialize()
        {
            _roomResponse = new RoomResponse();
        }

        public void SetDeviceProperty(DeviceKeys key, DeviceStatus device)
        {
            if(_roomResponse.Status.Devices == null)
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
        public RoomResponse GetRoomResponse() => _roomResponse;
    }
}
