using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FlightDispatcherApp.Presentation;

namespace FlightDispatcherApp.Views
{
    // VIEW: creates controls, reads input and renders controller-provided presentation data.
    // All flight rules, model access, and event subscriptions live outside MainForm.
    public sealed class MainForm : Form, IFlightView
    {
        private readonly TextBox flightIdTextBox = new TextBox();
        private readonly TextBox airlineTextBox = new TextBox();
        private readonly TextBox destinationTextBox = new TextBox();
        private readonly NumericUpDown durationInput = new NumericUpDown();
        private readonly Button addFlightButton = new Button();
        private readonly Button takeOffButton = new Button();
        private readonly Button prepareButton = new Button();
        private readonly ListView flightsList = new ListView();
        private readonly ListBox eventsList = new ListBox();
        private readonly Label summaryLabel = new Label();
        private readonly Label helpLabel = new Label();

        public event EventHandler? AddFlightRequested;
        public event EventHandler? TakeOffRequested;
        public event EventHandler? PrepareRequested;
        public event EventHandler? FlightSelectionChanged;

        public string FlightNumberInput => flightIdTextBox.Text;
        public string AirlineInput => airlineTextBox.Text;
        public string DestinationInput => destinationTextBox.Text;
        public double DurationInput => (double)durationInput.Value;
        public string? SelectedFlightNumber => flightsList.SelectedItems.Count > 0
            ? flightsList.SelectedItems[0].Tag as string
            : null;

        public MainForm()
        {
            Text = "Flight Dispatcher";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(970, 690);
            Size = new Size(1140, 770);
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(244, 247, 251);

            BuildInterface();
        }

