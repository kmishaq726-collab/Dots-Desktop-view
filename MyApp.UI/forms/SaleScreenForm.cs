using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MyApp.UI.Forms
{
    public class SaleScreenForm : Form
    {
        private SplitContainer mainSplit;

        // Left side controls
        private Guna2TextBox txtSearch;
        private FlowLayoutPanel cartItemsPanel;
        private Label lblNetTotal;
        private Label lblTotal;
        private Label lblDiscount;
        private Label lblChange;

        // Quick Amount Buttons
        private FlowLayoutPanel quickAmountPanel;
        private Guna2Button btnToggleSign;
        private bool isPositive = true;

        // Right side controls
        private FlowLayoutPanel saleTabsPanel;
        private FlowLayoutPanel paymentMethodsPanel;
        private FlowLayoutPanel pinnedItemsPanel;
        private FlowLayoutPanel actionButtonsPanel;
        private Guna2TextBox txtCash;
        private decimal netTotal = 0;
        private Guna2Button btnBack;

        public SaleScreenForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "POS Sale Screen";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // === Split Layout ===
            mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                SplitterWidth = 6,
                IsSplitterFixed = false
            };

            this.Load += (s, e) =>
            {
                mainSplit.SplitterDistance = this.ClientSize.Width / 2;
            };

            this.Controls.Add(mainSplit);

            // === LEFT PANEL ===
            var leftPanel = mainSplit.Panel1;
            leftPanel.BackColor = Color.White;

            var leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            leftPanel.Controls.Add(leftLayout);

            // Back Button
            btnBack = new Guna2Button
            {
                Text = "Back",
                Dock = DockStyle.Left,
                Width = 80,
                Height = 60,
                BorderRadius = 8,
                FillColor = Color.Red,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(8),
            };
            btnBack.Click += BtnBack_Click;
            leftLayout.Controls.Add(btnBack, 0, 0);

            var leftContent = new Panel { Dock = DockStyle.Fill };
            leftLayout.Controls.Add(leftContent, 0, 1);

            // Search bar
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search by barcode",
                Dock = DockStyle.Top,
                Height = 40,
                BorderRadius = 6,
                Margin = new Padding(5)
            };
            leftContent.Controls.Add(txtSearch);

            // Cart Items
            cartItemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };
            leftContent.Controls.Add(cartItemsPanel);

            // Quick Amount Buttons
            quickAmountPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10),
                WrapContents = false,
                AutoScroll = true
            };

            btnToggleSign = MakeQuickButton("+/-");
            btnToggleSign.Click += ToggleQuickButtons;
            quickAmountPanel.Controls.Add(btnToggleSign);

            quickAmountPanel.Controls.Add(MakeQuickButton("+50", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+100", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+500", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+1000", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+5000", QuickButtonClick));

            leftContent.Controls.Add(quickAmountPanel);

            // Totals area
            var totalsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.WhiteSmoke
            };
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            lblNetTotal = MakeLabel($"Net Total: {netTotal}");
            lblTotal = MakeLabel("Total: 0");
            lblDiscount = MakeLabel("Discount: 0");
            lblChange = MakeLabel("Change: 0");

            totalsPanel.Controls.Add(lblNetTotal, 0, 0);
            totalsPanel.Controls.Add(lblTotal, 1, 0);
            totalsPanel.Controls.Add(lblDiscount, 2, 0);
            totalsPanel.Controls.Add(lblChange, 3, 0);
            leftContent.Controls.Add(totalsPanel);

            // === RIGHT PANEL ===
            var rightPanel = mainSplit.Panel2;
            rightPanel.BackColor = Color.White;

            saleTabsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.AliceBlue
            };
            rightPanel.Controls.Add(saleTabsPanel);

            paymentMethodsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.WhiteSmoke
            };
            rightPanel.Controls.Add(paymentMethodsPanel);

            pinnedItemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };
            rightPanel.Controls.Add(pinnedItemsPanel);

            // === Action container ===
            var actionContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                ColumnCount = 1,
                RowCount = 3
            };
            actionContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            actionContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            actionContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var lblCash = new Label
            {
                Text = "Payment by Cash",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0),
                Margin = new Padding(0, 0, 0, 5)
            };
            actionContainer.Controls.Add(lblCash, 0, 0);

            txtCash = new Guna2TextBox
            {
                PlaceholderText = "Enter cash amount",
                Dock = DockStyle.Fill,
                BorderRadius = 6,
                Margin = new Padding(10, 0, 10, 0),
                Height = 30
            };
            txtCash.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            txtCash.TextChanged += (s, e) =>
            {
                if (decimal.TryParse(txtCash.Text, out var cash))
                    lblChange.Text = $"Change: {cash - netTotal}";
                else
                    lblChange.Text = "Change: 0";
            };

            actionContainer.Controls.Add(txtCash, 0, 1);

            // Action buttons
            actionButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };

            actionButtonsPanel.Controls.Add(MakeButton("Select Product", SelectProduct_Click)); // ✅ added button here
            actionButtonsPanel.Controls.Add(MakeButton("Select Customer", SelectCustomer_Click));

            actionButtonsPanel.Controls.Add(MakeButton("Notes"));
            actionButtonsPanel.Controls.Add(MakeButton("Help"));
            actionButtonsPanel.Controls.Add(MakeButton("Post Sale"));

            actionContainer.Controls.Add(actionButtonsPanel, 0, 2);
            rightPanel.Controls.Add(actionContainer);
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            var Dashboard = new Dashboard();
            this.Hide();
            Dashboard.ShowDialog();
            this.Close();
        }

        // === Open SearchProductForm ===
        private void SelectProduct_Click(object sender, EventArgs e)
        {
            var form = new SearchProductForm();
            form.ShowDialog();
        }

        // === Open SearchCustomerForm ===
        private void SelectCustomer_Click(object sender, EventArgs e)
        {
            var form = new SearchCustomerForm();
            form.ShowDialog(); // Open it as a modal window (same as SearchProductForm)
        }


        private void QuickButtonClick(object sender, EventArgs e)
        {
            if (sender is Guna2Button btn)
            {
                string valueStr = btn.Text.TrimStart('+', '-');
                if (int.TryParse(valueStr, out int value))
                {
                    if (!int.TryParse(txtCash.Text, out int current))
                        current = 0;

                    current += (btn.Text.StartsWith("-") ? -value : value);
                    if (current < 0) current = 0;
                    txtCash.Text = current.ToString();
                }
            }
        }

        private void ToggleQuickButtons(object sender, EventArgs e)
        {
            isPositive = !isPositive;

            foreach (var ctrl in quickAmountPanel.Controls)
            {
                if (ctrl is Guna2Button btn && btn != btnToggleSign)
                {
                    string value = btn.Text.TrimStart('+', '-');
                    btn.Text = (isPositive ? "+" : "-") + value;
                }
            }
        }

        private Label MakeLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private Guna2Button MakeButton(string text, EventHandler? onClick = null)
        {
            var btn = new Guna2Button
            {
                Text = text,
                AutoSize = true,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5)
            };

            if (onClick != null)
                btn.Click += onClick;

            return btn;
        }

        private Guna2Button MakeQuickButton(string text, EventHandler? onClick = null)
        {
            var btn = new Guna2Button
            {
                Text = text,
                Width = 95,
                Height = 35,
                BorderRadius = 6,
                FillColor = Color.DimGray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5)
            };

            if (onClick != null)
                btn.Click += onClick;

            return btn;
        }
    }
}
