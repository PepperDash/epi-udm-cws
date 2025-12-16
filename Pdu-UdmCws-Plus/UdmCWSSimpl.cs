

using Crestron.SimplSharp;

namespace PepperDash.Plugin.UdmCws
{
    public class UdmCWSController
    {
        private const int MAX_DEVICES = 20;
        private const int MAX_PROPERTIES = 20;

        // Singleton instance using Lazy<T> for thread-safe initialization
        private static readonly System.Lazy<UdmCWSController> _instance =
            new System.Lazy<UdmCWSController>(() => new UdmCWSController());

        /// <summary>
        /// Gets the singleton instance of UdmCWSController
        /// </summary>
        private static UdmCWSController Instance => _instance.Value;

        private State state = new State();
        private readonly object stateLock = new object();
        private readonly object initLock = new object();
        private UdmCwsServer cwsServer;
        private bool isInitialized = false;

        /// <summary>
        /// Public constructor for SIMPL+ compatibility
        /// Note: Use Instance property to access the singleton
        /// </summary>
        public UdmCWSController()
        {
            cwsServer = new UdmCwsServer();
        }

        /// <summary>
        /// Initialize and start the CWS server (static method for SIMPL+ compatibility)
        /// Must be called before the server can serve requests
        /// </summary>
        public static void Initialize()
        {
            Instance.InitializeInstance();
        }

        /// <summary>
        /// Stop the CWS server (static method for SIMPL+ compatibility)
        /// </summary>
        public static void Shutdown()
        {
            Instance.ShutdownInstance();
        }

        /// <summary>
        /// Initialize and start the CWS server (instance method)
        /// </summary>
        private void InitializeInstance()
        {
            lock (initLock)
            {
                if (isInitialized)
                {
                    CrestronConsole.PrintLine("UdmCWSController: Already initialized");
                    return;
                }

                cwsServer.Start(GetState);
                isInitialized = true;
                CrestronConsole.PrintLine("UdmCWSController: Initialized successfully");
            }
        }

        /// <summary>
        /// Stop the CWS server (instance method)
        /// </summary>
        private void ShutdownInstance()
        {
            lock (initLock)
            {
                if (!isInitialized)
                {
                    CrestronConsole.PrintLine("UdmCWSController: Server not initialized, nothing to shutdown");
                    return;
                }

                cwsServer.Stop();
                isInitialized = false;
                CrestronConsole.PrintLine("UdmCWSController: Shutdown complete");
            }
        }

        private State GetState()
        {
            lock (stateLock)
            {
                return state;
            }
        }

        // Static methods for SIMPL+ compatibility
        public static void SetDeviceLabel(ushort deviceNum, string label)
        {
            Instance.SetDeviceLabelInstance(deviceNum, label);
        }

        public static void SetDeviceStatus(ushort deviceNum, string status)
        {
            Instance.SetDeviceStatusInstance(deviceNum, status);
        }

        public static void SetDeviceDescription(ushort deviceNum, string description)
        {
            Instance.SetDeviceDescriptionInstance(deviceNum, description);
        }

        public static void SetDeviceVideoSource(ushort deviceNum, string videoSource)
        {
            Instance.SetDeviceVideoSourceInstance(deviceNum, videoSource);
        }

        public static void SetDeviceAudioSource(ushort deviceNum, string audioSource)
        {
            Instance.SetDeviceAudioSourceInstance(deviceNum, audioSource);
        }

        public static void SetDeviceUsage(ushort deviceNum, ushort usage)
        {
            Instance.SetDeviceUsageInstance(deviceNum, usage);
        }

        public static void SetDeviceError(ushort deviceNum, string error)
        {
            Instance.SetDeviceErrorInstance(deviceNum, error);
        }

        public static void SetPropertyLabel(ushort propertyNum, string label)
        {
            Instance.SetPropertyLabelInstance(propertyNum, label);
        }

        public static void SetPropertyValue(ushort propertyNum, string value)
        {
            Instance.SetPropertyValueInstance(propertyNum, value);
        }

        public static void SetStandardOccupancy(ushort occupancy)
        {
            Instance.SetStandardOccupancyInstance(occupancy);
        }

        public static void SetStandardError(string error)
        {
            Instance.SetStandardErrorInstance(error);
        }

        public static void SetStandardHelpRequest(string helpRequest)
        {
            Instance.SetStandardHelpRequestInstance(helpRequest);
        }

        public static void SetStandardVersion(string version)
        {
            Instance.SetStandardVersionInstance(version);
        }

        public static void SetStandardActivity(string activity)
        {
            Instance.SetStandardActivityInstance(activity);
        }

        public static void SetStandardState(string standardState)
        {
            Instance.SetStandardStateInstance(standardState);
        }

