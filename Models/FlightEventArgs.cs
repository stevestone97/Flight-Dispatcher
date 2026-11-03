using System;

namespace FlightDispatcherApp.Models
{
    /// <summary>
    /// Provides the flight and timestamp associated with a dispatcher event.
    /// </summary>
    public class FlightEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the flight associated with the event.
        /// </summary>
        public Flight Flight { get; }

        /// <summary>
        /// Gets the local date and time at which the event arguments were created.
        /// </summary>
        public DateTime EventTime { get; }

        /// <summary>
        /// Creates event arguments for the specified flight and records the current time.
        /// </summary>
        /// <param name="flight">The flight associated with the event.</param>
        public FlightEventArgs(Flight flight)
        {
            Flight = flight ?? throw new ArgumentNullException(nameof(flight));
            EventTime = DateTime.Now;
        }
    }
}
