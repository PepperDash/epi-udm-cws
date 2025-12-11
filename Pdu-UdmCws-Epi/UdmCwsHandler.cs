using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsHandler : EssentialsDevice, IUdmApi
    {

        public RoomResponse GetRoomResponse()
        {       
            var _stateManager = DeviceManager.AllDevices
                .OfType<UdmCwsStateManager>()
                .FirstOrDefault();

            if (_stateManager == null)
                return null;
            return _stateManager.GetState();
        }
        public void SetDeviceProperty(DeviceKeys key, DeviceStatus device)
        {
            var roomResponse = GetRoomResponse();

            if (roomResponse == null || roomResponse.Status == null)
                return;

            if (roomResponse.Status.Devices == null)
            {
                roomResponse.Status.Devices = new Dictionary<string, DeviceStatus>();
            }

            roomResponse.Status.Devices[key.ToString()] = device;
        }

        public void 
    }
}
