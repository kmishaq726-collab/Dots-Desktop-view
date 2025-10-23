using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Microsoft.Win32;
using MyApp.Common;
using MyApp.Models;
using MyApp.UI.Data;

namespace MyApp.UI.Forms
{
    public class Dashboard : Form
    {
        private static readonly Color BrandPrimary = Color.FromArgb(72, 118, 255);
        private static readonly Color BrandSuccess = Color.FromArgb(72, 255, 118);
        private static readonly Color BrandDanger = Color.FromArgb(255, 72, 118);
        private static readonly Color BrandBackground = Color.White;

        private readonly string _authHeader;

        private Guna2Panel topPanel = null!;
        private Label lblTitle = null!;
        private Label lblCostCenter = null!;
        private Guna2Button btnLogout = null!;

        private FlowLayoutPanel actionPanel = null!;
        private Guna2TextBox txtSearch = null!;
        private Guna2Button btnSearch = null!;
        private Guna2Button btnRefresh = null!;
        private Guna2Button btnAdd = null!;

        private Guna2DataGridView dgvSessions = null!;
        private int actionColumnIndex = -1;

        private readonly List<SaleSessionView> allSessions = new();
        private readonly BindingList<SaleSessionView> visibleSessions = new();

        private enum ActionButtonPart { None, Resume, Delete }
        private int hoverRowIndex = -1;
        private ActionButtonPart hoverPart = ActionButtonPart.None;

        public Dashboard()
        {
            _authHeader = GetAuthToken();
            InitializeComponent();
            _ = LoadSessionsAsync();
        }

        private void InitializeComponent()
        {
            if (string.IsNullOrEmpty(_authHeader) || _authHeader == "null")
            {
                this.Tag = "SignOut";
                this.Shown += (s, e) => this.Close();
                return;
            }

            this.Text = "Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = BrandBackground;
            this.Font = new Font("Segoe UI", 10);

            // === Top Panel ===
            topPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FillColor = BrandPrimary,
                Padding = new Padding(15, 10, 15, 10),
                ShadowDecoration = { Enabled = true, Depth = 6 }
            };

            var labelPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                Width = 300
            };
            labelPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            labelPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

            lblTitle = new Label
            {
                Text = "Sales Sessions",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Fill,
                BackColor = BrandPrimary,
                TextAlign = ContentAlignment.BottomLeft
            };

            lblCostCenter = new Label
            {
                Text = "Cost Center 001",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Fill,
                BackColor = BrandPrimary,
                TextAlign = ContentAlignment.TopLeft
            };

            labelPanel.Controls.Add(lblTitle, 0, 0);
            labelPanel.Controls.Add(lblCostCenter, 0, 1);

            btnLogout = new Guna2Button
            {
                Text = "Logout",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = Color.Red,
                AutoSize = true,
                Dock = DockStyle.Right,
                BorderRadius = 8,
                Margin = new Padding(5, 10, 5, 10),
                BackColor = BrandPrimary
            };
            btnLogout.Click += BtnLogout_Click;

            topPanel.Controls.Add(btnLogout);
            topPanel.Controls.Add(labelPanel);

