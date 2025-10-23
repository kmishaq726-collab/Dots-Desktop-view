using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using MyApp.Common;
using MyApp.Models;
using MyApp.UI.Data;
using MyApp.UI.Forms;
using MyApp.UI.Services;
using SQLitePCL;

namespace MyApp.UI.Controls
{
    public class SaleTabControl : UserControl
    {
        // Properties and Fields
        public decimal _NotSyncedTotal { get; set; } = 0;
        public decimal netTotal { get; set; } = 0;
        public decimal netDiscount { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public string customerid { get; set; } = null;
        private PaymentMethod selectedPaymentMethod { get; set; } = new PaymentMethod();
        private bool isPositive = true;

        private static  System.Text.Json.JsonSerializerOptions jsonHelper = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // UI Controls - Left Panel
        private SplitContainer mainSplit;
        private Guna2TextBox txtSearch;
        private FlowLayoutPanel cartItemsPanel;
        private Label lblNetTotal, lblTotal, lblDiscount, lblChange;
        private FlowLayoutPanel quickAmountPanel;
        private Guna2Button btnToggleSign;

        // UI Controls - Right Panel
        private FlowLayoutPanel pinnedItemsPanel;
        private FlowLayoutPanel actionButtonsPanel;
        private Guna2TextBox txtCash;
        private TableLayoutPanel rightPanelLayout;

        // Button References
        private Guna2Button btnSelectProduct;
        private Guna2Button btnSelectCustomer;
        private Guna2Button btnPostSale;
        private Guna2Button btnHelp;
        private Guna2Button btnCash;
        private Guna2Button btnSync;
        private Guna2Button btnReceivables;

        // Events
        public event Action NewTabRequested;
        public event Action RemoveTabRequested;
        public event Action PairPrinterRequested;
        public event Action PrintLastInvoiceRequested;
        public event Action MiscellaneousServicesRequested;
        public event Action AddNotesRequested;

        public SaleTabControl()
        {
            BuildUI();
            SetupKeyHandling();
            InitializePinnedProducts(); // Add this line

        }

        #region Initialization Methods

        private void SetupKeyHandling()
        {
            this.PreviewKeyDown += SaleTabControl_PreviewKeyDown;
            this.KeyDown += SaleTabControl_KeyDown;
            AttachKeyHandlingToChildren(this);
            this.TabStop = true;
            this.SetStyle(ControlStyles.Selectable, true);
        }

        private void AttachKeyHandlingToChildren(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                child.PreviewKeyDown += SaleTabControl_PreviewKeyDown;
                child.KeyDown += SaleTabControl_KeyDown;

                if (child.HasChildren)
                {
                    AttachKeyHandlingToChildren(child);
                }
            }
        }

        private void BuildUI()
        {
            _NotSyncedTotal = SystemConfigRepository.GetNotSyncedTotal();
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                SplitterWidth = 6
            };
            this.Controls.Add(mainSplit);

            this.Resize += (s, e) =>
            {
                mainSplit.SplitterDistance = (int)(this.Width * 0.45);
            };

            BuildLeftPanel(mainSplit.Panel1);
            BuildRightPanel(mainSplit.Panel2);
        }

        #endregion

        #region Left Panel Construction

        private void BuildLeftPanel(Panel leftPanel)
        {
            leftPanel.BackColor = Color.White;
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
            leftPanel.Controls.Add(layout);

            BuildSearchSection(layout);
            BuildCartItemsSection(layout);
            BuildBottomSection(layout);
        }

