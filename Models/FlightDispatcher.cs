using System;
using System.Collections.Generic;

namespace FlightDispatcherApp.Models
{
    /// <summary>
    /// Maintains registered flights and raises events as their simulation state changes.
    /// </summary>
    public class FlightDispatcher
    {
        private readonly List<Flight> flights = new List<Flight>();

        /// <summary>
        /// Occurs when a registered flight begins its departure.
        /// </summary>
        public event EventHandler<FlightEventArgs>? TakeOff;

        /// <summary>
        /// Occurs when a registered flight lands.
        /// </summary>
        public event EventHandler<FlightEventArgs>? Landed;

        /// <summary>
        /// Gets the registered flights as a read-only list.
        /// </summary>
        public IReadOnlyList<Flight> Flights => flights.AsReadOnly();

        /// <summary>
        /// Gets whether any registered flight is currently airborne.
        /// </summary>
        public bool HasFlightsInAir
        {
            get
            {
                foreach (Flight flight in flights)
                {
                    if (flight.Status == FlightStatus.InAir)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Creates and registers a flight, rejecting identifiers that are already in use.
        /// </summary>
        /// <param name="flightNumber">The unique identifier for the flight.</param>
        /// <param name="airline">The airline or aircraft name.</param>
        /// <param name="destination">The flight's destination.</param>
        /// <param name="durationHours">The planned flight duration in hours.</param>
        /// <returns>The newly registered flight.</returns>
        public Flight RegisterFlight(string flightNumber, string airline,
            string destination, double durationHours)
        {
            foreach (Flight existingFlight in flights)
            {
                if (string.Equals(existingFlight.FlightNumber, flightNumber?.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("A flight with this ID is already registered.");
                }
            }

            Flight flight = new Flight(flightNumber, airline, destination, durationHours);
            flights.Add(flight);
            return flight;
        }

        /// <summary>
        /// Starts the departure of a registered flight and raises the takeoff event.
        /// </summary>
        /// <param name="flight">The registered flight to depart.</param>
        public void StartDeparture(Flight flight)
        {
            CheckRegisteredFlight(flight);
            flight.Depart();
            TakeOff?.Invoke(this, new FlightEventArgs(flight));
        }

        /// <summary>
        /// Prepares a registered landed flight for another departure.
        /// </summary>
        /// <param name="flight">The registered flight to prepare.</param>
        /// <param name="destination">The destination for its next departure.</param>
        /// <param name="durationHours">The duration of its next departure in hours.</param>
        public void PrepareNextDeparture(Flight flight, string destination, double durationHours)
        {
            CheckRegisteredFlight(flight);
            flight.PrepareNextDeparture(destination, durationHours);
        }

        /// <summary>
        /// Checks airborne flights for completion and raises a landing event for each that lands.
        /// </summary>
        public void AdvanceSimulation()
        {
            foreach (Flight flight in flights)
            {
                if (flight.Status == FlightStatus.InAir && flight.TryLand())
                {
                    Landed?.Invoke(this, new FlightEventArgs(flight));
                }
            }
        }

        /// <summary>
        /// Ensures a flight is non-null and belongs to this dispatcher.
        /// </summary>
        /// <param name="flight">The flight to validate.</param>
        private void CheckRegisteredFlight(Flight? flight)
        {
            if (flight == null || !flights.Contains(flight))
            {
                throw new ArgumentException("Select a registered flight first.");
            }
        }
    }
}
