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
                    op.BasicConsume();
                    op.BasicExchanges();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("An Error occured whilst trying to log in: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                cbxType.Items.Add("Broadcast");

                if (login.LoginAsClient) cbxType.Items.Add("Broadcast Clients");
                else cbxType.Items.Add("Broadcast Servers");

                cbxType.SelectedIndex = 0;
            }
        }

        private void OnReceiveEvent(object sender, BasicDeliverEventArgs e)
        {
            txbChat.Text += Encoding.UTF8.GetString(e.Body);
            txbChat.ScrollToCaret();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            switch(cbxType.SelectedItem.ToString())
            {
                case "Broadcast":
                    op.Send("*", txbMessage.Text, "LHFullBroadcast");
                    break;
                case "Broadcast Servers":
                    op.Send("*", txbMessage.Text, "LHServerBroadcast");
                    break;
                case "Broadcast Clients":
                    op.Send("*", txbMessage.Text, "LHClientBroadcast");
                    break;
            }

            txbChat.Text += "Sent: " + txbMessage.Text;
            txbMessage.Text = "";
        }
    }
}