        private void BuildSearchSection(TableLayoutPanel layout)
        {
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search product barcode...",
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Height = 40
            };
            txtSearch.KeyDown += TxtSearch_KeyDown;
            layout.Controls.Add(txtSearch, 0, 0);
        }

        private void BuildCartItemsSection(TableLayoutPanel layout)
        {
            cartItemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            layout.Controls.Add(cartItemsPanel, 0, 1);
        }

        private void BuildBottomSection(TableLayoutPanel layout)
        {
            var bottomPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2
            };
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            BuildTotalsSection(bottomPanel);
            BuildQuickAmountsSection(bottomPanel);

            layout.Controls.Add(bottomPanel, 0, 2);
        }

        private void BuildTotalsSection(TableLayoutPanel bottomPanel)
        {
            var totals = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            lblNetTotal = MakeLabel("Net Total: 0");
            lblTotal = MakeLabel("Total: 0");
            lblDiscount = MakeLabel("Discount: 0");
            lblChange = MakeLabel("Change: 0");

            totals.Controls.Add(lblNetTotal, 0, 0);
            totals.Controls.Add(lblTotal, 1, 0);
            totals.Controls.Add(lblDiscount, 2, 0);
            totals.Controls.Add(lblChange, 3, 0);
            bottomPanel.Controls.Add(totals, 0, 0);
        }

        private void BuildQuickAmountsSection(TableLayoutPanel bottomPanel)
        {
            quickAmountPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };

            btnToggleSign = MakeQuickButton("+/-", ToggleQuickButtons);
            quickAmountPanel.Controls.Add(btnToggleSign);
            quickAmountPanel.Controls.Add(MakeQuickButton("+50", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+100", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+500", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+1000", QuickButtonClick));
            quickAmountPanel.Controls.Add(MakeQuickButton("+5000", QuickButtonClick));

            bottomPanel.Controls.Add(quickAmountPanel, 0, 1);
        }

        #endregion

        #region Right Panel Construction

        private void BuildRightPanel(Panel rightPanel)
        {
            rightPanel.BackColor = Color.White;

            rightPanelLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
            rightPanel.Controls.Add(rightPanelLayout);

            BuildPaymentMethodsSection();
            BuildPinnedItemsSection();
            BuildActionButtonsSection();
        }

        private void CreateDefaultPaymentButtons(FlowLayoutPanel paymentFlowLayout)
        {
            // Create default Cash button
            btnCash = CreatePaymentButton("Cash", Color.SeaGreen, "Cash");
            btnCash.Click += PaymentMethod_Click;
            paymentFlowLayout.Controls.Add(btnCash);

            // Create default Receivables button
            btnReceivables = CreatePaymentButton("Receivables", Color.FromArgb(72, 118, 255), "Receivables");
            btnReceivables.Click += PaymentMethod_Click;
            paymentFlowLayout.Controls.Add(btnReceivables);
        }

        private void BuildPaymentMethodsSection()
        {
            var paymentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 5, 10, 5)
            };

            var paymentFlowLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };

            // Clear existing buttons
            btnCash = null;
            btnReceivables = null;

            try
            {
                // Load payment methods from system configuration
                string saleInfo = SystemConfigRepository.GetConfig("SaleSessionInfo");
                if (!string.IsNullOrEmpty(saleInfo))
                {
                    var sessionInfo = JsonSerializer.Deserialize<SaleSessionInfo>(saleInfo, jsonHelper);
                    var paymentMethods = sessionInfo?.Data?.PaymentMethods;

                    if (paymentMethods != null && paymentMethods.Any())
                    {
                        // Create buttons for each payment method
                        foreach (var paymentMethod in paymentMethods)
                        {
                            var button = CreatePaymentButton(
                                paymentMethod.SalePaymentMethodName ?? "Unknown",
                                Color.FromArgb(72, 118, 255),
                                paymentMethod.SalePaymentMethodName ?? "Unknown"
                            );

                            button.Click += PaymentMethod_Click;
                            paymentFlowLayout.Controls.Add(button);
                        }
                    }
                    else
                    {
                        CreateDefaultPaymentButtons(paymentFlowLayout);
                    }
                }
                else
                {
                    CreateDefaultPaymentButtons(paymentFlowLayout);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payment methods: {ex.Message}. Using default buttons.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CreateDefaultPaymentButtons(paymentFlowLayout);
            }

            btnSync = CreateSyncButton();
            btnSync.Click += (s, e) => PostSaleApiCall();

            paymentFlowLayout.Controls.Add(btnSync);
            paymentPanel.Controls.Add(paymentFlowLayout);
            rightPanelLayout.Controls.Add(paymentPanel, 0, 0);
        }

        private Guna2Button CreatePaymentButton(string text, Color color, string tag)
        {
            return new Guna2Button
            {
                Text = text,
                AutoSize = true,
                BorderRadius = 8,
                FillColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5),
                Tag = tag
            };
        }

        private Guna2Button CreateSyncButton()
        {
            return new Guna2Button
            {
                Text = $"📶 Sync: {_NotSyncedTotal}",
                AutoSize = false,
                Size = new Size(160, 40),
                BorderRadius = 8,
                FillColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(5),
                Tag = "Sync"
            };
        }

        private void BuildPinnedItemsSection()
        {
            var pinnedLabel = new Label
            {
                Text = "Pinned Products",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            rightPanelLayout.Controls.Add(pinnedLabel, 0, 1);

            pinnedItemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.WhiteSmoke
            };
            rightPanelLayout.Controls.Add(pinnedItemsPanel, 0, 2);
        }

        private void BuildActionButtonsSection()
        {
            var actionContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            actionContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            actionContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            actionContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            BuildPaymentLabel(actionContainer);
            BuildCashInput(actionContainer);
            BuildActionButtons(actionContainer);

            rightPanelLayout.Controls.Add(actionContainer, 0, 3);
        }

        private void BuildPaymentLabel(TableLayoutPanel actionContainer)
        {
            var lblPayment = new Label
            {
                Text = "Payment by Cash",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft
            };
            actionContainer.Controls.Add(lblPayment, 0, 0);
        }

        private void BuildCashInput(TableLayoutPanel actionContainer)
        {
            txtCash = new Guna2TextBox
            {
                PlaceholderText = "Enter cash amount",
                Dock = DockStyle.Fill,
                BorderRadius = 6,
                Margin = new Padding(10, 0, 10, 0)
            };
            txtCash.TextChanged += (s, e) =>
            {
                if (decimal.TryParse(txtCash.Text, out var cash))
                    lblChange.Text = $"Change: {cash - netTotal:0.00}";
                else lblChange.Text = "Change: 0";
            };
            actionContainer.Controls.Add(txtCash, 0, 1);
        }

        private void BuildActionButtons(TableLayoutPanel actionContainer)
        {
            actionButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };

            btnSelectProduct = MakeButton("Select Product", SelectProductClick);
            btnSelectCustomer = MakeButton("Select Customer", SelectCustomerClick);
            btnPostSale = MakeButton("Post Sale", PostSaleClick);
            btnHelp = MakeButton("Help", HelpClick);

            btnPostSale.AutoSize = false;
            btnPostSale.Size = new Size(150, 45);
            btnPostSale.FillColor = Color.SeaGreen;
            btnPostSale.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            var spacerPanel = new Panel
            {
                AutoSize = false,
                Size = new Size(100, 1),
                Dock = DockStyle.Fill
            };

            actionButtonsPanel.Controls.Add(btnSelectProduct);
            actionButtonsPanel.Controls.Add(btnSelectCustomer);
            actionButtonsPanel.Controls.Add(btnHelp);
            actionButtonsPanel.Controls.Add(spacerPanel);
            actionButtonsPanel.Controls.Add(btnPostSale);

            actionContainer.Controls.Add(actionButtonsPanel, 0, 2);
        }

        #endregion

        #region Event Handlers

        private void PaymentMethod_Click(object sender, EventArgs e)
        {
            if (sender is Guna2Button clickedButton)
            {
                ResetAllPaymentButtons();

                clickedButton.FillColor = Color.SeaGreen;

                string paymentMethod = clickedButton.Tag?.ToString() ?? "Cash";

                UpdateSelectedPaymentMethod(paymentMethod);

                UpdatePaymentUI(paymentMethod);

                MessageBox.Show($"Payment method set to: {paymentMethod}", "Payment Method",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetAllPaymentButtons()
        {
            var paymentPanel = rightPanelLayout.Controls[0] as Panel;
            if (paymentPanel != null)
            {
                var flowLayout = paymentPanel.Controls[0] as FlowLayoutPanel;
                if (flowLayout != null)
                {
                    foreach (var control in flowLayout.Controls)
                    {
                        if (control is Guna2Button btn && btn.Tag?.ToString() != "Sync")
                        {
                            btn.FillColor = Color.FromArgb(72, 118, 255);
                        }
                    }
                }
            }
        }

        private void UpdateSelectedPaymentMethod(string paymentMethod)
        {
            try
            {
                string saleInfo = SystemConfigRepository.GetConfig("SaleSessionInfo");
                if (!string.IsNullOrEmpty(saleInfo))
                {
                    var sessionInfo = JsonSerializer.Deserialize<SaleSessionInfo>(saleInfo, jsonHelper);
                    var paymentMethods = sessionInfo?.Data?.PaymentMethods;

                    if (paymentMethods != null)
                    {

                        selectedPaymentMethod = paymentMethods.FirstOrDefault(pm =>
                            pm.SalePaymentMethodName.Equals(paymentMethod)) ?? new PaymentMethod();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payment methods: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                selectedPaymentMethod = new PaymentMethod();
            }
        }

        private void UpdatePaymentUI(string paymentMethod)
        {
            var actionContainer = rightPanelLayout.Controls[3] as TableLayoutPanel;
            if (actionContainer != null)
            {
                var lblPayment = actionContainer.Controls[0] as Label;
                if (lblPayment != null)
                {
                    lblPayment.Text = paymentMethod.ToLower().Contains("cash") ?
                        "Payment by Cash" : $"Payment by {paymentMethod}";
                }
            }

            if (paymentMethod.ToLower().Contains("cash"))
            {
                txtCash.PlaceholderText = "Enter cash amount";
            }
            else if (paymentMethod.ToLower().Contains("receivable"))
            {
                txtCash.PlaceholderText = "Enter receivables amount";
                if (netTotal > 0)
                {
                    txtCash.Text = netTotal.ToString("0.00");
                    txtCash.Focus();
                    txtCash.SelectAll();
                }
            }
            else
            {
                txtCash.PlaceholderText = $"Enter {paymentMethod} amount";
            }
        }

        private void SaleTabControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.T:
                case Keys.R:
                case Keys.A:
                case Keys.L:
                case Keys.M:
                case Keys.H:
                case Keys.Enter:
                case Keys.N:
                case Keys.OemQuestion:
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void SaleTabControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Alt) return;
            e.Handled = true;

            switch (e.KeyCode)
            {
                case Keys.T: NewTabRequested?.Invoke(); break;
                case Keys.R: RemoveTabRequested?.Invoke(); break;
                case Keys.A: PairPrinterRequested?.Invoke(); break;
                case Keys.L: PrintLastInvoiceRequested?.Invoke(); break;
                case Keys.M: MiscellaneousServicesRequested?.Invoke(); break;
                case Keys.H: HelpClick(null, null); break;
                case Keys.Enter: PostSaleClick(null, null); break;
                case Keys.N: AddNotesRequested?.Invoke(); break;
                case Keys.OemQuestion: FocusCashInput(); break;
                case Keys.D0: ToggleQuickButtons(null, null); break;
                case Keys.D1: AddQuickAmount(50); break;
                case Keys.D2: AddQuickAmount(100); break;
                case Keys.D3: AddQuickAmount(500); break;
                case Keys.D4: AddQuickAmount(1000); break;
                case Keys.D5: AddQuickAmount(5000); break;
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(txtSearch.Text))
            {
                SearchProductByBarcode(txtSearch.Text);
                txtSearch.Clear();
                e.Handled = true;
            }
        }

        // Button Click Handlers
        private void PostSaleClick(object? sender, EventArgs? e)
        {
            if (cartItemsPanel.Controls.Count <= 0)
            {
                MessageBox.Show("Cannot post sale with zero amount.", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!ValidateCashInput())
                return;

            ProcessSalePosting();
        }

        private void SelectCustomerClick(object sender, EventArgs e)
        {
            using var dlg = new SearchCustomerForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                customerid = dlg?.SelectedCustomerName;
                MessageBox.Show(customerid);
            }
        }

        private void SelectProductClick(object sender, EventArgs e)
        {
            using var dlg = new SearchProductForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var name = dlg.SelectedProductName;
                var price = dlg.SelectedProductPrice;
                var product = LoadProductByName(name);

                if (product != null)
                {
                    AddProductToCart(product.ProductName, product.SalePrice);
                }
                else
                {
                    AddProductToCart(name ?? "Unknown", price);
                }
            }
        }

        private void HelpClick(object? sender, EventArgs? e)
        {
            new HelpModel.displayMessage();
        }

        private void QuickButtonClick(object sender, EventArgs e)
        {
            if (sender is Guna2Button btn)
            {
                string valueStr = btn.Text.TrimStart('+', '-');
                if (int.TryParse(valueStr, out int value))
                {
                    if (!int.TryParse(txtCash.Text, out int current)) current = 0;
                    current += (btn.Text.StartsWith("-") ? -value : value);
                    if (current < 0) current = 0;
                    txtCash.Text = current.ToString();
                    txtCash.Focus();
                    txtCash.SelectAll();
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
            btnToggleSign.FillColor = isPositive ? Color.SeaGreen : Color.Crimson;
        }

        #endregion

        #region Cart Management

        private void AddProductToCart(string productName, decimal price)
        {
            if (TryIncrementExistingProduct(productName))
                return;

            CreateNewCartItem(productName, price);
        }

        private bool TryIncrementExistingProduct(string productName)
        {
            foreach (Panel existingItem in cartItemsPanel.Controls.OfType<Panel>())
            {
                var lbl = existingItem.Controls.OfType<Guna2Button>().FirstOrDefault(c => c.Name == "lblName");
                if (lbl != null && lbl.Text == productName)
                {
                    var qty = existingItem.Controls.OfType<NumericUpDown>().FirstOrDefault();
                    if (qty != null)
                    {
                        qty.Value += 1;
                        return true;
                    }
                }
            }
            return false;
        }

        private void CreateNewCartItem(string productName, decimal price)
        {
            var itemPanel = new Guna2Panel
            {
                Height = 120,
                Width = cartItemsPanel.ClientSize.Width - 25,
                BorderRadius = 12,
                FillColor = Color.White,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(230, 230, 230),
                Margin = new Padding(6),
                ShadowDecoration = { Enabled = true, Shadow = new Padding(3, 3, 5, 5) }
            };

            AddCartItemControls(itemPanel, productName, price);
            cartItemsPanel.Controls.Add(itemPanel);
            RecalculateNetTotal();
        }

        private void AddCartItemControls(Panel itemPanel, string productName, decimal price)
        {
            // === Row 1: Product Info ===
            var btnName = new Guna2Button
            {
                Name = "lblName",
                Text = productName,
                Font = new Font("Segoe UI Semibold", 11),
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.FromArgb(45, 45, 45),
                BorderRadius = 8,
                HoverState = { FillColor = Color.FromArgb(240, 240, 255) },
                Width = 230,
                Height = 40,
                Location = new Point(15, 15),
                TextAlign = HorizontalAlignment.Left
            };
            btnName.Click += CartItemClick;

            var lblPrice = CreateCartLabel("lblPrice", price.ToString("0.00"), 260, 70, FontStyle.Regular, Color.FromArgb(60, 60, 60), 20);
            var numQty = CreateQuantitySpinner(360, 18);

            // 📌 Pin & ❌ Remove buttons (larger + right aligned)
            var btnPin = CreateIconButton("📌", Color.Goldenrod, itemPanel.Width - 120, 12, 50, 45, 15);
            var btnRemove = CreateIconButton("✖", Color.Crimson, itemPanel.Width - 60, 12, 50, 45, 15);

            // === Row 2: Discount + Hidden Total ===
            var lblDiscount = CreateCartLabel("lblDiscount", "Discount: 0.00", 35, 200, FontStyle.Italic, Color.FromArgb(180, 0, 0), 70);
            var lblCalculation = CreateCartLabel("lblCalculation", $"{price} × {numQty.Value} = {price * numQty.Value:0.00}", 250, 350, FontStyle.Regular, Color.FromArgb(90, 90, 90), 70);
            var lblTotal = CreateCartLabel("lblTotal", $"Total: {(price * numQty.Value):0.00}", 700, 200, FontStyle.Bold, Color.FromArgb(0, 120, 215), 70);
            lblTotal.Visible = false; // 🔹 Hidden but still usable for logic

            // === Setup events ===
            SetupCartItemEvents(itemPanel, lblPrice, numQty, lblTotal, lblDiscount, lblCalculation, btnPin, btnRemove, productName, price);

            // === Add all controls ===
            itemPanel.Controls.AddRange(new Control[]
            {
        btnName, lblPrice, numQty,
        btnPin, btnRemove,
        lblDiscount, lblCalculation, lblTotal
            });

            // ✅ Adjust right-aligned buttons when resized
            itemPanel.Resize += (s, e) =>
            {
                btnRemove.Left = itemPanel.Width - 65;
                btnPin.Left = itemPanel.Width - 125;
            };
        }

        private Label CreateCartLabel(string name, string text, int x, int width, FontStyle style, Color color, int y)
        {
            return new Label
            {
                Name = name,
                Text = text,
                Font = new Font("Segoe UI", 10, style),
                AutoSize = false,
                Width = width,
                Height = 25,
                Location = new Point(x, y),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = color
            };
        }

        private NumericUpDown CreateQuantitySpinner(int x, int y)
        {
            return new NumericUpDown
            {
                Minimum = 1,
                Maximum = 9999,
                Value = 1,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White,
                Width = 70,
                Height = 30,
                Location = new Point(x, y),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };
        }

        private Guna2Button CreateIconButton(string icon, Color fill, int x, int y, int width, int height, int fontSize)
        {
            return new Guna2Button
            {
                Text = icon,
                FillColor = fill,
                ForeColor = Color.White,
                BorderRadius = 10,
                Width = width,
                Height = height,
                Font = new Font("Segoe UI Emoji", fontSize),
                Location = new Point(x, y),
                ShadowDecoration = { Enabled = true, Shadow = new Padding(2, 2, 4, 4) },
                HoverState = { FillColor = ControlPaint.Light(fill) },
                Cursor = Cursors.Hand
            };
        }

        private void SetupCartItemEvents(Panel itemPanel, Label lblPrice, NumericUpDown numQty,
            Label lblTotal, Label lblDiscount, Label lblCalculation,
            Guna2Button btnPin, Guna2Button btnRemove, string productName, decimal price)
        {
            numQty.ValueChanged += (s, e) =>
            {
                UpdateCartItemCalculation(lblPrice, numQty, lblTotal, lblDiscount, lblCalculation);
            };

            lblPrice.DoubleClick += (s, e) =>
            {
                ChangeCartItemPrice(lblPrice, numQty, lblTotal, lblDiscount, lblCalculation);
            };

            lblDiscount.DoubleClick += (s, e) =>
            {
                ChangeCartItemDiscount(lblPrice, numQty, lblTotal, lblDiscount, lblCalculation);
            };

            btnRemove.Click += (s, e) =>
            {
                cartItemsPanel.Controls.Remove(itemPanel);
                RecalculateNetTotal();
            };

            btnPin.Click += (s, e) => ToggleProductPin(productName, price);
        }

        private void ToggleProductPin(string productName, decimal price)
        {
            var existing = pinnedItemsPanel.Controls.OfType<Guna2Button>().FirstOrDefault(b => (b.Tag as string) == productName);

            if (existing != null)
            {
                PinnedProductsService.UnpinProduct(productName);
            }
            else
            {
                PinnedProductsService.PinProduct(productName, price);
            }
        }

        private void CartItemClick(object? sender, EventArgs? e)
        {
            if (sender is not Guna2Button btn) return;

            var itemPanel = btn.Parent as Panel;
            if (itemPanel == null) return;

            var lblPrice = itemPanel.Controls.OfType<Label>().FirstOrDefault(c => c.Name == "lblPrice");
            var numQty = itemPanel.Controls.OfType<NumericUpDown>().FirstOrDefault();
            var lblTotal = itemPanel.Controls.OfType<Label>().FirstOrDefault(c => c.Name == "lblTotal");
            var lblDiscount = itemPanel.Controls.OfType<Label>().FirstOrDefault(c => c.Name == "lblDiscount");
            var lblCalculation = itemPanel.Controls.OfType<Label>().FirstOrDefault(c => c.Name == "lblCalculation");

            decimal currentPrice = 0;
            int currentQty = numQty != null ? (int)numQty.Value : 0;
            decimal currentDiscount = 0;

            if (lblPrice != null) decimal.TryParse(lblPrice.Text, out currentPrice);
            if (numQty != null) currentQty = (int)numQty.Value;
            if (lblDiscount != null)
            {
                string discText = lblDiscount.Text.Replace("Discount:", "").Trim();
                decimal.TryParse(discText, out currentDiscount);
            }

            using (var dlg = new CustomAmountForm())
            {
                dlg.SalePrice = currentPrice;
                dlg.Quantity = currentQty;
                dlg.Discount = currentDiscount;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (lblPrice != null) lblPrice.Text = dlg.SalePrice.ToString("0.00");
                    if (numQty != null) numQty.Value = dlg.Quantity;
                    if (lblDiscount != null) lblDiscount.Text = $"Discount: {dlg.Discount:0.00}";

                    decimal total = dlg.SalePrice * dlg.Quantity;
                    if (lblTotal != null)
                        lblTotal.Text = $"Total: {total - dlg.Discount:0.00}"; // still updates, but hidden
                    if (lblCalculation != null)
                        lblCalculation.Text = $"{dlg.SalePrice} × {dlg.Quantity} = {total:0.00}";

                    RecalculateNetTotal();
                }
            }
        }
        #endregion

        #region Calculation Logic

        private void UpdateCartItemCalculation(Label lblPrice, NumericUpDown numQty, Label lblTotal, Label lblDiscount, Label lblCalculation)
        {
            if (!decimal.TryParse(lblPrice.Text, out var price)) return;

            decimal qty = numQty.Value;
            decimal total = price * qty;

            decimal discount = 0;
            string discText = lblDiscount.Text.Replace("Discount:", "").Trim();
            decimal.TryParse(discText, out discount);

            lblTotal.Text = $"Total: {total - discount:0.00}";

            lblCalculation.Text = $"{price:0.00} × {qty} = {total:0.00} - {discount:0.00}";

            RecalculateNetTotal();
        }

        private void ChangeCartItemPrice(Label lblPrice, NumericUpDown numQty, Label lblTotal, Label lblDiscount, Label lblCalculation)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter new price:", "Change Price", lblPrice.Text);
            if (decimal.TryParse(input, out var newPrice))
            {
                lblPrice.Text = newPrice.ToString("0.00");
                UpdateCartItemCalculation(lblPrice, numQty, lblTotal, lblDiscount, lblCalculation);
            }
        }

        private void ChangeCartItemDiscount(Label lblPrice, NumericUpDown numQty, Label lblTotal, Label lblDiscount, Label lblCalculation)
        {
            string currentDiscount = lblDiscount.Text.Replace("Discount:", "").Trim();
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter discount amount:", "Change Discount", currentDiscount);
            if (decimal.TryParse(input, out var newDiscount))
            {
                lblDiscount.Text = $"Discount: {newDiscount:0.00}";
                UpdateCartItemCalculation(lblPrice, numQty, lblTotal, lblDiscount, lblCalculation);
            }
        }

        private void RecalculateNetTotal()
        {
            decimal totalAmount = 0;
            decimal totalDiscount = 0;

            foreach (Guna2Panel item in cartItemsPanel.Controls.OfType<Guna2Panel>())
            {
                var lblTotal = item.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblTotal");
                if (lblTotal != null)
                {
                    string txt = lblTotal.Text.Replace("Total:", "").Trim();
                    if (decimal.TryParse(txt, out var t))
                        totalAmount += t;
                }


                var lblDiscount = item.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblDiscount");
                if (lblDiscount != null)
                {
                    string disc = lblDiscount.Text.Replace("Discount:", "").Trim();
                    if (decimal.TryParse(disc, out var d))
                        totalDiscount += d;
                }
            }

            netTotal = totalAmount;

            decimal netAmount = totalAmount;
            netDiscount = totalDiscount;
            decimal finalTotal = netAmount;

            Total = netAmount + netDiscount;


            // update footer labels
            lblTotal.Text = $"Total: {Total:0.00}";
            lblNetTotal.Text = $"Net Total: {netTotal:0.00}";
            lblDiscount.Text = $"Discount: {netDiscount:0.00}";

            if (decimal.TryParse(txtCash.Text, out var cash))
            {
                lblChange.Text = $"Change: {cash - finalTotal:0.00}";
            }
        }

        #endregion

        #region Business Logic Methods

        private void SearchProductByBarcode(string barcode)
        {
            MessageBox.Show($"Searching for product with barcode: {barcode}", "Barcode Search",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddQuickAmount(int amount)
        {
            if (!int.TryParse(txtCash.Text, out int current)) current = 0;
            current += (isPositive ? amount : -amount);
            if (current < 0) current = 0;
            txtCash.Text = current.ToString();
            txtCash.Focus();
            txtCash.SelectAll();
        }

        private bool ValidateCashInput()
        {
            if (string.IsNullOrEmpty(txtCash.Text) || !decimal.TryParse(txtCash.Text, out decimal cash) || cash < netTotal)
            {
                MessageBox.Show("Please enter sufficient cash amount.", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCash.Focus();
                return false;
            }
            return true;
        }

        public void PutSaleData()
        {
            string SaleSessionId = SystemConfigRepository.GetConfig("SaleSessionId");

            List<Product> productsInSale = GetProductsInCart();
            string paymentMethod = selectedPaymentMethod.SalePaymentMethodId;
            if(paymentMethod == null)
            {
                string Data = SystemConfigRepository.GetConfig("SaleSessionInfo");
                var ProductInfo = JsonSerializer.Deserialize<SaleSessionInfo>(Data, jsonHelper);
                paymentMethod = ProductInfo.Data.PaymentMethods[0].SalePaymentMethodId;
            }
            var sale = new PosSale
            {
                SaleSessionId = SaleSessionId ?? string.Empty,
                InvoiceAmount = Total,
                InvoiceDiscount = netDiscount,
                InvoiceNet = netTotal,
                CustomerId = customerid,
                InvoiceDate = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(), 
                FbrInvoiceNumber = null,
                SalePaymentMethods = new List<SalePaymentMethod>
                {
                    new SalePaymentMethod
                    {
                        SalePaymentMethodId = paymentMethod,
                        Amount = netTotal
                    }
                },
                Items = productsInSale.Select(p => new Item
                {
                    CostCenterProductId = p.CostCenterProductId,
                    ProductId = p.ProductId,
                    ProductGroupId = p.ProductGroupId,
                    ProductTypeId = p.ProductTypeId,
                    ProductBrandId = p.ProductBrandId,
                    UnitCount = p.Quantity,
                    LineDiscount = (decimal)p.LineDiscount,
                    UnitPrice = p.SalePrice
                }).ToList()
            };

            customerid = null;
            SystemConfigRepository.SavePostSale($"Sale{_NotSyncedTotal + 1}", sale);
        }

        private void ProcessSalePosting()
        {
            PutSaleData();
            PostSaleApiCall();
            _NotSyncedTotal = SystemConfigRepository.GetNotSyncedTotal();

            UpdateSyncButton();
            MessageBox.Show($"Sale Posted Successfully ✅\nPayment Method: {selectedPaymentMethod.SalePaymentMethodName}", "POS",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearCart();
        }

        private List<Product> GetProductsInCart()
        {
            string Products = SystemConfigRepository.GetConfig("SaleSessionInfo");
            var ProductInfo = JsonSerializer.Deserialize<SaleSessionInfo>(Products, jsonHelper);
            List<Product> productsInSale = new List<Product>();
            foreach (Panel existingItem in cartItemsPanel.Controls.OfType<Panel>())
            {
                var lbl = existingItem.Controls.OfType<Guna2Button>().FirstOrDefault(e => e.Name == "lblName");
                var lblDis = existingItem.Controls.OfType<Label>().FirstOrDefault(e => e.Name == "lblDiscount");
                var qty = existingItem.Controls.OfType<NumericUpDown>().FirstOrDefault();
                Product p = ProductInfo.Data?.Products?.FirstOrDefault(pr => pr.ProductName == lbl?.Text);
                p.Quantity = (int)qty.Value;
                p.LineDiscount = double.Parse((lblDis.Text).Split(" ")[1]);
                MessageBox.Show(p.LineDiscount.ToString());
                productsInSale.Add(p);
            }
            return productsInSale;
        }

        private void ClearCart()
        {
            cartItemsPanel.Controls.Clear();
            netTotal = 0;
            netDiscount = 0;
            Total = 0;
            txtCash.Clear();

            lblTotal.Text = "Total: 0";
            lblNetTotal.Text = "Net Total: 0";
            lblDiscount.Text = "Discount: 0";
            lblChange.Text = "Change: 0";
            txtCash.PlaceholderText = "Enter cash amount";
        }

        public Product? LoadProductByName(string productName)
        {
            string productData = SystemConfigRepository.GetConfig("SaleSessionInfo");
            if (!string.IsNullOrEmpty(productData))
            {
                var apiResponse = JsonSerializer.Deserialize<SaleSessionInfo>(productData, jsonHelper);
                return apiResponse?.Data?.Products?.FirstOrDefault(p => string.Equals(p?.ProductName, productName, StringComparison.OrdinalIgnoreCase));
            }
            return null;
        }

        #endregion

        #region Helper Methods
        private Label MakeLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        private Guna2Button MakeButton(string text, EventHandler? onClick = null, int btnSize = 10)
        {
            var btn = new Guna2Button
            {
                Name = "lblName",
                Text = text,
                AutoSize = true,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", btnSize, FontStyle.Bold),
                Margin = new Padding(5)
            };
            if (onClick != null) btn.Click += onClick;
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
            if (onClick != null) btn.Click += onClick;
            return btn;
        }

        private void FocusCashInput()
        {
            txtCash.Focus();
            txtCash.SelectAll();
        }

        private void UpdateSyncButton()
        {
            if (btnSync != null)
            {
                btnSync.Text = $"📶 Sync: {_NotSyncedTotal}";

                if (_NotSyncedTotal > 0)
                {
                    btnSync.FillColor = Color.FromArgb(255, 152, 0);
                }
                else
                {
                    btnSync.FillColor = Color.Green;
                }
            }
        }

        #endregion

        #region Pinned Products

        // In SaleTabControl class

        private void InitializePinnedProducts()
        {
            // Load existing pinned products
            RefreshPinnedProducts(PinnedProductsService.GetPinnedProducts());

            // Subscribe to changes
            PinnedProductsService.PinnedProductsChanged += OnPinnedProductsChanged;
        }

        private void OnPinnedProductsChanged(List<PinnedProductsService.PinnedProduct> products)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => RefreshPinnedProducts(products)));
            }
            else
            {
                RefreshPinnedProducts(products);
            }
        }

        public void RefreshPinnedProducts(List<PinnedProductsService.PinnedProduct> products)
        {
            if (pinnedItemsPanel == null) return;

            // Clear existing pinned products
            pinnedItemsPanel.Controls.Clear();

            // Add all pinned products
            foreach (var product in products)
            {
                CreatePinnedProductButton(product.ProductName, product.Price);
            }
        }

        private void CreatePinnedProductButton(string productName, decimal price)
        {
            var productButton = CreateProductButton(productName);
            var closeButton = CreateCloseButton(productName);

            AttachEventHandlers(productButton, closeButton, productName, price);
            ComposeButtonControls(productButton, closeButton);
        }

        private Guna2Button CreateProductButton(string productName)
        {
            return new Guna2Button
            {
                Text = productName,
                AutoSize = true,
                Size = new Size(140, 40),
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255), // Professional blue
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(5),
                Tag = productName,
                TextAlign = HorizontalAlignment.Left,
                Padding = new Padding(10, 0, 10, 0),
                Cursor = Cursors.Hand
            };
        }

        private Guna2Button CreateCloseButton(string productName)
        {
            return new Guna2Button
            {
                Text = "×", // Multiplication sign for cleaner look
                Size = new Size(20, 20),
                BorderRadius = 10, // Perfect circle
                FillColor = Color.FromArgb(220, 53, 69), // Bootstrap-style danger red
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Tag = productName,
                Cursor = Cursors.Hand
            };
        }

        private void AttachEventHandlers(Guna2Button productButton, Guna2Button closeButton,
            string productName, decimal price)
        {
            // Close button handler - clean and focused
            closeButton.Click += (s, e) => HandleProductUnpin(productName);

            // Product button handler - optimized cart logic
            productButton.Click += (s, e) => HandleProductSelection(productName, price);

            // Maintain close button positioning
            productButton.SizeChanged += (s, e) =>
                PositionCloseButton(productButton, closeButton);
        }

        private void HandleProductUnpin(string productName)
        {
            PinnedProductsService.UnpinProduct(productName);
            // Optional: Add animation or visual feedback here
        }

        private void HandleProductSelection(string productName, decimal price)
        {
            if (!TryIncrementExistingProduct(productName))
            {
                AddProductToCart(productName, price);
            }
            // Optional: Add selection feedback animation
        }

        private void PositionCloseButton(Guna2Button productButton, Guna2Button closeButton)
        {
            closeButton.Location = new Point(
                productButton.Width - closeButton.Width - 5,
                5 // Consistent 5px from top
            );
        }

        private void ComposeButtonControls(Guna2Button productButton, Guna2Button closeButton)
        {
            productButton.Controls.Add(closeButton);
            PositionCloseButton(productButton, closeButton); // Initial positioning
            pinnedItemsPanel.Controls.Add(productButton);
        }
        #endregion

        #region ApiCalling Models
        private void PostSaleApiCall()
        {
            string _authHeader = GetAuthToken();
            string _posToken = GetPosToken();
            PostSaleRecord record = SystemConfigRepository.GetFirstPostSale();
            if (record != null)
            {
                try
                {
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", _authHeader);
                    client.DefaultRequestHeaders.Add("X-PosToken", _posToken);

                    string url = $"{AppConfig.BaseApiUrl}pos/sales/PostSale";
                    var obj = JsonSerializer.Deserialize<PosSale>(record.Value, jsonHelper);

                    var saleJson = JsonSerializer.Serialize(obj);

                    var content = new StringContent(saleJson, System.Text.Encoding.UTF8, "application/json");


                    var response = client.PostAsync(url, content).Result;
                    var responseBody = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Error posting sale: {response.StatusCode}\n{responseBody}", "POS",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    SystemConfigRepository.DeletePostSaleById(record.Id);
                    _NotSyncedTotal = SystemConfigRepository.GetNotSyncedTotal();
                    UpdateSyncButton();
                    MessageBox.Show("Sale posted successfully!", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Exception occurred: {ex.Message}", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }
        private static string GetPosToken()
        {
            string posToken = SystemConfigRepository.GetConfig("LastResumedSession");

            var tokenData = JsonSerializer.Deserialize<ResumeData>(posToken, jsonHelper);
            return tokenData?.PosToken ?? "null";
        }

        private static string GetAuthToken()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\MyCompany\MyApp");
            if (key != null)
            {
                var token = key.GetValue("AuthToken") as string;
                key.Close();
                return token ?? "null";
            }
            return "null";
        }
        #endregion
    }
}