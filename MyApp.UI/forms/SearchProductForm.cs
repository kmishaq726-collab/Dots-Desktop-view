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
    public class SearchProductForm : Form
    {
        private Guna2TextBox txtSearch;
        private Label lblNoProducts;
        private Guna2Button btnDiscard;
        private Guna2Button btnSelectProduct;
        private TableLayoutPanel mainLayout;
        private Guna2DataGridView dgvProducts;

        private List<Product> allProducts = new();
        private Product? selectedProduct;

        public string? SelectedProductName { get; private set; }
        public decimal SelectedProductPrice { get; private set; }

        public SearchProductForm()
        {
            InitializeComponent();
            LoadProductsFromDatabase();
        }

        private void InitializeComponent()
        {
            // ---- FORM SETTINGS ----
            this.Text = "Search Product";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(900, 650);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            // ---- MAIN LAYOUT ----
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(30, 30, 30, 20),
                BackColor = Color.White
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // Label
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // Search box
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // Buttons

            // ---- SEARCH LABEL ----
            var lblSearch = new Label
            {
                Text = "Search by name, barcode, or code",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Dock = DockStyle.Left,
                Margin = new Padding(0, 5, 0, 5)
            };
            mainLayout.Controls.Add(lblSearch, 0, 0);

            // ---- SEARCH BOX ----
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Type here to search...",
                Dock = DockStyle.Top,
                Height = 45,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            mainLayout.Controls.Add(txtSearch, 0, 1);

            // ---- PRODUCT GRID ----
            dgvProducts = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "ProductName", Width = 300 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Barcode", DataPropertyName = "ProductBarCode", Width = 200 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "SalePrice", Width = 100 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tax Type", DataPropertyName = "TaxType", Width = 120 });

            // ---- GRID STYLE ----
            dgvProducts.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(250, 250, 255);
            dgvProducts.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvProducts.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 11);  // 🔹 Larger readable cell font
            dgvProducts.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvProducts.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvProducts.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgvProducts.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); // 🔹 Bold header
            dgvProducts.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(72, 118, 255);
            dgvProducts.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvProducts.ThemeStyle.HeaderStyle.Height = 35;
            dgvProducts.GridColor = Color.FromArgb(240, 240, 240);

            dgvProducts.SelectionChanged += (s, e) =>
            {
                if (dgvProducts.SelectedRows.Count > 0)
                {
                    selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
                    btnSelectProduct.Enabled = selectedProduct != null;
                }
            };

            dgvProducts.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgvProducts.Rows[e.RowIndex].DataBoundItem is Product p)
                {
                    selectedProduct = p;
                    AcceptProductSelection();
                }
            };

            mainLayout.Controls.Add(dgvProducts, 0, 2);

            // ---- NO PRODUCT LABEL ----
            lblNoProducts = new Label
            {
                Text = "No products found!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Visible = false
            };
            mainLayout.Controls.Add(lblNoProducts, 0, 2);

            // ---- BUTTON PANEL ----
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 15, 0, 0),
                AutoSize = true
            };

            btnSelectProduct = new Guna2Button
            {
                Text = "Select Product",
                Width = 160,
                Height = 45,
                BorderRadius = 8,
                FillColor = Color.FromArgb(0, 180, 200),
                ForeColor = Color.White,
                Enabled = false,
                Margin = new Padding(10, 0, 0, 0)
            };
            btnSelectProduct.Click += (s, e) => AcceptProductSelection();

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

            buttonPanel.Controls.Add(btnSelectProduct);
            buttonPanel.Controls.Add(btnDiscard);
            mainLayout.Controls.Add(buttonPanel, 0, 3);

            // ---- ADD TO FORM ----
            Controls.Add(mainLayout);
        }

        // ---------------- LOGIC ---------------- //
        private void AcceptProductSelection()
        {
            if (selectedProduct == null)
            {
                MessageBox.Show("Please select a product.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedProductName = selectedProduct.ProductName;
            SelectedProductPrice = selectedProduct.SalePrice;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadProductsFromDatabase()
        {
            try
            {
                string json = SystemConfigRepository.GetConfig("SaleSessionInfo");
                if (string.IsNullOrWhiteSpace(json))
                {
                    lblNoProducts.Visible = true;
                    dgvProducts.DataSource = null;
                    return;
                }

                var apiResponse = JsonSerializer.Deserialize<SaleSessionInfo>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data?.Products != null)
                {
                    allProducts = apiResponse.Data.Products;
                    dgvProducts.DataSource = allProducts;
                    lblNoProducts.Visible = allProducts.Count == 0;
                }
                else
                {
                    allProducts = new List<Product>();
                    dgvProducts.DataSource = allProducts;
                    lblNoProducts.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string query = txtSearch.Text.ToLower().Trim();

            var filtered = string.IsNullOrEmpty(query)
                ? allProducts
                : allProducts.Where(p =>
                    (p.ProductName ?? "").ToLower().Contains(query) ||
                    (p.ProductBarCode ?? "").ToLower().Contains(query) ||
                    (p.GenericCode ?? "").ToLower().Contains(query)).ToList();

            dgvProducts.DataSource = filtered;
            lblNoProducts.Visible = filtered.Count == 0;
        }
    }
}
