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
                    Error = "",
                    Occupancy = true,
                    HelpRequest = "none",
                    Activity = "meeting"
                },
                Status = new StatusProperties
                {
                    Devices = new Dictionary<string, DeviceStatus>
                    {
                        { "display", new DeviceStatus
                            {
                                Label = "Main Display",
                                Status = "online",
                                Description = "75 inch LCD display",
                                VideoSource = "hdmi1",
                                AudioSource = "hdmi1",
                                Usage = 45,
                                Error = ""
                            }
                        },
                        { "camera", new DeviceStatus
                            {
                                Label = "PTZ Camera",
                                Status = "online",
                                Description = "Pan-Tilt-Zoom Camera",
                                VideoSource = "usb1",
                                AudioSource = "",
                                Usage = 120,
                                Error = ""
                            }
                        },
                        { "microphone", new DeviceStatus
                            {
                                Label = "Ceiling Microphone",
                                Status = "online",
                                Description = "Beamforming ceiling mic",
                                VideoSource = "",
                                AudioSource = "dante1",
                                Usage = 89,
                                Error = ""
                            }
                        },
                        { "codec", new DeviceStatus
                            {
                                Label = "Video Codec",
                                Status = "online",
                                Description = "Collaboration codec",
                                VideoSource = "internal",
                                AudioSource = "internal",
                                Usage = 230,
                                Error = ""
                            }
                        }
                    }
                },
                Custom = new Dictionary<string, CustomProperties>
                {
                    { "roomName", new CustomProperties
                        {
                            Label = "Room Name",
                            Value = "Conference Room A"
                        }
                    },
                    { "location", new CustomProperties
                        {
                            Label = "Location",
                            Value = "Building 1, Floor 2"
                        }
                    },
                    { "capacity", new CustomProperties
                        {
                            Label = "Capacity",
                            Value = "12"
                        }
                    },
                    { "equipment", new CustomProperties
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
