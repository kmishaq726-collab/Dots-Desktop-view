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
        private bool isPositive = true;   // track + / -

        // Right side controls
        private FlowLayoutPanel saleTabsPanel;
        private FlowLayoutPanel paymentMethodsPanel;
        private FlowLayoutPanel pinnedItemsPanel;
        private FlowLayoutPanel actionButtonsPanel;

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

            // Split 50/50 when form loads
            this.Load += (s, e) =>
            {
                mainSplit.SplitterDistance = this.ClientSize.Width / 2;
            };

            this.Controls.Add(mainSplit);

            // === LEFT PANEL ===
            var leftPanel = mainSplit.Panel1;
            leftPanel.BackColor = Color.White;

            // Search bar
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search by barcode",
                Dock = DockStyle.Top,
                Height = 40,
                BorderRadius = 6,
                Margin = new Padding(5)
            };
            leftPanel.Controls.Add(txtSearch);

            // Cart Items area
            cartItemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };
            leftPanel.Controls.Add(cartItemsPanel);

            // === Quick Amount Buttons ===
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

            // +/- button with event
            btnToggleSign = MakeQuickButton("+/-");
            btnToggleSign.Click += ToggleQuickButtons;
            quickAmountPanel.Controls.Add(btnToggleSign);

            // other quick buttons
            quickAmountPanel.Controls.Add(MakeQuickButton("+50"));
            quickAmountPanel.Controls.Add(MakeQuickButton("+100"));
            quickAmountPanel.Controls.Add(MakeQuickButton("+500"));
            quickAmountPanel.Controls.Add(MakeQuickButton("+1000"));
            quickAmountPanel.Controls.Add(MakeQuickButton("+5000"));

            leftPanel.Controls.Add(quickAmountPanel);

            // === Totals area ===
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

            lblNetTotal = MakeLabel("Net Total: 0");
            lblTotal = MakeLabel("Total: 0");
            lblDiscount = MakeLabel("Discount: 0");
            lblChange = MakeLabel("Change: 0");

            totalsPanel.Controls.Add(lblNetTotal, 0, 0);
            totalsPanel.Controls.Add(lblTotal, 1, 0);
            totalsPanel.Controls.Add(lblDiscount, 2, 0);
            totalsPanel.Controls.Add(lblChange, 3, 0);

            leftPanel.Controls.Add(totalsPanel);

            // === RIGHT PANEL ===
            var rightPanel = mainSplit.Panel2;
            rightPanel.BackColor = Color.White;

            // Tabs (sales tabs)
            saleTabsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.AliceBlue
            };
            rightPanel.Controls.Add(saleTabsPanel);

            // Payment methods row
            paymentMethodsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.WhiteSmoke
            };
            rightPanel.Controls.Add(paymentMethodsPanel);

            // Pinned items area
            pinnedItemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };
            rightPanel.Controls.Add(pinnedItemsPanel);

            // Action buttons row (bottom)
            actionButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };

            var btnCustomer = MakeButton("Select Customer");
            var btnNotes = MakeButton("Notes");
            var btnHelp = MakeButton("Help");
            var btnPostSale = MakeButton("Post Sale");

            actionButtonsPanel.Controls.Add(btnCustomer);
            actionButtonsPanel.Controls.Add(btnNotes);
            actionButtonsPanel.Controls.Add(btnHelp);
            actionButtonsPanel.Controls.Add(btnPostSale);

            rightPanel.Controls.Add(actionButtonsPanel);
        }

        // === Toggle Event ===
        private void ToggleQuickButtons(object sender, EventArgs e)
        {
            isPositive = !isPositive; // switch mode

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

        private Guna2Button MakeButton(string text)
        {
            return new Guna2Button
            {
                Text = text,
                AutoSize = true,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5)
            };
        }

        private Guna2Button MakeQuickButton(string text)
        {
            return new Guna2Button
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
        }
    }
}
