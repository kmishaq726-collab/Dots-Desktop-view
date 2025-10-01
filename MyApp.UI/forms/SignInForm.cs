using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MyApp.UI.Forms;

namespace MyApp.UI
{
    public class SignInForm : Form
    {
        private Guna2Panel cardPanel = null!;
        private TableLayoutPanel layout = null!;
        private Guna2TextBox txtEmail = null!;
        private Guna2TextBox txtPassword = null!;
        private Guna2GradientButton btnSignIn = null!;
        private Label lblTitle = null!;
        private LinkLabel linkRegister = null!;

        public SignInForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ==== Form Settings ====
            this.Text = "Sign In";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable; // standard title bar
            this.WindowState = FormWindowState.Normal;
            this.BackColor = Color.White; // fallback color

            // ==== Card Panel ====
            cardPanel = new Guna2Panel
            {
                BorderRadius = 28,
                FillColor = Color.White,
                Size = new Size(560, 560),
                Anchor = AnchorStyles.None
            };
            this.Controls.Add(cardPanel);

            // Center card on resize
            this.Resize += (s, e) =>
            {
                cardPanel.Left = Math.Max(40, (this.ClientSize.Width - cardPanel.Width) / 2);
                cardPanel.Top = Math.Max(40, (this.ClientSize.Height - cardPanel.Height) / 2);
            };

            // ==== Layout inside Card ====
            layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(36, 28, 36, 28),
                ColumnCount = 1,
                RowCount = 6,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));  // Email
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));  // Password
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));  // Button
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Spacer
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));  // Register
            cardPanel.Controls.Add(layout);

            // ==== Title ====
            lblTitle = new Label
            {
                Text = "DOTS",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };
            layout.Controls.Add(lblTitle, 0, 0);

            // ==== Email ====
            txtEmail = new Guna2TextBox
            {
                PlaceholderText = "Email address",
                BorderRadius = 14,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(220, 220, 220),
                FillColor = Color.FromArgb(250, 250, 250),
                Font = new Font("Segoe UI", 12),
                Dock = DockStyle.Fill,
                Margin = new Padding(6)
            };
            txtEmail.FocusedState.BorderColor = Color.FromArgb(72, 118, 255);
            layout.Controls.Add(txtEmail, 0, 1);

            // ==== Password ====
            txtPassword = new Guna2TextBox
            {
                PlaceholderText = "Password",
                BorderRadius = 14,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(220, 220, 220),
                FillColor = Color.FromArgb(250, 250, 250),
                Font = new Font("Segoe UI", 12),
                UseSystemPasswordChar = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(6)
            };
            txtPassword.FocusedState.BorderColor = Color.FromArgb(72, 118, 255);
            layout.Controls.Add(txtPassword, 0, 2);

            // ==== Sign In Button ====
            btnSignIn = new Guna2GradientButton
            {
                Text = "Sign In",
                BorderRadius = 18,
                Animated = false,
                FillColor = Color.FromArgb(72, 118, 255),
                FillColor2 = Color.FromArgb(156, 89, 233),
                Font = new Font("Segoe UI Semibold", 14),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                Margin = new Padding(6)
            };
            btnSignIn.HoverState.FillColor = Color.FromArgb(58, 98, 230);
            btnSignIn.Click += BtnSignIn_Click;
            layout.Controls.Add(btnSignIn, 0, 3);

            // Center card initially
            this.OnResize(EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // static gradient background
            using var brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(72, 118, 255),
                Color.FromArgb(156, 89, 233),
                45f);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            base.OnPaint(e);
        }

        private void BtnSignIn_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both Email and Password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Dashboard dashboard = new Dashboard();
            dashboard.Show();

            this.Hide(); // hides SignInForm
        }
    }
}