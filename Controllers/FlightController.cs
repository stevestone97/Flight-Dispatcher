using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using FlightDispatcherApp.Models;
using FlightDispatcherApp.Presentation;
using FlightDispatcherApp.Services;
using FlightDispatcherApp.Views;

namespace FlightDispatcherApp.Controllers
{
    /// <summary>
    /// Coordinates flight data and simulation actions between the view and dispatcher.
    /// </summary>
    public class FlightController : IDisposable
    {
        private IFlightView view;
        private FlightDispatcher dispatcher;
        private FlightEventLogger logger;
        private Timer timer;
        private bool disposed;

        /// <summary>
        /// Initializes the controller, connects event handlers, and displays the initial state.
        /// </summary>
        /// <param name="view">The view through which users interact with the dispatcher.</param>
        /// <param name="dispatcher">The model that manages flights and simulation events.</param>
        public FlightController(IFlightView view, FlightDispatcher dispatcher)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            // Subscribe a separate logger before subscribing this controller.
            logger = new FlightEventLogger(dispatcher);
            logger.MessageLogged += Logger_MessageLogged;
            
            timer = new Timer { Interval = 100 };
            timer.Tick += Timer_Tick;

            view.AddFlightRequested += View_AddFlightRequested;
            view.TakeOffRequested += View_TakeOffRequested;
            view.PrepareRequested += View_PrepareRequested;
            view.FlightSelectionChanged += View_FlightSelectionChanged;

            dispatcher.TakeOff += Dispatcher_TakeOff;
            dispatcher.Landed += Dispatcher_Landed;

            RefreshView();
        }

        /// <summary>
        /// Registers a flight using the current view inputs and selects the new flight.
        /// </summary>
        private void View_AddFlightRequested(object? sender, EventArgs e)
        {
            ExecuteSafely(delegate
            {
                Flight flight = dispatcher.RegisterFlight(
                    view.FlightNumberInput,
                    view.AirlineInput,
                    view.DestinationInput,
                    view.DurationInput);

                view.ClearFlightNumberInput();
                RefreshView();
                view.SelectFlight(flight.FlightNumber);
                UpdateActions();
            });
        }

        /// <summary>
        /// Starts the departure of the flight currently selected in the view.
        /// </summary>
        private void View_TakeOffRequested(object? sender, EventArgs e)
        {
            ExecuteSafely(delegate
            {
                dispatcher.StartDeparture(GetSelectedFlight());
            });
        }

        /// <summary>
        /// Prepares the selected landed flight for another departure.
        /// </summary>
        private void View_PrepareRequested(object? sender, EventArgs e)
        {
            ExecuteSafely(delegate
            {
                dispatcher.PrepareNextDeparture(
                    GetSelectedFlight(), view.DestinationInput, view.DurationInput);
                RefreshView();
            });
        }

        /// <summary>
        /// Updates available actions after the selected flight changes.
        /// </summary>
        private void View_FlightSelectionChanged(object? sender, EventArgs e)
        {
            UpdateActions();
        }

        /// <summary>
        /// Starts the simulation timer when a flight takes off and refreshes the view.
        /// </summary>
        private void Dispatcher_TakeOff(object? sender, FlightEventArgs e)
        {
            if (!timer.Enabled)
            {
                timer.Start();
            }
            RefreshView();
        }

        /// <summary>
        /// Stops the timer when all flights have landed and refreshes the view.
        /// </summary>
        private void Dispatcher_Landed(object? sender, FlightEventArgs e)
        {
            if (!dispatcher.HasFlightsInAir)
            {
                timer.Stop();
            }
            RefreshView();
        }

        /// <summary>
        /// Sends a logged flight event to the view.
        /// </summary>
        private void Logger_MessageLogged(string message)
        {
            view.AppendEvent(message);
        }

        /// <summary>
        /// Advances the flight simulation and refreshes countdowns on each timer tick.
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                dispatcher.AdvanceSimulation();
                RefreshView(); // refresh the countdown even between landing events