            // === Action Panel ===
            actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = Color.WhiteSmoke,
                AutoSize = true,
                WrapContents = false
            };

            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search",
                Font = new Font("Segoe UI", 10),
                Width = 220,
                Height = 36,
                BorderRadius = 8,
                Margin = new Padding(6)
            };

            btnSearch = CreateButton("Search", BrandPrimary, BtnSearch_Click);
            btnRefresh = CreateButton("Refresh", BrandPrimary, async (s, e) => await LoadSessionsAsync());
            btnAdd = CreateButton("Select Customer", BrandPrimary, BtnAdd_Click);

            actionPanel.Controls.Add(txtSearch);
            actionPanel.Controls.Add(btnSearch);
            actionPanel.Controls.Add(btnRefresh);
            actionPanel.Controls.Add(btnAdd);

            // === DataGridView ===
            dgvSessions = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.DeepPurple,
                AutoGenerateColumns = false,
                RowTemplate = { Height = 45 }
            };

            dgvSessions.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            dgvSessions.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvSessions.ThemeStyle.HeaderStyle.BackColor = BrandPrimary;
            dgvSessions.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvSessions.ColumnHeadersHeight = 55;
            dgvSessions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvSessions.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSessions.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            dgvSessions.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 12);
            dgvSessions.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvSessions.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvSessions.ThemeStyle.RowsStyle.SelectionBackColor = Color.LightBlue;

            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SessionId", DataPropertyName = "SaleSessionId" });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Channel", DataPropertyName = "Channel" });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Start Time", DataPropertyName = "StartTime" });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "End Time", DataPropertyName = "EndTime" });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Session State", DataPropertyName = "SessionState" });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Actions" });

            dgvSessions.DataSource = visibleSessions;
            actionColumnIndex = dgvSessions.Columns.Count - 1;

            dgvSessions.CellPainting += DgvSessions_CellPainting;
            dgvSessions.CellMouseClick += DgvSessions_CellMouseClick;
            dgvSessions.CellMouseMove += DgvSessions_CellMouseMove;
            dgvSessions.CellMouseLeave += DgvSessions_CellMouseLeave;

            // === Add Controls ===
            this.Controls.Add(dgvSessions);
            this.Controls.Add(actionPanel);
            this.Controls.Add(topPanel);
        }

        private Guna2Button CreateButton(string text, Color color, EventHandler onClick)
        {
            var btn = new Guna2Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                BorderRadius = 8,
                FillColor = color,
                ForeColor = Color.White,
                Margin = new Padding(6)
            };
            btn.Click += onClick;
            return btn;
        }

        private async Task LoadSessionsAsync()
        {
            if (_authHeader == null)
            {
                return;

            }
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", _authHeader);
                var url = $"{AppConfig.BaseApiUrl}l2a/sales/sale-sessions/GetAll?PerPage=20&Page=0";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();



                //MessageBox.Show(json);
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                SystemConfigRepository.SaveConfig("SaleSessions", apiResponse);

                allSessions.Clear();
                if (apiResponse?.Data?.Records != null)
                {
                    foreach (var rec in apiResponse.Data.Records)
                    {
                        allSessions.Add(new SaleSessionView
                        {
                            SaleSessionId = rec.SaleSessionId,
                            Channel = rec.SaleChannel?.SaleChannelName ?? "N/A",
                            StartTime = UnixToDate(rec.StartTime),
                            EndTime = rec.EndTime.HasValue ? UnixToDate(rec.EndTime.Value) : "N/A",
                            SessionState = rec.SessionState
                        });
                    }
                }

                ResetVisibleSessions(allSessions);
            }
            catch (Exception ex)
            {
                string json = SystemConfigRepository.GetConfig("SaleSessions");

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                SystemConfigRepository.SaveConfig("SaleSessions", apiResponse);

                allSessions.Clear();
                if (apiResponse?.Data?.Records != null)
                {
                    foreach (var rec in apiResponse.Data.Records)
                    {
                        allSessions.Add(new SaleSessionView
                        {
                            SaleSessionId = rec.SaleSessionId,
                            Channel = rec.SaleChannel?.SaleChannelName ?? "N/A",
                            StartTime = UnixToDate(rec.StartTime),
                            EndTime = rec.EndTime.HasValue ? UnixToDate(rec.EndTime.Value) : "N/A",
                            SessionState = rec.SessionState
                        });
                    }
                }

                ResetVisibleSessions(allSessions);

                if (allSessions == null)
                { MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private static string UnixToDate(long unix)
        {
            var dt = DateTimeOffset.FromUnixTimeSeconds(unix).ToLocalTime().DateTime;
            return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
        }

        private void ResetVisibleSessions(IEnumerable<SaleSessionView> items)
        {
            visibleSessions.RaiseListChangedEvents = false;
            visibleSessions.Clear();
            foreach (var it in items)
                visibleSessions.Add(it);
            visibleSessions.RaiseListChangedEvents = true;
            visibleSessions.ResetBindings();
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string q = txtSearch.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(q))
            {
                ResetVisibleSessions(allSessions);
                return;
            }

            var filtered = allSessions.FindAll(s =>
                (s.Channel ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (s.SessionState ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
            ResetVisibleSessions(filtered);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Select Customer clicked!");
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\MyCompany\MyApp", true);
            if (key != null)
            {
                key.DeleteValue("AuthToken", false);
                key.Close();
            }
            this.Tag = "SignOut";
            this.Close();
        }

        // ================== GRID BUTTONS ==================
        private void DgvSessions_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != actionColumnIndex) return;

            e.PaintBackground(e.CellBounds, true);
            var (resumeRect, deleteRect) = GetActionButtonRects(e.CellBounds);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var resumeBrush = new SolidBrush(BrandSuccess);
            var deleteBrush = new SolidBrush(BrandDanger);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            g.FillRoundedRectangle(resumeBrush, resumeRect, 8);
            g.DrawString("Resume", e.CellStyle.Font, Brushes.White, resumeRect, sf);

            g.FillRoundedRectangle(deleteBrush, deleteRect, 8);
            g.DrawString("Delete", e.CellStyle.Font, Brushes.White, deleteRect, sf);

            e.Handled = true;
        }

        async private void DgvSessions_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != actionColumnIndex) return;

            int buttonWidth = dgvSessions.Columns[e.ColumnIndex].Width / 2;
            var selectedSession = visibleSessions[e.RowIndex];

            if (e.Location.X < buttonWidth)
            {
                await ResumeSessionAsync(selectedSession.SaleSessionId);
            }
            else
            {
                MessageBox.Show($"Delete clicked for row {e.RowIndex}");
            }
        }

        private void DgvSessions_CellMouseMove(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != actionColumnIndex)
            {
                dgvSessions.Cursor = Cursors.Default;
                return;
            }

            Rectangle cellRect = dgvSessions.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Point local = new Point(e.X - cellRect.X, e.Y - cellRect.Y);
            var (resumeRect, deleteRect) = GetActionButtonRects(new Rectangle(Point.Empty, cellRect.Size));

            ActionButtonPart part = resumeRect.Contains(local)
                ? ActionButtonPart.Resume
                : deleteRect.Contains(local)
                    ? ActionButtonPart.Delete
                    : ActionButtonPart.None;

            dgvSessions.Cursor = part == ActionButtonPart.None ? Cursors.Default : Cursors.Hand;
        }

        private void DgvSessions_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            dgvSessions.Cursor = Cursors.Default;
        }

        private static (Rectangle, Rectangle) GetActionButtonRects(Rectangle cellBounds)
        {
            int padding = 6, gap = 8;
            int h = cellBounds.Height - (padding * 2);
            int w = (cellBounds.Width - (padding * 2) - gap) / 2;
            var rect1 = new Rectangle(cellBounds.Left + padding, cellBounds.Top + padding, w, h);
            var rect2 = new Rectangle(rect1.Right + gap, cellBounds.Top + padding, w, h);
            return (rect1, rect2);
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

        private async Task ResumeSessionAsync(string saleSessionId)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", _authHeader);

                string url = $"{AppConfig.BaseApiUrl}pos/sessions/ResumeSession";

                // ✅ Payload with the session ID
                var payload = new
                {
                    SaleSessionId = saleSessionId
                };

                string jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                // ✅ Send POST request
                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Resume failed ({response.StatusCode}):\n{responseBody}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ Parse and save API response
                var apiResponse = JsonSerializer.Deserialize<ApiResponse_ResumeSession>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data != null)
                {
                    var extendedData = new
                    {
                        SaleSessionId = saleSessionId,
                        apiResponse?.Data.Pk,
                        apiResponse?.Data.PosToken,
                    };

                    // ✅ Serialize and save to database
                    SystemConfigRepository.SaveConfig("LastResumedSession", extendedData);

                    SystemConfigRepository.SaveConfig("SaleSessionId", saleSessionId);
                    SystemConfigRepository.SaveConfig("PosToken", apiResponse?.Data.PosToken);

                    // ✅ Navigate to SaleScreenForm
                    this.Tag = "Sale";
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show("Not allowed to resume this session.",
                        "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string data = SystemConfigRepository.GetConfig("LastResumedSession");

                var resumeData = JsonSerializer.Deserialize<ResumeData>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resumeData != null)
                {
                    if (resumeData.SaleSessionId == saleSessionId)
                    {
                        this.Tag = "Sale";
                        this.Close();
                        return;
                    }
                }

                MessageBox.Show($"Error while resuming session:\n{ex.Message}",
                    "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }

}
