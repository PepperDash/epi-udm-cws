using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PepperDash.Plugin.UdmCws;

namespace PepperDash.Plugin.UdmCws
{
    internal class MockState
    {
        public static State GetMockState()
        {
            return new State
            {
                Standard = new StandardProperties
                {
                    Version = "1.0.0",
                    State = "active",
                    Error = "No Error",
                    Occupancy = true,
                    HelpRequest = "none",
                    Activity = "meeting"
                },
                Status = new StatusProperties
                {
                    Devices = new Dictionary<string, DeviceStatus>
                    {
                        { DeviceKeys.device1.ToString(), new DeviceStatus
                            {
                                Label = "Main Display",
                                Status = "online",
                                Description = "75 inch LCD display",
                                VideoSource = "hdmi1",
                                AudioSource = "hdmi1",
                                Usage = 45,
                                Error = "No Error"
                            }
                        },
                        { DeviceKeys.device2.ToString(), new DeviceStatus
                            {
                                Label = "PTZ Camera",
                                Status = "online",
                                Description = "Pan-Tilt-Zoom Camera",
                                VideoSource = "usb1",
                                AudioSource = "N/A",
                                Usage = 120,
                                Error = "No Error"
                            }
                        },
                        { DeviceKeys.device3.ToString(), new DeviceStatus
                            {
                                Label = "Ceiling Microphone",
                                Status = "online",
                                Description = "Beamforming ceiling mic",
                                VideoSource = "N/A",
                                AudioSource = "dante1",
                                Usage = 89,
                                Error = "No Error"
                            }
                        },
                        { DeviceKeys.device4.ToString(), new DeviceStatus
                            {
                                Label = "Video Codec",
                                Status = "online",
                                Description = "Collaboration codec",
                                VideoSource = "internal",
                                AudioSource = "internal",
                                Usage = 230,
                                Error = "No Error"
                            }
                        }
                    }
                },
                Custom = new Dictionary<string, CustomProperties>
                {
                    { PropertyKeys.property1.ToString(), new CustomProperties
                        {
                            Label = "Room Name",
                            Value = "Conference Room A"
                        }
                    },
                    { PropertyKeys.property2.ToString(), new CustomProperties
                        {
                            Label = "Location",
                            Value = "Building 1, Floor 2"
                        }
                    },
                    { PropertyKeys.property3.ToString(), new CustomProperties
                        {
                            Label = "Capacity",
                            Value = "12"
                        }
                    },
                    { PropertyKeys.property4.ToString(), new CustomProperties
                        {
                            Label = "Equipment",
                            Value = "Video conferencing, whiteboard"
                        }
                    }
                }
            };
        }
    }
}