        private void BuildInterface()
        {
            TableLayoutPanel page = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 95));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 174));
            page.RowStyles.Add(new RowStyle(SizeType.Percent, 57));
            page.RowStyles.Add(new RowStyle(SizeType.Percent, 43));
            page.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            Controls.Add(page);

            page.Controls.Add(CreateHeader(), 0, 0);
            page.Controls.Add(CreateFlightEntryPanel(), 0, 1);
            page.Controls.Add(CreateFlightListPanel(), 0, 2);
            page.Controls.Add(CreateEventsPanel(), 0, 3);
            page.Controls.Add(CreateFooter(), 0, 4);
        }

        private Control CreateHeader()
        {
            TableLayoutPanel header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 46, 77),
                ColumnCount = 2,
                Padding = new Padding(24, 15, 24, 12)
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            TableLayoutPanel titleArea = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
            titleArea.RowStyles.Add(new RowStyle(SizeType.Percent, 64));
            titleArea.RowStyles.Add(new RowStyle(SizeType.Percent, 36));

            Label title = new Label
            {
                Text = "FLIGHT DISPATCHER",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 21F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };
            titleArea.Controls.Add(title, 0, 0);

            summaryLabel.Dock = DockStyle.Fill;
            summaryLabel.TextAlign = ContentAlignment.MiddleRight;
            summaryLabel.ForeColor = Color.White;
            summaryLabel.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);

            header.Controls.Add(titleArea, 0, 0);
            header.Controls.Add(summaryLabel, 1, 0);
            return header;
        }

        private Control CreateFlightEntryPanel()
        {
            TableLayoutPanel section = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(22, 11, 22, 8),
                BackColor = Color.White
            };
            section.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
            section.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            section.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Label heading = new Label
            {
                Text = "REGISTER A FLIGHT  |  1 real second = 1 flight hour",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 67, 102),
                TextAlign = ContentAlignment.MiddleLeft
            };
            section.Controls.Add(heading, 0, 0);

            TableLayoutPanel fields = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 2,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            fields.Controls.Add(MakeFieldLabel("Flight ID"), 0, 0);
            fields.Controls.Add(MakeFieldLabel("Airline / aircraft"), 1, 0);
            fields.Controls.Add(MakeFieldLabel("Destination"), 2, 0);
            fields.Controls.Add(MakeFieldLabel("Duration (hours)"), 3, 0);

            SetFieldAppearance(flightIdTextBox);
            SetFieldAppearance(airlineTextBox);
            SetFieldAppearance(destinationTextBox);
            flightIdTextBox.MaxLength = 22;
            airlineTextBox.MaxLength = 80;
            destinationTextBox.MaxLength = 80;
            flightIdTextBox.PlaceholderText = "e.g. SK101";
            airlineTextBox.PlaceholderText = "e.g. SAS";
            destinationTextBox.PlaceholderText = "e.g. London";

            durationInput.Dock = DockStyle.Fill;
            durationInput.Minimum = 0.5M;
            durationInput.Maximum = 48M;
            durationInput.Increment = 0.5M;
            durationInput.DecimalPlaces = 1;
            durationInput.Value = 4M;
            durationInput.Margin = new Padding(3, 4, 12, 2);
            durationInput.TextAlign = HorizontalAlignment.Center;

            addFlightButton.Text = "+  Add flight";
            StyleButton(addFlightButton, Color.FromArgb(23, 112, 168));
            addFlightButton.Click += AddFlightButton_Click;
            addFlightButton.Margin = new Padding(8, 3, 0, 2);

            fields.Controls.Add(flightIdTextBox, 0, 1);
            fields.Controls.Add(airlineTextBox, 1, 1);
            fields.Controls.Add(destinationTextBox, 2, 1);
            fields.Controls.Add(durationInput, 3, 1);
            fields.Controls.Add(addFlightButton, 4, 1);
            section.Controls.Add(fields, 0, 1);

            TableLayoutPanel actions = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 196));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            takeOffButton.Text = "Take off selected";
            StyleButton(takeOffButton, Color.FromArgb(30, 128, 99));
            takeOffButton.Click += TakeOffButton_Click;
            takeOffButton.Margin = new Padding(0, 4, 9, 0);

            prepareButton.Text = "Prepare next flight";
            StyleButton(prepareButton, Color.FromArgb(80, 91, 115));
            prepareButton.Click += PrepareButton_Click;
            prepareButton.Margin = new Padding(0, 4, 9, 0);

            helpLabel.Text = "Select a landed flight, enter a new destination / duration, then prepare it.";
            helpLabel.ForeColor = Color.FromArgb(91, 105, 122);
            helpLabel.Font = new Font("Segoe UI", 8.8F);
            helpLabel.Dock = DockStyle.Fill;
            helpLabel.TextAlign = ContentAlignment.MiddleLeft;
            helpLabel.AutoEllipsis = true;

            actions.Controls.Add(takeOffButton, 0, 0);
            actions.Controls.Add(prepareButton, 1, 0);
            actions.Controls.Add(helpLabel, 2, 0);
            section.Controls.Add(actions, 0, 2);
            return section;
        }

        private Control CreateFlightListPanel()
        {
            GroupBox group = new GroupBox
            {
                Text = "  Registered flights  ",
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 18, 14, 12),
                Margin = new Padding(22, 12, 22, 6),
                ForeColor = Color.FromArgb(31, 56, 84),
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold)
            };

            flightsList.Dock = DockStyle.Fill;
            flightsList.View = View.Details;
            flightsList.FullRowSelect = true;
            flightsList.HideSelection = false;
            flightsList.MultiSelect = false;
            flightsList.GridLines = true;
            flightsList.BackColor = Color.White;
            flightsList.Font = new Font("Segoe UI", 9.5F);
            flightsList.Columns.Add("Flight ID", 140);
            flightsList.Columns.Add("Airline / aircraft", 204);
            flightsList.Columns.Add("Destination", 210);
            flightsList.Columns.Add("Duration (h)", 127);
            flightsList.Columns.Add("Remaining (h)", 142);
            flightsList.Columns.Add("Status", 145);
            flightsList.SelectedIndexChanged += FlightsList_SelectedIndexChanged;
            group.Controls.Add(flightsList);
            return group;
        }

        private Control CreateEventsPanel()
        {
            GroupBox group = new GroupBox
            {
                Text = "  Live event log — TakeOff / Landed  ",
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 18, 14, 12),
                Margin = new Padding(22, 5, 22, 8),
                ForeColor = Color.FromArgb(31, 56, 84),
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold)
            };

            eventsList.Dock = DockStyle.Fill;
            eventsList.BackColor = Color.White;
            eventsList.HorizontalScrollbar = true;
            eventsList.Font = new Font("Consolas", 9.5F);
            eventsList.IntegralHeight = false;
            group.Controls.Add(eventsList);
            return group;
        }

        private Control CreateFooter()
        {
            Label footer = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Flight Dispatcher",
                ForeColor = Color.FromArgb(101, 115, 134),
                Font = new Font("Segoe UI", 8.7F),
                TextAlign = ContentAlignment.MiddleCenter
            };
            return footer;
        }

        private static Label MakeFieldLabel(string title)
        {
            return new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(76, 88, 107),
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(3, 0, 2, 1)
            };
        }

        private static void SetFieldAppearance(TextBox field)
        {
            field.Dock = DockStyle.Fill;
            field.Margin = new Padding(3, 4, 12, 2);
            field.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleButton(Button button, Color background)
        {
            button.Dock = DockStyle.Fill;
            button.BackColor = background;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;
        }

        // Only raise input notifications; the controller decides what each request does.
        private void AddFlightButton_Click(object? sender, EventArgs e)
        {
            AddFlightRequested?.Invoke(this, EventArgs.Empty);
        }

        private void TakeOffButton_Click(object? sender, EventArgs e)
        {
            TakeOffRequested?.Invoke(this, EventArgs.Empty);
        }

        private void PrepareButton_Click(object? sender, EventArgs e)
        {
            PrepareRequested?.Invoke(this, EventArgs.Empty);
        }

        private void FlightsList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            FlightSelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        // Display only: domain objects never enter the View.
        public void DisplayFlights(IReadOnlyList<FlightRow> rows, string summary)
        {
            flightsList.BeginUpdate();
            try
            {
                foreach (FlightRow row in rows)
                {
                    ListViewItem? item = null;
                    foreach (ListViewItem existing in flightsList.Items)
                    {
                        if (string.Equals(existing.Tag as string, row.FlightNumber,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            item = existing;
                            break;
                        }
                    }

                    if (item == null)
                    {
                        item = new ListViewItem(row.FlightNumber);
                        item.SubItems.Add(row.Airline);
                        item.SubItems.Add(row.Destination);
                        item.SubItems.Add(row.DurationHours);
                        item.SubItems.Add(row.RemainingHours);
                        item.SubItems.Add(row.Status);
                        item.Tag = row.FlightNumber;
                        flightsList.Items.Add(item);
                    }
                    else
                    {
                        item.SubItems[1].Text = row.Airline;
                        item.SubItems[2].Text = row.Destination;
                        item.SubItems[3].Text = row.DurationHours;
                        item.SubItems[4].Text = row.RemainingHours;
                        item.SubItems[5].Text = row.Status;
                    }

                    item.ForeColor = row.Status == "In air"
                        ? Color.FromArgb(20, 109, 158)
                        : row.Status == "Landed"
                            ? Color.FromArgb(27, 126, 88)
                            : Color.FromArgb(46, 58, 73);
                }
            }
            finally
            {
                flightsList.EndUpdate();
            }

            summaryLabel.Text = summary;
        }

        // Called ONLY by the Controller. The View never subscribes to model or logger events.
        public void AppendEvent(string message)
        {
            eventsList.Items.Add(message);
            eventsList.TopIndex = eventsList.Items.Count - 1;
        }

        public void SetActionAvailability(bool canTakeOff, bool canPrepare)
        {
            takeOffButton.Enabled = canTakeOff;
            prepareButton.Enabled = canPrepare;
        }

        public void ClearFlightNumberInput()
        {
            flightIdTextBox.Clear();
        }

        public void SelectFlight(string flightNumber)
        {
            foreach (ListViewItem item in flightsList.Items)
            {
                if (string.Equals(item.Tag as string, flightNumber, StringComparison.OrdinalIgnoreCase))
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                    return;
                }
            }
        }

        public void ShowError(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

    }
}
