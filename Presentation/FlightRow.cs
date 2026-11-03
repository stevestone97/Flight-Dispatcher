namespace FlightDispatcherApp.Presentation
{
    /// <summary>
    /// Holds the display-ready values for one flight row in the view.
    /// </summary>
    public class FlightRow
    {
        /// <summary>
        /// Gets the flight identifier to display.
        /// </summary>
        public string FlightNumber { get; }

        /// <summary>
        /// Gets the airline or aircraft name to display.
        /// </summary>
        public string Airline { get; }

        /// <summary>
        /// Gets the destination to display.
        /// </summary>
        public string Destination { get; }

        /// <summary>
        /// Gets the formatted planned duration to display.
        /// </summary>
        public string DurationHours { get; }

        /// <summary>
        /// Gets the formatted remaining duration to display.
        /// </summary>
        public string RemainingHours { get; }

        /// <summary>
        /// Gets the status text to display.
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// Creates a display row from the supplied flight values.
        /// </summary>
        /// <param name="flightNumber">The flight identifier.</param>
        /// <param name="airline">The airline or aircraft name.</param>
        /// <param name="destination">The flight destination.</param>
        /// <param name="durationHours">The formatted planned duration.</param>
        /// <param name="remainingHours">The formatted remaining duration.</param>
        /// <param name="status">The formatted flight status.</param>
        public FlightRow(string flightNumber, string airline, string destination,
            string durationHours, string remainingHours, string status)
        {
            FlightNumber = flightNumber;
            Airline = airline;
            Destination = destination;
            DurationHours = durationHours;
            RemainingHours = remainingHours;
            Status = status;
        }
    }
}
