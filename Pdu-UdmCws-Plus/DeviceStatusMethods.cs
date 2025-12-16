using PepperDash.Plugin.UdmCws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdu_UdmCws_Plus
{
    public class DeviceStatusMethods
    {
        private DeviceStatus deviceStatus = new DeviceStatus();

        public void SetLabel(string label)
        {
            deviceStatus.Label = label;
        }

        public string GetLabel()
        {
            return deviceStatus.Label;
        }

        public void SetStatus(string status)
        {
            deviceStatus.Status = status;
        }
        public string GetStatus()
        {
            return deviceStatus.Status;
        }
        public void SetDescription(string description)
        {
            deviceStatus.Description = description;
        }
        public string GetDescription()
        {
            return deviceStatus.Description;
        }
        public void SetVideoSource(string videoSource)
        {
            deviceStatus.VideoSource = videoSource;
        }
        public string GetVideoSource()
        {
            return deviceStatus.VideoSource;
        }
        public void SetAudioSource(string audioSource)
        {
            deviceStatus.AudioSource = audioSource;
        }
        public string GetAudioSource()
        {
            return deviceStatus.AudioSource; 
        }
        public void SetUsage(ushort usage)
        {
            deviceStatus.Usage = (int)usage;
        }
        public ushort GetUsage()
        {
            return (ushort)deviceStatus.Usage;
        }
        public void SetError(string error)
        {
            deviceStatus.Error = error;
        }
        public string GetError()
        {
            return deviceStatus.Error;
        }
    }
}
