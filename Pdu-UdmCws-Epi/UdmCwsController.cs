
using PepperDash.Essentials.Core;


namespace Pdu_UdmCws_Epi
{
    public class UdmCwsController : EssentialsDevice
    {
        /// <summary>
        /// The UDM CWS handler instance that manages room response data
        /// </summary>
        public UdmCwsHandler Handler { get; private set; }

        public UdmCwsController(string key) : base(key)
        {
            Handler = new UdmCwsHandler();
            Handler.Initialize();
        }
    }
}