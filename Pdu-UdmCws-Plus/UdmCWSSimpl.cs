

using Crestron.SimplSharp;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCWSController
    {

        private State state = new State();

        private State GetState()
        {
            return state;
        }
        public void SetDeviceLabel(ushort deviceNum, string label)
        {
            state.Status.Devices[$"device{deviceNum}"].Label = label;
        }

        public void SetDeviceStatus(ushort deviceNum, string status)
        {
            state.Status.Devices[$"device{deviceNum}"].Status = status;
        }

        public void SetDeviceDescription(ushort deviceNum, string description)
        {
            state.Status.Devices[$"device{deviceNum}"].Description = description;
        }

        public void SetDeviceVideoSource(ushort deviceNum, string videoSource)
        {
            state.Status.Devices[$"device{deviceNum}"].VideoSource = videoSource;
        }

        public void SetDeviceAudioSource(ushort deviceNum, string audioSource)
        {
            state.Status.Devices[$"device{deviceNum}"].AudioSource = audioSource;
        }

        public void SetDeviceUsage(ushort deviceNum, ushort usage)
        {
            state.Status.Devices[$"device{deviceNum}"].Usage = usage;
        }

        public void SetDeviceError(ushort deviceNum, string error)
        {
            state.Status.Devices[$"device{deviceNum}"].Error = error;
        }

        public void SetPropertyLabel(ushort propertyNum, string label)
        {
            state.Custom[$"property{propertyNum}"].Label = label;
        }

        public void SetPropertyValue(ushort propertyNum, string value)
        {
            state.Custom[$"property{propertyNum}"].Value = value;
        }

        public void SetStandardOccupancy(ushort occupancy)
        {
            state.Standard.Occupancy = occupancy == 1;
        }

        public void SetStandardError(string error)
        {
            state.Standard.Error = error;
        }

        public void SetStandardHelpRequest(string helpRequest) 
        {
            state.Standard.HelpRequest = helpRequest;
        }

        public void SetStandardVersion(string version)
        {
            state.Standard.Version = version;
        }

        public void SetStandardActivity(string activity)
        {
            state.Standard.Activity = activity;
        }

        public void SetStandardState(string standardState)
        {
            state.Standard.State = standardState;
            CrestronConsole.PrintLine("Standard State set to: " + standardState);
        }

    }
}
