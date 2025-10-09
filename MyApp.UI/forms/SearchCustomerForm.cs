using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MyApp.UI.Forms
{
    public class SearchCustomerForm : Form
    {
        private Label lblHeader;
        private Guna2TextBox txtSearch;
        private Label lblNoCustomers;
        private Guna2Button btnDiscard;
        private Guna2Button btnSelectCustomer;
        private Guna2Button btnViewForm;
        private Guna2Panel topPanel;
        private TableLayoutPanel mainLayout;

        public SearchCustomerForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ----- FORM SETTINGS -----
            this.Text = "Search Customers";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Padding = new Padding(0);

            // ----- TOP PANEL -----
            topPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                FillColor = Color.FromArgb(72, 118, 255)
            };

            lblHeader = new Label
            {
                Text = "Search Customers",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(72, 118, 255),
                AutoSize = true,
                Dock = DockStyle.Left,
                Padding = new Padding(20, 20, 0, 0)
            };

            topPanel.Controls.Add(lblHeader);

            // ----- MAIN LAYOUT -----
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(40),
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ----- SEARCH LABEL -----
            var lblSearch = new Label
            {
                Text = "Search by name or phone",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Dock = DockStyle.Left,
                Margin = new Padding(0, 10, 0, 5)
            };

            // ----- SEARCH BOX -----
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Type here to search Customers...",
                Dock = DockStyle.Top,
                Height = 45,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10)
            };

            txtSearch.TextChanged += (s, e) =>
            {
                lblNoCustomers.Visible = string.IsNullOrWhiteSpace(txtSearch.Text);
            };

            // ----- NO CUSTOMERS LABEL -----
            lblNoCustomers = new Label
            {
                Text = "There are no Customers!",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 90),
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var noCustomerPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            noCustomerPanel.Controls.Add(lblNoCustomers);
            lblNoCustomers.Location = new Point(
                (noCustomerPanel.Width - lblNoCustomers.Width) / 2,
                (noCustomerPanel.Height - lblNoCustomers.Height) / 2
            );
            noCustomerPanel.Resize += (s, e) =>
            {
                lblNoCustomers.Left = (noCustomerPanel.Width - lblNoCustomers.Width) / 2;
                lblNoCustomers.Top = (noCustomerPanel.Height - lblNoCustomers.Height) / 2;
            };

            // ----- ADD TO MAIN LAYOUT -----
            mainLayout.Controls.Add(lblSearch, 0, 0);
            mainLayout.Controls.Add(txtSearch, 0, 1);
            mainLayout.Controls.Add(noCustomerPanel, 0, 2);

            // ----- BOTTOM PANEL (FOR BUTTONS) -----
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                Padding = new Padding(40, 0, 40, 30),
                BackColor = Color.White
            };

            // ----- BUTTONS -----
            btnViewForm = new Guna2Button
            {
                Text = "View Form",
                Width = 150,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.FromArgb(100, 149, 237),
                ForeColor = Color.White
            };
            btnViewForm.Click += (s, e) =>
            {
                var createForm = new CreateNewCustomer();
                createForm.ShowDialog();
                this.Close();
            };

            btnSelectCustomer = new Guna2Button
            {
                Text = "Select Customer",
                Width = 160,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.FromArgb(0, 180, 200),
                ForeColor = Color.White,
                Enabled = false
            };

            btnDiscard = new Guna2Button
            {
                Text = "Discard",
                Width = 130,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.LightGray,
                BorderThickness = 1
            };
            btnDiscard.Click += (s, e) => this.Close();

            // Add buttons to bottomPanel
            bottomPanel.Controls.Add(btnViewForm);
            bottomPanel.Controls.Add(btnSelectCustomer);
            bottomPanel.Controls.Add(btnDiscard);

            // Align buttons dynamically
            bottomPanel.Resize += (s, e) =>
            {
                btnViewForm.Location = new Point(40, bottomPanel.Height - btnViewForm.Height - 20);
                btnSelectCustomer.Location = new Point(bottomPanel.Width - btnSelectCustomer.Width - 40, bottomPanel.Height - btnSelectCustomer.Height - 20);
                btnDiscard.Location = new Point(btnSelectCustomer.Left - btnDiscard.Width - 15, bottomPanel.Height - btnDiscard.Height - 20);
            };

            // ----- ADD TO FORM -----
            this.Controls.Add(mainLayout);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);
        }
    }
}
