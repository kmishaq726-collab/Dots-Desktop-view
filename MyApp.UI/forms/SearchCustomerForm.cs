using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MyApp.Models;
using MyApp.UI.Data;

namespace MyApp.UI.Forms
{
    public class SearchCustomerForm : Form
    {
        private Guna2TextBox txtSearch;
        private Label lblNoCustomers;
        private Guna2Button btnDiscard;
        private Guna2Button btnSelectCustomer;
        private Guna2Button btnViewForm;
        private TableLayoutPanel mainLayout;
        private Guna2DataGridView dgvCustomers;

        private List<Customer> allCustomers = new();
        private Customer? selectedCustomer;

        public string? SelectedCustomerName { get; private set; }

        public SearchCustomerForm()
        {
            InitializeComponent();
            LoadCustomersFromDatabase();
        }

        private void InitializeComponent()
        {
            // ----- FORM SETTINGS -----
            this.Text = "Search Customers";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(1000, 700);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            // ----- MAIN LAYOUT -----
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(40, 40, 40, 20),
                BackColor = Color.White
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Label
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Search box
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // ----- SEARCH LABEL -----
            var lblSearch = new Label
            {
                Text = "Search by name, phone, or CNIC",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Dock = DockStyle.Left,
                Margin = new Padding(0, 5, 0, 5)
            };
            mainLayout.Controls.Add(lblSearch, 0, 0);

            // ----- SEARCH BOX -----
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Type here to search customers...",
                Dock = DockStyle.Top,
                Height = 45,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            mainLayout.Controls.Add(txtSearch, 0, 1);

            // ----- CUSTOMER GRID -----
            dgvCustomers = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                GridColor = Color.FromArgb(240, 240, 240),
                ColumnHeadersHeight = 35
            };

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "FullName", Width = 250 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Phone", DataPropertyName = "Phone1", Width = 180 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "CNIC", DataPropertyName = "Cnic", Width = 180 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Customer ID", DataPropertyName = "CustomerId", Width = 200 });

            // Style Grid same as product form
            dgvCustomers.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(250, 250, 255);
            dgvCustomers.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvCustomers.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 11);
            dgvCustomers.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvCustomers.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvCustomers.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgvCustomers.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvCustomers.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(72, 118, 255);
            dgvCustomers.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvCustomers.ThemeStyle.HeaderStyle.Height = 35;

            dgvCustomers.SelectionChanged += (s, e) =>
            {
                if (dgvCustomers.SelectedRows.Count > 0)
                {
                    selectedCustomer = dgvCustomers.SelectedRows[0].DataBoundItem as Customer;
                    btnSelectCustomer.Enabled = selectedCustomer != null;
                }
            };

            dgvCustomers.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgvCustomers.Rows[e.RowIndex].DataBoundItem is Customer c)
                {
                    selectedCustomer = c;
                    AcceptCustomerSelection();
                }
            };

            mainLayout.Controls.Add(dgvCustomers, 0, 2);

            // ----- NO CUSTOMERS LABEL -----
            lblNoCustomers = new Label
            {
                Text = "No customers found!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Visible = false
            };
            mainLayout.Controls.Add(lblNoCustomers, 0, 2);

            // ----- BUTTON PANEL -----
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 15, 0, 0),
                AutoSize = true
            };

            btnSelectCustomer = new Guna2Button
            {
                Text = "Select Customer",
                Width = 160,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.FromArgb(0, 180, 200),
                ForeColor = Color.White,
                Enabled = false,
                Margin = new Padding(10, 0, 0, 0)
            };
            btnSelectCustomer.Click += (s, e) => AcceptCustomerSelection();

            btnViewForm = new Guna2Button
            {
                Text = "View Form",
                Width = 150,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.FromArgb(100, 149, 237),
                ForeColor = Color.White
            };
            // Add event if needed

            btnDiscard = new Guna2Button
            {
                Text = "Cancel",
                Width = 130,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.LightGray,
                BorderThickness = 1
            };
            btnDiscard.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            buttonPanel.Controls.Add(btnSelectCustomer);
            buttonPanel.Controls.Add(btnDiscard);
            buttonPanel.Controls.Add(btnViewForm);
            mainLayout.Controls.Add(buttonPanel, 0, 3);

            // ----- ADD TO FORM -----
            Controls.Add(mainLayout);
        }

        private void AcceptCustomerSelection()
        {
            if (selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedCustomerName = selectedCustomer.CustomerId;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadCustomersFromDatabase()
        {
            try
            {
                string json = SystemConfigRepository.GetConfig("SaleSessionInfo");
                if (string.IsNullOrWhiteSpace(json))
                {
                    lblNoCustomers.Visible = true;
                    dgvCustomers.DataSource = null;
                    return;
                }

                var sessionData = JsonSerializer.Deserialize<SaleSessionInfo>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var customers = sessionData?.Data?.Customers;

                if (customers != null && customers.Any())
                {
                    allCustomers = customers;
                    dgvCustomers.DataSource = allCustomers;
                    lblNoCustomers.Visible = false;
                }
                else
                {
                    allCustomers = new List<Customer>();
                    dgvCustomers.DataSource = allCustomers;
                    lblNoCustomers.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string query = txtSearch.Text.ToLower().Trim();

            var filtered = string.IsNullOrEmpty(query)
                ? allCustomers
                : allCustomers.Where(c =>
                    (c.FullName ?? "").ToLower().Contains(query) ||
                    (c.Phone1 ?? "").ToLower().Contains(query) ||
                    (c.Cnic ?? "").ToLower().Contains(query))
                .ToList();

            dgvCustomers.DataSource = filtered;
            lblNoCustomers.Visible = filtered.Count == 0;
        }
    }
}
