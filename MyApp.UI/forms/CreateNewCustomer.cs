using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MyApp.UI.Forms
{
    public class CreateNewCustomer : Form
    {
        private Guna2TextBox txtFirstName;
        private Guna2TextBox txtLastName;
        private Guna2TextBox txtPhone;
        private Guna2TextBox txtAddress;
        private Guna2Button btnDiscard;
        private Guna2Button btnCreateNew;
        private Guna2Button btnViewList;
        private Label lblHeader;

        public CreateNewCustomer()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ----- FORM SETTINGS -----
            this.Text = "Create New Customer";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // ----- HEADER -----
            lblHeader = new Label
            {
                Text = "Create New Customer",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 70),
                AutoSize = true
            };

            // ----- INPUT FIELDS -----
            txtFirstName = CreateTextBox("Enter first name");
            txtLastName = CreateTextBox("Enter last name");
            txtPhone = CreateTextBox("Enter phone number");
            txtAddress = CreateTextBox("Enter address");

            var lblFirstName = CreateLabel("First Name *");
            var lblLastName = CreateLabel("Last Name *");
            var lblPhone = CreateLabel("Phone *");
            var lblAddress = CreateLabel("Address");

            // ----- BUTTONS -----
            btnViewList = new Guna2Button
            {
                Text = "View List",
                Width = 130,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White
            };
            btnViewList.Click += (s, e) =>
            {
                var form = new SearchCustomerForm();
                form.ShowDialog();
                this.Close();
            };

            btnDiscard = new Guna2Button
            {
                Text = "Discard",
                Width = 120,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.LightGray,
                BorderThickness = 1
            };
            btnDiscard.Click += (s, e) => this.Close();

            btnCreateNew = new Guna2Button
            {
                Text = "Create New",
                Width = 150,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.FromArgb(0, 180, 200),
                ForeColor = Color.White
            };
            btnCreateNew.Click += BtnCreateNew_Click;

            // ----- MAIN PANEL -----
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(200, 40, 200, 0)
            };
            this.Controls.Add(mainPanel);

            // ----- STACK PANEL FOR TITLE + FIELDS -----
            var stackPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 20, 0, 0)
            };

            stackPanel.Anchor = AnchorStyles.Top;
            stackPanel.Controls.Add(lblHeader);
            stackPanel.SetFlowBreak(lblHeader, true);
            lblHeader.Margin = new Padding(0, 0, 0, 30);

            AddField(stackPanel, lblFirstName, txtFirstName);
            AddField(stackPanel, lblLastName, txtLastName);
            AddField(stackPanel, lblPhone, txtPhone);
            AddField(stackPanel, lblAddress, txtAddress);

            mainPanel.Controls.Add(stackPanel);

            // ----- BOTTOM BUTTONS (STICKY AT BOTTOM) -----
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                Padding = new Padding(40, 0, 40, 30),
                BackColor = Color.White
            };

            // Add buttons
            bottomPanel.Controls.Add(btnViewList);
            bottomPanel.Controls.Add(btnDiscard);
            bottomPanel.Controls.Add(btnCreateNew);

            // Place buttons dynamically when resized
            bottomPanel.Resize += (s, e) =>
            {
                btnViewList.Location = new Point(40, bottomPanel.Height - btnViewList.Height - 20);
                btnCreateNew.Location = new Point(bottomPanel.Width - btnCreateNew.Width - 40, bottomPanel.Height - btnCreateNew.Height - 20);
                btnDiscard.Location = new Point(btnCreateNew.Left - btnDiscard.Width - 15, bottomPanel.Height - btnDiscard.Height - 20);
            };

            this.Controls.Add(bottomPanel);
        }

        private void AddField(FlowLayoutPanel panel, Label label, Control field)
        {
            label.Margin = new Padding(0, 5, 0, 2);
            field.Margin = new Padding(0, 0, 0, 12);
            panel.Controls.Add(label);
            panel.Controls.Add(field);
        }

        private Guna2TextBox CreateTextBox(string placeholder)
        {
            return new Guna2TextBox
            {
                PlaceholderText = placeholder,
                Width = 400,
                BorderRadius = 6
            };
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private void BtnCreateNew_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please fill all required fields (First Name, Last Name, Phone).",
                    "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Customer created successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
        }
    }
}