                if (!dispatcher.HasFlightsInAir)
                {
                    timer.Stop();
                }
            }
            catch (Exception ex)
            {
                timer.Stop();
                view.ShowError("Simulation error", ex.Message);
            }
        }

        /// <summary>
        /// Returns the selected registered flight or throws if the selection is invalid.
        /// </summary>
        /// <returns>The flight identified by the view's current selection.</returns>
        private Flight GetSelectedFlight()
        {
            string? selectedId = view.SelectedFlightNumber;
            if (selectedId == null)
            {
                throw new InvalidOperationException("Select a flight from the list first.");
            }

            foreach (Flight flight in dispatcher.Flights)
            {
                if (string.Equals(flight.FlightNumber, selectedId, StringComparison.OrdinalIgnoreCase))
                {
                    return flight;
                }
            }

            throw new InvalidOperationException("The selected flight was not found.");
        }

        /// <summary>
        /// Builds the flight rows and status summary, then updates the view.
        /// </summary>
        private void RefreshView()
        {
            List<FlightRow> rows = new List<FlightRow>();
            int ready = 0;
            int inAir = 0;
            int landed = 0;

            foreach (Flight flight in dispatcher.Flights)
            {
                string status;
                switch (flight.Status)
                {
                    case FlightStatus.Ready:
                        status = "Ready";
                        ready++;
                        break;
                    case FlightStatus.InAir:
                        status = "In air";
                        inAir++;
                        break;
                    case FlightStatus.Landed:
                        status = "Landed";
                        landed++;
                        break;
                    default:
                        status = "Unknown";
                        break;
                }

                rows.Add(new FlightRow(
                    flight.FlightNumber,
                    flight.Airline,
                    flight.Destination,
                    flight.DurationHours.ToString("0.0", CultureInfo.InvariantCulture),
                    flight.RemainingHours.ToString("0.0", CultureInfo.InvariantCulture),
                    status));
            }

            string summary = string.Format("{0} total   |   {1} ready   |   {2} in air   |   {3} landed",
                rows.Count, ready, inAir, landed);

            view.DisplayFlights(rows, summary);
            UpdateActions();
        }

        /// <summary>
        /// Enables view actions according to the selected flight's status.
        /// </summary>
        private void UpdateActions()
        {
            string? selectedId = view.SelectedFlightNumber;
            FlightStatus? status = null;

            foreach (Flight flight in dispatcher.Flights)
            {
                if (string.Equals(flight.FlightNumber, selectedId, StringComparison.OrdinalIgnoreCase))
                {
                    status = flight.Status;
                    break;
                }
            }

            view.SetActionAvailability(
                status == FlightStatus.Ready,
                status == FlightStatus.Landed);
        }

        /// <summary>
        /// Runs a view-triggered operation and presents any resulting error in the view.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        private void ExecuteSafely(Action operation)
        {
            try
            {
                operation();
            }
            catch (ArgumentException ex)
            {
                view.ShowError("Check flight details", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                view.ShowError("Action not allowed", ex.Message);
            }
            catch (Exception ex)
            {
                view.ShowError("Flight Dispatcher error", ex.Message);
            }
        }

        /// <summary>
        /// Stops and releases the timer and detaches all event handlers.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            timer.Stop();
            timer.Tick -= Timer_Tick;
            timer.Dispose();

            view.AddFlightRequested -= View_AddFlightRequested;
            view.TakeOffRequested -= View_TakeOffRequested;
            view.PrepareRequested -= View_PrepareRequested;
            view.FlightSelectionChanged -= View_FlightSelectionChanged;

            dispatcher.TakeOff -= Dispatcher_TakeOff;
            dispatcher.Landed -= Dispatcher_Landed;
            logger.MessageLogged -= Logger_MessageLogged;
            logger.Dispose();
            disposed = true;
        }
    }
}
