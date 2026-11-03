using System;
using System.Collections.Generic;
using FlightDispatcherApp.Presentation;

namespace FlightDispatcherApp.Views
{
    /// <summary>
    /// Defines the UI operations and events required by the flight controller.
    /// </summary>
    public interface IFlightView
    {
        /// <summary>
        /// Occurs when the user requests registration of a flight.
        /// </summary>
        event EventHandler? AddFlightRequested;

        /// <summary>
        /// Occurs when the user requests takeoff for the selected flight.
        /// </summary>
        event EventHandler? TakeOffRequested;

        /// <summary>
        /// Occurs when the user requests preparation of the selected flight.
        /// </summary>
        event EventHandler? PrepareRequested;

        /// <summary>
        /// Occurs when the selected flight changes.
        /// </summary>
        event EventHandler? FlightSelectionChanged;

        /// <summary>
        /// Gets the flight identifier entered by the user.
        /// </summary>
        string FlightNumberInput { get; }

        /// <summary>
        /// Gets the airline or aircraft name entered by the user.
        /// </summary>
        string AirlineInput { get; }

        /// <summary>
        /// Gets the destination entered by the user.
        /// </summary>
        string DestinationInput { get; }

        /// <summary>
        /// Gets the duration in hours entered by the user.
        /// </summary>
        double DurationInput { get; }

        /// <summary>
        /// Gets the identifier of the currently selected flight, if any.
        /// </summary>
        string? SelectedFlightNumber { get; }

        /// <summary>
        /// Displays the current flight rows and their summary.
        /// </summary>
        /// <param name="flights">The flight rows to display.</param>
        /// <param name="summary">A summary of flight counts.</param>
        void DisplayFlights(IReadOnlyList<FlightRow> flights, string summary);

        /// <summary>
        /// Appends a flight event message to the view.
        /// </summary>
        /// <param name="message">The message to append.</param>
        void AppendEvent(string message);

        /// <summary>
        /// Enables or disables flight actions according to the selection state.
        /// </summary>
        /// <param name="canTakeOff">Whether takeoff should be available.</param>
        /// <param name="canPrepare">Whether preparation should be available.</param>
        void SetActionAvailability(bool canTakeOff, bool canPrepare);

        /// <summary>
        /// Clears the flight identifier input.
        /// </summary>
        void ClearFlightNumberInput();

        /// <summary>
        /// Selects the flight with the specified identifier.
        /// </summary>
        /// <param name="flightNumber">The identifier of the flight to select.</param>
        void SelectFlight(string flightNumber);

        /// <summary>
        /// Presents an error message to the user.
        /// </summary>
        /// <param name="title">The error dialog title.</param>
        /// <param name="message">The error details to display.</param>
        void ShowError(string title, string message);
    }
}
