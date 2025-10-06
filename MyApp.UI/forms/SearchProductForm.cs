using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MyApp.UI.Forms
{
    public class SearchProductForm : Form
    {
        private Label lblHeader;
        private Guna2TextBox txtSearch;
        private Label lblNoProducts;
        private Guna2Button btnDiscard;
        private Guna2Button btnSelectProduct;
        private Guna2Panel topPanel;
        private TableLayoutPanel mainLayout;

        public SearchProductForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ----- FORM SETTINGS -----
            this.Text = "Search Products";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable; // Built-in Min/Max/Close
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
                Text = "Search Products",
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
                RowCount = 4,
                Padding = new Padding(40),
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // Search label
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // Search box
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Center message
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // Buttons

            // ----- SEARCH LABEL -----
            var lblSearch = new Label
            {
                Text = "Search by name, barcode, or generic code",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Dock = DockStyle.Left,
                Margin = new Padding(0, 10, 0, 5)
            };

            // ----- SEARCH BOX -----
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Type here to search products...",
                Dock = DockStyle.Top,
                Height = 45,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10)
            };

            txtSearch.TextChanged += (s, e) =>
            {
                lblNoProducts.Visible = string.IsNullOrWhiteSpace(txtSearch.Text);
            };

            // ----- NO PRODUCT LABEL -----
            lblNoProducts = new Label
            {
                Text = "There are no products!",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 90),
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 50, 0, 50)
            };

            var noProductPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            noProductPanel.Controls.Add(lblNoProducts);
            lblNoProducts.Location = new Point(
                (noProductPanel.Width - lblNoProducts.Width) / 2,
                (noProductPanel.Height - lblNoProducts.Height) / 2
            );
            noProductPanel.Resize += (s, e) =>
            {
                lblNoProducts.Left = (noProductPanel.Width - lblNoProducts.Width) / 2;
                lblNoProducts.Top = (noProductPanel.Height - lblNoProducts.Height) / 2;
            };

            // ----- BUTTONS -----
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 10, 0, 0),
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

            // ✅ Go back to SaleScreenForm on Discard
            btnDiscard.Click += (s, e) =>
            {
                this.Close();
            };

            buttonPanel.Controls.Add(btnSelectProduct);
            buttonPanel.Controls.Add(btnDiscard);

            // ----- ADD TO MAIN LAYOUT -----
            mainLayout.Controls.Add(lblSearch, 0, 0);
            mainLayout.Controls.Add(txtSearch, 0, 1);
            mainLayout.Controls.Add(noProductPanel, 0, 2);
            mainLayout.Controls.Add(buttonPanel, 0, 3);

            // ----- ADD TO FORM -----
            this.Controls.Add(mainLayout);
            this.Controls.Add(topPanel);
        }
    }
}
