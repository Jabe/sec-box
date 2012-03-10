using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SecBox
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void BlurOnReturn(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return && sender is Control)
            {
                SelectNextControl((Control) sender, true, true, true, true);
            }
        }

        private void UxPasswordTextChanged(object sender, EventArgs e)
        {
            string unsecureText = uxPassword.UnsecureText;

            Console.WriteLine("Length: {0,4} | Text: {1} | UnsecureText: {2}",
                              unsecureText.Length,
                              uxPassword.Text,
                              unsecureText);
        }
    }
}