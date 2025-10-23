using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Microsoft.Win32;
using MyApp.Common;
using MyApp.Models;
using MyApp.UI.Controls;
using MyApp.UI.Data;
using static MyApp.UI.Services.PinnedProductsService;

namespace MyApp.UI.Forms
{
    public class SaleScreenForm : Form
    {
        private FlowLayoutPanel saleTabsPanel;
        private Panel tabContentHost;
        private Guna2Button btnAddTab, btnRemoveTab;
        private readonly List<Guna2Button> activeTabs = new();
        private readonly Dictionary<Guna2Button, SaleTabControl> tabContents = new();
        private const int MaxTabs = 5;

        public SaleScreenForm()
        {
            InitializeUI();
            GetPosToken();
            GetPinnedProducts();
        }
        
        private void GetPinnedProducts()
        {
           string getPinnedProduct = SystemConfigRepository.GetConfig("_pinnedProducts");
            if (getPinnedProduct != null)
            {
                var pinnedProduct = JsonSerializer.Deserialize<List<PinnedProduct>>(getPinnedProduct, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                PinSavedProducts(pinnedProduct);
            } 
        }

        private async Task InitializeUI()
        {
        
            await SaveSaleSessionInfo();
            Text = "POS Sale Screen";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;
            MinimumSize = new Size(1200, 700);

            // ----- Main Layout -----
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(layout);

            // ----- Tabs Header Bar -----
            var headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(10, 8, 10, 8)
            };
            layout.Controls.Add(headerPanel, 0, 0);

            // container that holds tabs (left) and control buttons (right)
            var headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            headerPanel.Controls.Add(headerLayout);

            // ----- Tabs Panel (Left) -----
            saleTabsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0),
                BackColor = Color.Transparent,
                Margin = new Padding(0),

            };
            headerLayout.Controls.Add(saleTabsPanel, 0, 0);

            // ----- Control Buttons (Right) -----
            var controlButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            btnAddTab = MakeControlButton("+", AddNewTab, Color.SeaGreen);
            btnRemoveTab = MakeControlButton("−", RemoveActiveTab, Color.Crimson);
            controlButtonsPanel.Controls.AddRange(new Control[] { btnAddTab, btnRemoveTab });
            headerLayout.Controls.Add(controlButtonsPanel, 1, 0);


            // ----- Tab Content Host (Main Area) -----
            tabContentHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            layout.Controls.Add(tabContentHost, 0, 1);

            // Start with one tab
            AddNewTab(null, null);
        }

        // ---------------- TAB MANAGEMENT ---------------- //
        private void AddNewTab(object? sender, EventArgs? e)
        {
            if (activeTabs.Count >= MaxTabs)
            {
                MessageBox.Show("Maximum 5 tabs allowed.", "Info");
                return;
            }

            int newIndex = activeTabs.Count + 1;
            var tab = new Guna2Button
            {
                Text = $"Tab {newIndex}",
                Width = 120,
                Height = 35,
                BorderRadius = 8,
                FillColor = Color.FromArgb(72, 118, 255),
                HoverState = { FillColor = Color.FromArgb(50, 90, 230) },
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5, 0, 5, 0),
                ShadowDecoration = { Enabled = true, Shadow = new Padding(2) }
            };
            tab.Click += (s, ev) => ActivateTab(tab);
            saleTabsPanel.Controls.Add(tab);
            activeTabs.Add(tab);

            var saleTab = new SaleTabControl();
            tabContents[tab] = saleTab;

            ActivateTab(tab);
        }

        private void RemoveActiveTab(object? sender, EventArgs? e)
        {
            if (activeTabs.Count <= 1)
            {
                MessageBox.Show("At least one tab must remain open.", "Info");
                return;
            }

            var active = activeTabs.FirstOrDefault(t => t.FillColor == Color.DodgerBlue);
            if (active != null)
            {
                saleTabsPanel.Controls.Remove(active);
                activeTabs.Remove(active);
                tabContents.Remove(active);

                if (activeTabs.Any())
                    ActivateTab(activeTabs.Last());
            }
        }

        private void ActivateTab(Guna2Button tab)
        {
            foreach (var t in activeTabs)
                t.FillColor = Color.FromArgb(72, 118, 255);

            tab.FillColor = Color.DodgerBlue;

            tabContentHost.Controls.Clear();
            tabContentHost.Controls.Add(tabContents[tab]);
        }

        // ---------------- BUTTON STYLE HELPER ---------------- //
        private Guna2Button MakeControlButton(string text, EventHandler click, Color baseColor)
        {
            var btn = new Guna2Button
            {
                Text = text,
                Width = 60,
                Height = 33,
                BorderRadius = 10,
                FillColor = baseColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Margin = new Padding(5, 0, 0, 0),
                ShadowDecoration = { Enabled = true, Shadow = new Padding(2) }
            };

            btn.HoverState.FillColor = ControlPaint.Light(baseColor, 0.2f);
            btn.Click += click;
            return btn;
        }

        private static string GetPosToken()
        {
            string posToken = SystemConfigRepository.GetConfig("LastResumedSession");

            var tokenData = JsonSerializer.Deserialize<ResumeData>(posToken, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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

        private async Task SaveSaleSessionInfo()
        {
            string _authHeader = GetAuthToken();
            string _posToken = GetPosToken();

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", _authHeader);
                client.DefaultRequestHeaders.Add("X-PosToken", _posToken);

                string url = $"{AppConfig.BaseApiUrl}pos/sales/SaleSessionInfo";

                var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");

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
                var apiResponse = JsonSerializer.Deserialize<SaleSessionInfo>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                SystemConfigRepository.SaveConfig("SaleSessionInfo", apiResponse);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving sale session info: {ex.Message}");
            }
        }
    }
}
