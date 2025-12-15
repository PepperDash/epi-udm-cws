
namespace PepperDash.Plugin.UdmCws
{
    public interface IUdmApi
    {
        void SetDeviceProperty(DeviceKeys DeviceKey, DeviceStatus DeviceStatus );
    }

    public delegate State GetStateDelegate();

}
