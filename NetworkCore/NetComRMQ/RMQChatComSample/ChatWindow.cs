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
    public partial class ChatWindow : Form
    {
        public ChatWindow()
        {
            InitializeComponent();
        }

        private void ChatWindow_Load(object sender, EventArgs e)
        {
            Login login = new Login();
            if(login.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("alles kk");
            }
        }
    }
}
