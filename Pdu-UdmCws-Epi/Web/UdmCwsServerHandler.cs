using Crestron.SimplSharp;
using PepperDash.Core.Web;
using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCwsServerHandler : EssentialsDevice
    {
        public UdmCwsServerHandler(string key, string name) : base(key, name)
        {
        }

        private readonly WebApiServer _server;
        private readonly string _defaultBasePath = CrestronEnvironment.DevicePlatform == eDevicePlatform.Appliance
            ? string.Format("/app{0:00}/api", InitialParametersClass.ApplicationNumber)
            : "/api";

        private const int DebugTrace = 0;
        private const int DebugInfo = 1;
        private const int DebugVerbose = 2;

        public bool IsRegistered
        {
            get { return _server.IsRegistered; }
        }




    }
}
