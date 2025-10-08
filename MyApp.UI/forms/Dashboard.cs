using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MyApp.UI.Forms
{
    public class Dashboard : Form
    {
        private static readonly Color BrandPrimary = Color.FromArgb(63, 114, 255);
        private static readonly Color BrandSuccess = Color.FromArgb(46, 204, 113);
        private static readonly Color BrandDanger = Color.FromArgb(231, 76, 60);
        private static readonly Color BrandBackground = Color.FromArgb(247, 249, 252);

        private readonly string _authHeader;

        private Guna2Panel topPanel = null!;
        private Label lblTitle = null!;
        private Label lblCostCenter = null!;
        private TableLayoutPanel root = null!;
        private TableLayoutPanel actionBar = null!;
        private FlowLayoutPanel leftActions = null!;
        private FlowLayoutPanel rightSearch = null!;
        private Guna2TextBox txtSearch = null!;
        private Guna2Button btnSearch = null!;
        private Guna2Button btnRefresh = null!;
        private Guna2Button btnAdd = null!;
        private Guna2DataGridView dgvSessions = null!;

        private int actionColumnIndex = -1;
        private int hoverRowIndex = -1;
        private ActionButtonPart hoverPart = ActionButtonPart.None;

        private readonly List<SaleSessionView> allSessions = new();
        private readonly BindingList<SaleSessionView> visibleSessions = new();

        private enum ActionButtonPart { None, Resume, Delete }

        public Dashboard(string authHeader)
        {
            _authHeader = authHeader;
            InitializeComponent();
            _ = LoadSessionsAsync();
        }

        private void InitializeComponent()
        {
            AutoScaleMode = AutoScaleMode.Dpi;
            Text = "Dashboard";
            WindowState = FormWindowState.Maximized;
            BackColor = BrandBackground;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // ========== ROOT ==========
            root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = BrandBackground,
                ColumnCount = 1,
                RowCount = 3
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72f));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // ========== TOP BAR ==========
            topPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = BrandPrimary,
                Padding = new Padding(24, 12, 24, 12),
                ShadowDecoration = { Enabled = true, Depth = 6 }
            };

            lblTitle = new Label
            {
                Text = "🧾 Sales Dashboard",
                Font = new Font("Segoe UI Semibold", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Left
            };

            lblCostCenter = new Label
            {
                Text = "Cost Center: 001",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight
            };

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblCostCenter);

            // ========== ACTION BAR ==========
            actionBar = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ColumnCount = 3,
                Padding = new Padding(20, 12, 20, 12),
            };
            actionBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            actionBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            actionBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            leftActions = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };
            rightSearch = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };

            btnAdd = CreateButton("➕ Add Session", BrandPrimary, BtnAdd_Click);
            btnRefresh = CreateButton("🔄 Refresh", BrandSuccess, BtnRefresh_Click);
            btnSearch = CreateButton("🔍 Search", BrandPrimary, BtnSearch_Click);

            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search by Channel or State...",
                Font = new Font("Segoe UI", 10),
                Width = 280,
                Height = 40,
                BorderRadius = 10,
                BorderColor = Color.LightGray,
                Margin = new Padding(0, 0, 10, 0),
               // IconLeft = Properties.Resources.search // Optional: if you have an icon resource
            };

            leftActions.Controls.Add(btnAdd);
            leftActions.Controls.Add(btnRefresh);
            rightSearch.Controls.Add(txtSearch);
            rightSearch.Controls.Add(btnSearch);

            actionBar.Controls.Add(leftActions, 0, 0);
            actionBar.Controls.Add(new Panel(), 1, 0);
            actionBar.Controls.Add(rightSearch, 2, 0);

            // ========== GRID ==========
            dgvSessions = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 225, 230)
            };

            dgvSessions.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BrandPrimary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(0, 10, 0, 10)
            };
            dgvSessions.ColumnHeadersHeight = 56;
            dgvSessions.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10),
                SelectionBackColor = Color.FromArgb(235, 242, 255),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvSessions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            dgvSessions.RowTemplate.Height = 44;

            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Channel", DataPropertyName = "Channel", FillWeight = 25 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Start Time", DataPropertyName = "StartTime", FillWeight = 25 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "End Time", DataPropertyName = "EndTime", FillWeight = 25 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Session State", DataPropertyName = "SessionState", FillWeight = 15 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Actions", ReadOnly = true, FillWeight = 10 });

            var actionCol = dgvSessions.Columns["Actions"];
            actionColumnIndex = actionCol?.Index ?? dgvSessions.Columns.Count - 1;

            dgvSessions.DataSource = visibleSessions;

            dgvSessions.CellPainting += DgvSessions_CellPainting;
            dgvSessions.CellMouseClick += DgvSessions_CellMouseClick;
            dgvSessions.CellMouseMove += DgvSessions_CellMouseMove;
            dgvSessions.CellMouseLeave += DgvSessions_CellMouseLeave;

            root.Controls.Add(topPanel, 0, 0);
            root.Controls.Add(actionBar, 0, 1);
            root.Controls.Add(dgvSessions, 0, 2);
            Controls.Add(root);
        }

        private Guna2Button CreateButton(string text, Color color, EventHandler onClick)
        {
            return new Guna2Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                BorderRadius = 8,
                FillColor = color,
                ForeColor = Color.White,
                Height = 40,
                Margin = new Padding(0, 0, 10, 0),
                Padding = new Padding(14, 0, 14, 0),
               // Click = onClick
            };
        }

        // ================== DATA LOAD ==================
        private async Task LoadSessionsAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", _authHeader);

                var url = "https://dots.optimuzai.com/api/l2a/sales/sale-sessions/GetAll?PerPage=20&Page=0";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(json, options);

                allSessions.Clear();

                if (apiResponse?.Data?.Records != null)
                {
                    foreach (var rec in apiResponse.Data.Records)
                    {
                        allSessions.Add(new SaleSessionView
                        {
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
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadSessionsAsync();
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string q = (txtSearch.Text ?? "").Trim();
            if (string.IsNullOrEmpty(q))
            {
                ResetVisibleSessions(allSessions);
                return;
            }

            var filtered = allSessions
                .Where(s => (s.Channel ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (s.SessionState ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            ResetVisibleSessions(filtered);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Add new session clicked!");
        }

        // ================== GRID BUTTONS ==================
        private void DgvSessions_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != actionColumnIndex) return;

            e.PaintBackground(e.CellBounds, true);
            var rects = GetActionButtonRects(e.CellBounds);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            bool isHovered = e.RowIndex == hoverRowIndex;
            var resumeColor = isHovered && hoverPart == ActionButtonPart.Resume ? Lighten(BrandSuccess, 0.1f) : BrandSuccess;
            var deleteColor = isHovered && hoverPart == ActionButtonPart.Delete ? Lighten(BrandDanger, 0.08f) : BrandDanger;

            using var resumeBrush = new SolidBrush(resumeColor);
            using var deleteBrush = new SolidBrush(deleteColor);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            g.FillRoundedRectangle(resumeBrush, rects.Item1, 8);
            g.DrawString("Resume", e.CellStyle.Font, Brushes.White, rects.Item1, sf);
            g.FillRoundedRectangle(deleteBrush, rects.Item2, 8);
            g.DrawString("Delete", e.CellStyle.Font, Brushes.White, rects.Item2, sf);

            e.Handled = true;
        }

        private void DgvSessions_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != actionColumnIndex) return;
            MessageBox.Show("Action button clicked!");
        }

        private void DgvSessions_CellMouseMove(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != actionColumnIndex)
            {
                hoverRowIndex = -1;
                hoverPart = ActionButtonPart.None;
                dgvSessions.Cursor = Cursors.Default;
                return;
            }

            Rectangle cellRect = dgvSessions.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Point local = new Point(e.X - cellRect.X, e.Y - cellRect.Y);
            var rects = GetActionButtonRects(new Rectangle(Point.Empty, cellRect.Size));

            ActionButtonPart part = rects.Item1.Contains(local)
                ? ActionButtonPart.Resume
                : rects.Item2.Contains(local)
                    ? ActionButtonPart.Delete
                    : ActionButtonPart.None;

            if (hoverRowIndex != e.RowIndex || hoverPart != part)
            {
                hoverRowIndex = e.RowIndex;
                hoverPart = part;
                dgvSessions.Cursor = part == ActionButtonPart.None ? Cursors.Default : Cursors.Hand;
                dgvSessions.Invalidate(cellRect);
            }
        }

        private void DgvSessions_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            hoverRowIndex = -1;
            hoverPart = ActionButtonPart.None;
            dgvSessions.Cursor = Cursors.Default;
        }

        private static Tuple<Rectangle, Rectangle> GetActionButtonRects(Rectangle cellBounds)
        {
            const int padding = 6, gap = 8;
            int h = cellBounds.Height - (padding * 2);
            int w = (cellBounds.Width - (padding * 2) - gap) / 2;
            var rect1 = new Rectangle(cellBounds.Left + padding, cellBounds.Top + padding, w, h);
            var rect2 = new Rectangle(rect1.Right + gap, cellBounds.Top + padding, w, h);
            return Tuple.Create(rect1, rect2);
        }

        private static Color Lighten(Color c, float a)
        {
            int r = c.R + (int)((255 - c.R) * a);
            int g = c.G + (int)((255 - c.G) * a);
            int b = c.B + (int)((255 - c.B) * a);
            return Color.FromArgb(r, g, b);
        }

        // ================== MODELS ==================
        private class ApiResponse { public ApiData? Data { get; set; } }
        private class ApiData { public List<SaleSessionRecord>? Records { get; set; } }
        private class SaleSessionRecord
        {
            public string SaleSessionId { get; set; } = "";
            public long StartTime { get; set; }
            public long? EndTime { get; set; }
            public string SessionState { get; set; } = "";
            public SaleChannel? SaleChannel { get; set; }
        }
        private class SaleChannel { public string SaleChannelName { get; set; } = ""; }
        private sealed class SaleSessionView
        {
            public string Channel { get; set; } = "";
            public string StartTime { get; set; } = "";
            public string EndTime { get; set; } = "";
            public string SessionState { get; set; } = "";
        }
    }

    // 🔧 Helper Extension for Rounded Rectangles
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
                path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
                path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }
    }
}
