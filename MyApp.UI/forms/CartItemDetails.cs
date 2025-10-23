using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MyApp.UI.Forms
{
    public class CustomAmountForm : Form
    {
        private Guna2TextBox txtCustomAmount;
        private Guna2TextBox txtSalePrice;
        private Guna2TextBox txtQuantity;
        private Guna2TextBox txtDiscount;
        private Guna2Button btnUpdate;
        private Guna2Button btnDiscard;

        // === Public Properties (now read/write) ===
        public decimal CustomAmount
        {
            get => decimal.TryParse(txtCustomAmount.Text, out var v) ? v : 0;
            set => txtCustomAmount.Text = value.ToString("0.##");
        }

        public decimal SalePrice
        {
            get => decimal.TryParse(txtSalePrice.Text, out var v) ? v : 0;
            set => txtSalePrice.Text = value.ToString("0.##");
        }

        public int Quantity
        {
            get => int.TryParse(txtQuantity.Text, out var v) ? v : 0;
            set => txtQuantity.Text = value.ToString();
        }

        public decimal Discount
        {
            get => decimal.TryParse(txtDiscount.Text, out var v) ? v : 0;
            set => txtDiscount.Text = value.ToString("0.##");
        }

        public CustomAmountForm()
        {
            // === WINDOW SETTINGS ===
            Text = "Custom Amount";
            Size = new Size(420, 420);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // === LAYOUT PANEL ===
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25, 20, 25, 20),
                RowCount = 2,
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            Controls.Add(layout);

            // === INPUT AREA ===
            var inputPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(0, 0, 0, 10)
            };
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            inputPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            layout.Controls.Add(inputPanel, 0, 0);

            // === TEXT FIELDS ===
            txtCustomAmount = CreateInput("Custom Amount");
            txtSalePrice = CreateInput("Sale Price");
            txtQuantity = CreateInput("Quantity");
            txtDiscount = CreateInput("Discount (+/-)");

            inputPanel.Controls.Add(txtCustomAmount);
            inputPanel.Controls.Add(txtSalePrice);
            inputPanel.Controls.Add(txtQuantity);
            inputPanel.Controls.Add(txtDiscount);

            // === BUTTONS PANEL ===
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 10, 0, 0)
            };
            layout.Controls.Add(buttonPanel, 0, 1);

            // === BUTTONS ===
            btnUpdate = new Guna2Button
            {
                Text = "Update",
                Width = 120,
                Height = 40,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Margin = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnUpdate.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

            btnDiscard = new Guna2Button
            {
                Text = "Discard",
                Width = 120,
                Height = 40,
                BorderRadius = 8,
                FillColor = Color.LightGray,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black,
                Margin = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnDiscard.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            buttonPanel.Controls.Add(btnUpdate);
            buttonPanel.Controls.Add(btnDiscard);
        }

        // === HELPER: TEXTBOX CREATION ===
        private Guna2TextBox CreateInput(string placeholder)
        {
            return new Guna2TextBox
            {
                PlaceholderText = placeholder,
                BorderRadius = 8,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Margin = new Padding(0, 5, 0, 5),
                BorderThickness = 1,
                BorderColor = Color.FromArgb(220, 225, 230),
                HoverState = { BorderColor = Color.DodgerBlue },
                FocusedState = { BorderColor = Color.DodgerBlue },
            };
        }
    }
}
