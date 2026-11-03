using System;
using System.Collections.Generic;
using FlightDispatcherApp.Models;

namespace FlightDispatcherApp.Services
{
    /// <summary>
    /// Subscribes to dispatcher events, stores formatted messages, and notifies listeners.
    /// </summary>
    public class FlightEventLogger : IDisposable
    {
        private FlightDispatcher dispatcher;
        private List<string> entries = new List<string>();
        private bool disposed;

        /// <summary>
        /// Occurs when a formatted flight event message is recorded.
        /// </summary>
        public event Action<string>? MessageLogged;

        /// <summary>
        /// Gets the recorded messages as a read-only list.
        /// </summary>
        public IReadOnlyList<string> Entries => entries.AsReadOnly();

        /// <summary>
        /// Creates a logger and subscribes it to the dispatcher's takeoff and landing events.
        /// </summary>
        /// <param name="dispatcher">The dispatcher whose events should be logged.</param>
        public FlightEventLogger(FlightDispatcher dispatcher)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            dispatcher.TakeOff += Dispatcher_TakeOff;
            dispatcher.Landed += Dispatcher_Landed;
        }

        /// <summary>
        /// Records a formatted message when a flight takes off.
        /// </summary>
        private void Dispatcher_TakeOff(object? sender, FlightEventArgs e)
        {
            Write(string.Format("[{0:HH:mm:ss}] TAKEOFF | {1} ({2}) departed for {3} ({4:0.0} h).",
                e.EventTime, e.Flight.FlightNumber, e.Flight.Airline,
                e.Flight.Destination, e.Flight.DurationHours));
        }

        /// <summary>
        /// Records a formatted message when a flight lands.
        /// </summary>
        private void Dispatcher_Landed(object? sender, FlightEventArgs e)
        {
            Write(string.Format("[{0:HH:mm:ss}] LANDED  | {1} ({2}) landed. Destination: {3}.",
                e.EventTime, e.Flight.FlightNumber, e.Flight.Airline,
                e.Flight.Destination));
        }

        /// <summary>
        /// Stores a message and notifies subscribers that it was logged.
        /// </summary>
        /// <param name="message">The message to record and publish.</param>
        private void Write(string message)
        {
            entries.Add(message);
            MessageLogged?.Invoke(message);
        }

        /// <summary>
        /// Detaches from dispatcher events and clears message subscribers.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            dispatcher.TakeOff -= Dispatcher_TakeOff;
            dispatcher.Landed -= Dispatcher_Landed;
            MessageLogged = null;
            disposed = true;
        }
    }
}