        // Instance methods (called by static wrappers)
        private void SetDeviceLabelInstance(ushort deviceNum, string label)
        {
            if (!ValidateDeviceNumber(deviceNum, "SetDeviceLabel"))
                return;

            lock (stateLock)
            {
                state.Status.Devices[$"device{deviceNum}"].Label = label ?? string.Empty;
            }
        }

        private void SetDeviceStatusInstance(ushort deviceNum, string status)
        {
            if (!ValidateDeviceNumber(deviceNum, "SetDeviceStatus"))
                return;

            lock (stateLock)
            {
                state.Status.Devices[$"device{deviceNum}"].Status = status ?? string.Empty;
            }
        }

        private void SetDeviceDescriptionInstance(ushort deviceNum, string description)
        {
            if (!ValidateDeviceNumber(deviceNum, "SetDeviceDescription"))
                return;

            lock (stateLock)
            {
                state.Status.Devices[$"device{deviceNum}"].Description = description ?? string.Empty;
            }
        }

        private void SetDeviceVideoSourceInstance(ushort deviceNum, string videoSource)
        {
            if (!ValidateDeviceNumber(deviceNum, "SetDeviceVideoSource"))
                return;

            lock (stateLock)
            {
                state.Status.Devices[$"device{deviceNum}"].VideoSource = videoSource ?? string.Empty;
            }
        }

        private void SetDeviceAudioSourceInstance(ushort deviceNum, string audioSource)
        {
            if (!ValidateDeviceNumber(deviceNum, "SetDeviceAudioSource"))
                return;

            lock (stateLock)
            {
                state.Status.Devices[$"device{deviceNum}"].AudioSource = audioSource ?? string.Empty;
            }
        }

        private void SetDeviceUsageInstance(ushort deviceNum, ushort usage)
        {
            if (!ValidateDeviceNumber(deviceNum, "SetDeviceUsage"))
                return;

            lock (stateLock)
            {
                state.Status.Devices[$"device{deviceNum}"].Usage = usage;
            }
        }

        private void SetDeviceErrorInstance(ushort deviceNum, string error)
        {
            if (!ValidateDeviceNumber(deviceNum, "SetDeviceError"))
                return;

            lock (stateLock)
            {
                state.Status.Devices[$"device{deviceNum}"].Error = error ?? string.Empty;
            }
        }

        private void SetPropertyLabelInstance(ushort propertyNum, string label)
        {
            if (!ValidatePropertyNumber(propertyNum, "SetPropertyLabel"))
                return;

            lock (stateLock)
            {
                state.Custom[$"property{propertyNum}"].Label = label ?? string.Empty;
            }
        }

        private void SetPropertyValueInstance(ushort propertyNum, string value)
        {
            if (!ValidatePropertyNumber(propertyNum, "SetPropertyValue"))
                return;

            lock (stateLock)
            {
                state.Custom[$"property{propertyNum}"].Value = value ?? string.Empty;
            }
        }

        private void SetStandardOccupancyInstance(ushort occupancy)
        {
            lock (stateLock)
            {
                state.Standard.Occupancy = occupancy == 1;
            }
        }

        private void SetStandardErrorInstance(string error)
        {
            lock (stateLock)
            {
                state.Standard.Error = error ?? string.Empty;
            }
        }

        private void SetStandardHelpRequestInstance(string helpRequest)
        {
            lock (stateLock)
            {
                state.Standard.HelpRequest = helpRequest ?? string.Empty;
            }
        }

        private void SetStandardVersionInstance(string version)
        {
            lock (stateLock)
            {
                state.Standard.Version = version ?? string.Empty;
            }
        }

        private void SetStandardActivityInstance(string activity)
        {
            lock (stateLock)
            {
                state.Standard.Activity = activity ?? string.Empty;
            }
        }

        private void SetStandardStateInstance(string standardState)
        {
            lock (stateLock)
            {
                state.Standard.State = standardState ?? string.Empty;
            }
        }

        /// <summary>
        /// Validates device number is within valid range
        /// </summary>
        private bool ValidateDeviceNumber(ushort deviceNum, string methodName)
        {
            if (deviceNum < 1 || deviceNum > MAX_DEVICES)
            {
                CrestronConsole.PrintLine("UdmCWSController.{0}: Invalid device number {1} (valid range: 1-{2})",
                    methodName, deviceNum, MAX_DEVICES);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates property number is within valid range
        /// </summary>
        private bool ValidatePropertyNumber(ushort propertyNum, string methodName)
        {
            if (propertyNum < 1 || propertyNum > MAX_PROPERTIES)
            {
                CrestronConsole.PrintLine("UdmCWSController.{0}: Invalid property number {1} (valid range: 1-{2})",
                    methodName, propertyNum, MAX_PROPERTIES);
                return false;
            }
            return true;
        }
    }
}
