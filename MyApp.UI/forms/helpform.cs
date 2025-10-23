namespace MyApp.UI.Forms
{
    public class HelpModel
    {

        internal class displayMessage
        {
            public displayMessage()
            {
                string helpText =
                     @"🧾 Shortcut Keys:
                        -----------------------------------------
                        Add New Tab ............ Alt + T
                        Remove Active Tab ....... Alt + R
                        Pair Thermal Printer ..... Alt + A
                        Print Last Invoice ....... Alt + L
                        Miscellaneous Services ... Alt + M
                        Help Window .............. Alt + H
                        Post Sale ................ Alt + Enter
                        Add Notes ................ Alt + N
                        Payment Amount ........... Alt + /
                        Toggle Amount Sign ....... Alt + 0
                        Add 50 ................... Alt + 1
                        Add 100 .................. Alt + 2
                        Add 500 .................. Alt + 3
                        Add 1000 ................. Alt + 4
                        Add 5000 ................. Alt + 5";

                MessageBox.Show(helpText, "Help & Shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}