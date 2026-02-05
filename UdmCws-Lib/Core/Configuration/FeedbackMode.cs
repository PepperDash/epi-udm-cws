namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Feedback mode for PATCH responses
    /// </summary>
    public enum FeedbackMode
    {
        /// <summary>
        /// Deferred mode - Return 202 Accepted immediately, state applied asynchronously
        /// </summary>
        Deferred = 0,

        /// <summary>
        /// Immediate mode - Return 200 OK with updated state after applying changes
        /// </summary>
        Immediate = 1
    }

    /// <summary>
    /// Configuration for UDM-CWS behavior
    /// </summary>
    public class UdmCwsConfig
    {
        /// <summary>
        /// Feedback mode for PATCH requests
        /// </summary>
        public FeedbackMode FeedbackMode { get; set; }

        /// <summary>
        /// API version for version validation
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// Pre-shared key for request validation (empty or null = no PSK validation)
        /// </summary>
        public string Psk { get; set; }

        public UdmCwsConfig()
        {
            FeedbackMode = FeedbackMode.Deferred; // Default to deferred
            ApiVersion = "1.0.0";
            Psk = string.Empty;
        }
    }
}
