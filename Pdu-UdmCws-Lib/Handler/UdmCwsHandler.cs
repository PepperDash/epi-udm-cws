using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsHandler : IUdmApi
    {
        private readonly IHasState _stateManager;

        public UdmCwsHandler(IHasState stateManager)
        {
            _stateManager = stateManager;
        }

        public void SetDeviceProperty(DeviceKeys key, DeviceStatus device)
        {
            var roomResponse = _stateManager.GetRoomResponse();

            if (roomResponse == null || roomResponse.Status == null)
                return;

            if (roomResponse.Status.Devices == null)
            {
                roomResponse.Status.Devices = new Dictionary<string, DeviceStatus>();
            }

            roomResponse.Status.Devices[key.ToString()] = device;
        }


    }
}
