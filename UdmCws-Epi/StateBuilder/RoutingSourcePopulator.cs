using PepperDash.Essentials.Core;

namespace PepperDash.Plugin.UdmCws.Epi.StateBuilder
{
    /// <summary>
    /// Populates routing source information for devices
    /// Handles: videoSource, audioSource
    /// Uses: IRoutingSink → CurrentSourceInfo
    /// </summary>
    public class RoutingSourcePopulator
    {
        /// <summary>
        /// Populates video and audio source information from routing interfaces
        /// </summary>
        public void PopulateRoutingSources(object device, DeviceStatus status)
        {
            // Check if device is a routing sink with current source info
            if (device is IRoutingSink routingSink)
            {
                var sourceInfo = routingSink.CurrentSourceInfo;
                if (sourceInfo != null)
                {
                    // Use source name for both video and audio
                    // Most devices route both together
                    status.VideoSource = sourceInfo.Name ?? "";
                    status.AudioSource = sourceInfo.Name ?? "";
                }
                else
                {
                    status.VideoSource = null;
                    status.AudioSource = null;
                }
            }
            else
            {
                // Device doesn't support routing feedback
                status.VideoSource = null;
                status.AudioSource = null;
            }
        }
    }
}
