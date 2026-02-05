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

        public UdmCwsConfig()
        {
            FeedbackMode = FeedbackMode.Deferred; // Default to deferred
        }
    }
}
