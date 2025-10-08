using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MyApp.UI.Forms;
using MyApp.UI.Models; // Ensure SystemConfigResponse and related models are here

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

        public SignInForm() => InitializeComponent();

        private void InitializeComponent()
        {
            this.Text = "Sign In";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            cardPanel = new Guna2Panel
            {
                BorderRadius = 28,
                FillColor = Color.White,
                Size = new Size(560, 560),
                Anchor = AnchorStyles.None
            };
            this.Controls.Add(cardPanel);

            this.Resize += (s, e) =>
            {
                cardPanel.Left = Math.Max(40, (this.ClientSize.Width - cardPanel.Width) / 2);
                cardPanel.Top = Math.Max(40, (this.ClientSize.Height - cardPanel.Height) / 2);
            };

            layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(36, 28, 36, 28),
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            cardPanel.Controls.Add(layout);

            lblTitle = new Label
            {
                Text = "DOTS",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };
            layout.Controls.Add(lblTitle, 0, 0);

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

            btnSignIn = new Guna2GradientButton
            {
                Text = "Sign In",
                BorderRadius = 18,
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

            this.OnResize(EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(72, 118, 255),
                Color.FromArgb(156, 89, 233),
                45f);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            base.OnPaint(e);
        }

        private async void BtnSignIn_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            // For testing:
            email = "system-admin";
            password = "SysAdmin";

            btnSignIn.Enabled = false;
            btnSignIn.Text = "Signing in...";

            try
            {
                string? token = await AuthenticateAsync(email, password);
                if (!string.IsNullOrEmpty(token))
                {
                    // Create full Authorization header string
                    string authHeader =  token;
                    MessageBox.Show(token);

                    var configData = await LoadSystemConfigAsync(token);
                    if (configData != null)
                    {

                        string json = JsonSerializer.Serialize(configData, new JsonSerializerOptions{ WriteIndented = true });
                        MessageBox.Show(json, "System Config Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Dashboard dashboard = new Dashboard(authHeader); 
                        dashboard.Show();
                        this.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSignIn.Enabled = true;
                btnSignIn.Text = "Sign In";
            }
        }

        private async Task<string?> AuthenticateAsync(string email, string password)
        {
            using var client = new HttpClient { BaseAddress = new Uri("https://dots.optimuzai.com") };
            var payload = new { Username = email, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/api/v1/auth/Authenticate", content);
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Authentication failed:\n" + result);
                return null;
            }

            try
            {
                var doc = JsonDocument.Parse(result);
                if (doc.RootElement.TryGetProperty("Data", out var data) &&
                    data.TryGetProperty("Token", out var token))
                    return token.GetString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading token: " + ex.Message);
            }

            MessageBox.Show("Unable to extract token from response.");
            return null;
        }

        private async Task<SystemConfigResponse?> LoadSystemConfigAsync(string fullAuthHeader)
        {
            using var client = new HttpClient { BaseAddress = new Uri("https://dots.optimuzai.com") };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", fullAuthHeader);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var content = new StringContent("{}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/api/system/common/SystemConfig", content);
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Failed to load SystemConfig:\n{result}");
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<SystemConfigResponse>(result,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing SystemConfig: " + ex.Message);
                return null;
            }
        }
    }
}
