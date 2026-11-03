using System;
using System.Diagnostics;

namespace FlightDispatcherApp.Models
{
    /// <summary>
    /// Represents a flight and tracks its destination, duration, and lifecycle status.
    /// </summary>
    public class Flight
    {
        private Stopwatch? flightClock;

        /// <summary>
        /// Gets the unique identifier for this flight.
        /// </summary>
        public string FlightNumber { get; }

        /// <summary>
        /// Gets the airline or aircraft associated with this flight.
        /// </summary>
        public string Airline { get; }

        /// <summary>
        /// Gets the flight's current destination.
        /// </summary>
        public string Destination { get; private set; }

        /// <summary>
        /// Gets the planned flight duration in hours.
        /// </summary>
        public double DurationHours { get; private set; }

        /// <summary>
        /// Gets the current lifecycle status of the flight.
        /// </summary>
        public FlightStatus Status { get; private set; }

        /// <summary>
        /// Gets the remaining flight time in hours, or the planned duration before departure.
        /// </summary>
        public double RemainingHours
        {
            get
            {
                if (Status == FlightStatus.Landed)
                {
                    return 0;
                }

                if (Status != FlightStatus.InAir || flightClock == null)
                {
                    return DurationHours;
                }

                return Math.Max(0, DurationHours - flightClock.Elapsed.TotalSeconds);
            }
        }

        /// <summary>
        /// Creates a flight after validating its identifier, airline, destination, and duration.
        /// </summary>
        /// <param name="flightNumber">The flight's unique identifier.</param>
        /// <param name="airline">The airline or aircraft name.</param>
        /// <param name="destination">The flight's destination.</param>
        /// <param name="durationHours">The planned duration, which must be greater than zero and at most 48 hours.</param>
        public Flight(string flightNumber, string airline, string destination, double durationHours)
        {
            FlightNumber = RequireText(flightNumber, "Flight ID");
            Airline = RequireText(airline, "Airline / aircraft");
            Destination = ValidateDestination(destination);
            DurationHours = ValidateDuration(durationHours);
            Status = FlightStatus.Ready;
        }

        /// <summary>
        /// Starts this flight if it is ready for departure.
        /// </summary>
        public void Depart()
        {
            if (Status != FlightStatus.Ready)
            {
                throw new InvalidOperationException(
                    "This flight is not ready for takeoff. An airborne flight must land first; " +
                    "a landed flight must be prepared with a new destination.");
            }
            flightClock = Stopwatch.StartNew();
            Status = FlightStatus.InAir;
        }

        /// <summary>
        /// Lands this flight if it is airborne and its remaining duration has elapsed.
        /// </summary>
        /// <returns><see langword="true"/> if the flight landed; otherwise, <see langword="false"/>.</returns>
        public bool TryLand()
        {
            if (Status != FlightStatus.InAir || RemainingHours > 0)
            {
                return false;
            }

            if (flightClock != null)
            {
                flightClock.Stop();
                flightClock = null;
            }

            Destination = "Home";
            Status = FlightStatus.Landed;
            return true;
        }

        /// <summary>
        /// Updates the destination and duration of a landed flight and makes it ready to depart.
        /// </summary>
        /// <param name="newDestination">The destination for the next departure.</param>
        /// <param name="newDurationHours">The duration for the next departure in hours.</param>
        public void PrepareNextDeparture(string newDestination, double newDurationHours)
        {
            if (Status != FlightStatus.Landed)
            {
                throw new InvalidOperationException(
                    "Only a flight that has already landed can be prepared for a new departure.");
            }

            string validatedDestination = ValidateDestination(newDestination);
            double validatedDuration = ValidateDuration(newDurationHours);
            Destination = validatedDestination;
            DurationHours = validatedDuration;
            Status = FlightStatus.Ready;
        }

        /// <summary>
        /// Trims and returns required text, or throws when the value is blank.
        /// </summary>
        /// <param name="value">The text to validate.</param>
        /// <param name="fieldName">The field name used in the validation error.</param>
        /// <returns>The trimmed text.</returns>
        private string RequireText(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(fieldName + " is required.");
            }

            return value.Trim();
        }

        /// <summary>
        /// Validates a destination and prevents using the reserved destination name "Home".
        /// </summary>
        /// <param name="destination">The destination to validate.</param>
        /// <returns>The validated destination.</returns>
        private string ValidateDestination(string? destination)
        {
            string result = RequireText(destination, "Destination");
            if (string.Equals(result, "Home", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Enter a destination other than Home.");
            }

            return result;
        }

        /// <summary>
        /// Validates that a flight duration is finite, positive, and no greater than 48 hours.
        /// </summary>
        /// <param name="hours">The duration to validate.</param>
        /// <returns>The validated duration.</returns>
        private double ValidateDuration(double hours)
        {
            if (double.IsNaN(hours) || double.IsInfinity(hours) || hours <= 0 || hours > 48)
            {
                throw new ArgumentOutOfRangeException(nameof(hours),
                    "Flight duration must be more than 0 and at most 48 hours.");
            }

            return hours;
        }
    }
}
