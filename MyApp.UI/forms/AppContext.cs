using System;
using System.Windows.Forms;
using MyApp.UI.Forms;

namespace MyApp.UI
{
    public class AppContext : ApplicationContext
    {
        public AppContext()
        {
            ShowDashboard(); // ✅ Start with Dashboard
        }

        private void ShowDashboard()
        {
            var dashboard = new Dashboard();
            dashboard.FormClosed += (s, e) =>
            {
                // Check dashboard.Tag to decide what to do next
                switch (dashboard.Tag)
                {
                    case "SignOut":
                        ShowSignIn();
                        break;
                    case "Sale":
                        ShowSaleScreen();
                        break;
                    default:
                        ExitThread(); // Exit app when dashboard closes normally
                        break;
                }
            };
            dashboard.Show();
        }

        private void ShowSignIn()
        {
            var signIn = new SignInForm();
            signIn.FormClosed += (s, e) =>
            {
                if (signIn.DialogResult == DialogResult.OK)
                    ShowDashboard(); // Back to dashboard on successful login
                else
                    ExitThread(); // Exit if closed manually
            };
            signIn.Show();
        }

        private void ShowSaleScreen()
        {
            var sale = new SaleScreenForm();
            sale.FormClosed += (s, e) =>
            {
                ShowDashboard(); // Return to dashboard after closing sale screen
            };
            sale.Show();
        }
    }
}
