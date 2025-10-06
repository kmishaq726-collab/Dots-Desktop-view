using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Enums;

namespace MyApp.UI.Forms
{
    public class Dashboard : Form
    {
        private Guna2Panel topPanel = null!;
        private Label lblTitle = null!;
        private Label lblCostCenter = null!;
        private FlowLayoutPanel actionPanel = null!;
        private Guna2TextBox txtSearch = null!;
        private Guna2Button btnSearch = null!;
        private Guna2Button btnRefresh = null!;
        private Guna2Button btnAdd = null!;
        private Guna2DataGridView dgvSessions = null!;
        private Guna2Button btnLogout = null!;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // === Form ===
            this.Text = "Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // === Top Panel ===
            topPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FillColor = Color.FromArgb(72, 118, 255),
                Padding = new Padding(15, 10, 15, 10),
                ShadowDecoration = { Enabled = true, Depth = 6 }
            };

            // Labels container
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

            // Title Label
            lblTitle = new Label
            {
                Text = "Sale Sessions",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(72, 118, 255),
                TextAlign = ContentAlignment.BottomLeft
            };

            // Cost Center Label
            lblCostCenter = new Label
            {
                Text = "Cost Center 001",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(72, 118, 255),
                TextAlign = ContentAlignment.TopLeft
            };

            // Add labels
            labelPanel.Controls.Add(lblTitle, 0, 0);
            labelPanel.Controls.Add(lblCostCenter, 0, 1);

            // === Logout Button ===
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
                BackColor = Color.FromArgb(72, 118, 255)
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

            // === Search TextBox ===
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search",
                Font = new Font("Segoe UI", 10),
                Width = 220,
                Height = 36,
                BorderRadius = 8,
                Margin = new Padding(6)
                
            };

            // === Search Button ===
            btnSearch = new Guna2Button
            {
                Text = "Search",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White,
                Margin = new Padding(6)
            };
            btnSearch.Click += BtnSearch_Click;

            // === Refresh Button ===
            btnRefresh = new Guna2Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White,
                Margin = new Padding(6)
            };
            btnRefresh.Click += BtnRefresh_Click;

            // === Add Button (Select Customer) ===
            btnAdd = new Guna2Button
            {
                Text = "Select Customer",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                ForeColor = Color.White,
                Margin = new Padding(6)
            };
            btnAdd.Click += BtnAdd_Click;

            // Add controls to panel
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
                Theme = DataGridViewPresetThemes.DeepPurple,
            };

            dgvSessions.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvSessions.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvSessions.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(72, 118, 255);
            dgvSessions.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvSessions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvSessions.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 10);
            dgvSessions.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvSessions.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvSessions.ThemeStyle.RowsStyle.SelectionBackColor = Color.LightBlue;
            dgvSessions.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Add columns
            dgvSessions.Columns.Add("Channel", "Channel");
            dgvSessions.Columns.Add("StartTime", "Start Time");
            dgvSessions.Columns.Add("EndTime", "End Time");
            dgvSessions.Columns.Add("SessionState", "Session State");

            var colAction = new DataGridViewButtonColumn
            {
                HeaderText = "Actions",
                Name = "Action",
                UseColumnTextForButtonValue = false
            };
            dgvSessions.Columns.Add(colAction);

            // Dummy data
            dgvSessions.Rows.Add("Default", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), "N/A", "In Progress");
            dgvSessions.Rows.Add("Default2", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), "N/A", "Progress");

            foreach (DataGridViewRow row in dgvSessions.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                row.DefaultCellStyle.SelectionForeColor = Color.Black;
            }

            dgvSessions.CellPainting += DgvSessions_CellPainting;
            dgvSessions.CellMouseClick += DgvSessions_CellMouseClick;

            // === Add Controls to Form ===
            this.Controls.Add(dgvSessions);
            this.Controls.Add(actionPanel);
            this.Controls.Add(topPanel);
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            var signInForm = new SignInForm();
            this.Hide();
            signInForm.ShowDialog();
            this.Close();
        }

        private void DgvSessions_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == dgvSessions.Columns["Action"].Index && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                int buttonWidth = e.CellBounds.Width / 2 - 6;
                int buttonHeight = e.CellBounds.Height - 8;
                Rectangle resumeRect = new Rectangle(e.CellBounds.Left + 4, e.CellBounds.Top + 4, buttonWidth, buttonHeight);
                Rectangle deleteRect = new Rectangle(e.CellBounds.Left + buttonWidth + 8, e.CellBounds.Top + 4, buttonWidth, buttonHeight);

                using (SolidBrush resumeBrush = new SolidBrush(Color.FromArgb(72, 255, 118)))
                using (SolidBrush deleteBrush = new SolidBrush(Color.FromArgb(255, 72, 118)))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.FillRectangle(resumeBrush, resumeRect);
                    e.Graphics.DrawString("Resume", e.CellStyle.Font, Brushes.White, resumeRect, sf);

                    e.Graphics.FillRectangle(deleteBrush, deleteRect);
                    e.Graphics.DrawString("Delete", e.CellStyle.Font, Brushes.White, deleteRect, sf);
                }

                e.Handled = true;
            }
        }

        private void DgvSessions_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == dgvSessions.Columns["Action"].Index && e.RowIndex >= 0)
            {
                int buttonWidth = dgvSessions.Columns[e.ColumnIndex].Width / 2;

                if (e.Location.X < buttonWidth)
                {
                    var saleForm = new SaleScreenForm();
                    this.Hide();
                    saleForm.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Delete clicked for row " + e.RowIndex);
                }
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e) =>
            MessageBox.Show("Search: " + txtSearch.Text);

        private void BtnRefresh_Click(object? sender, EventArgs e) =>
            MessageBox.Show("Refreshing sessions...");

        private void BtnAdd_Click(object? sender, EventArgs e) =>
            MessageBox.Show("Select Customer clicked!");
    }
}
