using System;

namespace PepperDash.Plugin.UdmCws
{
    /// <summary>
    /// Event args for reporting current state (GET requests)
    /// </summary>
    public class ReportStateEventArgs : EventArgs
    {
        /// <summary>
        /// The state to be populated by event subscribers
        /// </summary>
        public State ReportedState { get; }

        public ReportStateEventArgs()
        {
            ReportedState = new State();
        }
    }

    /// <summary>
    /// Event args for handling desired state changes (PATCH requests)
    /// </summary>
    public class DesiredStateEventArgs : EventArgs
    {
        /// <summary>
        /// The desired state from the PATCH request
        /// </summary>
        public State DesiredState { get; }

        /// <summary>
        /// Indicates if the state change was handled successfully
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Error message if Success is false
        /// </summary>
        public string ErrorMessage { get; set; }

        public DesiredStateEventArgs(State desiredState)
        {
            DesiredState = desiredState;
        }
    }

    /// <summary>
    /// Delegate for ReportState events
    /// </summary>
    public delegate void ReportStateEventHandler(object sender, ReportStateEventArgs args);

    /// <summary>
    /// Delegate for DesiredState events
    /// </summary>
    public delegate void DesiredStateEventHandler(object sender, DesiredStateEventArgs args);
}
