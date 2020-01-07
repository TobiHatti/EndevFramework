using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RMQChatComSample
{
    public partial class Login : Form
    {
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public bool LoginAsClient { get; set; } = false;

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoginRoutine();
        }

        private void LoginRoutine()
        {
            if (!string.IsNullOrEmpty(txbUsername.Text) && !string.IsNullOrEmpty(txbPassword.Text))
            {
                Username = txbUsername.Text;
                Password = txbPassword.Text;

                if (rbnLoginClient.Checked) LoginAsClient = true;
                else LoginAsClient = false;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("Please enter Username and Password!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                LoginRoutine();
        }

        private void txbUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                LoginRoutine();
        }
    }
}
