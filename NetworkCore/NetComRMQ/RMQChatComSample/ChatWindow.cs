using NetComRMQ;
using RabbitMQ.Client.Events;
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
        private RMQOperator op = null;
        private string host = "localhost";

        public ChatWindow()
        {
            InitializeComponent();
        }

        private void ChatWindow_Load(object sender, EventArgs e)
        {
            Login login = new Login();
            if(login.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (login.LoginAsClient) op = new RMQClient(host, login.Username, login.Password);
                    else op = new RMQServer(host, login.Username, login.Password);
                
                    op.ReceiveEvent(OnReceiveEvent);
                    op.SelfConsume();
                    op.BasicExchanges();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("An Error occured whilst trying to log in: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                cbxType.Items.Add("Broadcast");
                cbxType.Items.Add("Broadcast Clients");
                cbxType.Items.Add("Broadcast Servers");
                cbxType.SelectedIndex = 0;

                if (login.LoginAsClient) this.Text = "Logged in as Client";
                else this.Text = "Logged in as Server";
            }
        }

        private void OnReceiveEvent(object sender, BasicDeliverEventArgs e)
        {
            txbChat.Text += "Received> " + Encoding.UTF8.GetString(e.Body) + Environment.NewLine;
            txbChat.ScrollToCaret();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void Send()
        {
            switch (cbxType.SelectedItem.ToString())
            {
                case "Broadcast":
                    op.SendTo(txbMessage.Text, "LHFullBroadcast", "*");
                    break;
                case "Broadcast Servers":
                    op.SendTo(txbMessage.Text, "LHServerBroadcast", "*");
                    break;
                case "Broadcast Clients":
                    op.SendTo(txbMessage.Text, "LHClientBroadcast", "*");
                    break;
            }

            txbChat.Text += "Sent> " + txbMessage.Text + Environment.NewLine;
            txbMessage.Text = "";
            txbChat.ScrollToCaret();

            txbMessage.Focus();
        }

        private void txbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                Send();
        }

        private void ChatWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            op.Close();
        }
    }
}
