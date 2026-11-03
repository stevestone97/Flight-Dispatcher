using System;
using System.Windows.Forms;
using FlightDispatcherApp.Controllers;
using FlightDispatcherApp.Models;
using FlightDispatcherApp.Views;

namespace FlightDispatcherApp
{
    /// <summary>
    /// Defines the application entry point and initializes the main window.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Starts the Windows Forms application and wires up its view, model, and controller.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Composition root: create the View, Model, and Controller separately.
            using (MainForm view = new MainForm())
            using (FlightController controller = new FlightController(view, new FlightDispatcher()))
            {
                Application.Run(view);
            }
        }
    }
}
