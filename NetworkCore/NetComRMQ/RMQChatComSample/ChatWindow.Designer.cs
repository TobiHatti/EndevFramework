namespace RMQChatComSample
{
    partial class ChatWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.txbMessage = new System.Windows.Forms.TextBox();
            this.cbxType = new System.Windows.Forms.ComboBox();
            this.txbChat = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txbMessage
            // 
            this.txbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbMessage.Location = new System.Drawing.Point(12, 511);
            this.txbMessage.Multiline = true;
            this.txbMessage.Name = "txbMessage";
            this.txbMessage.Size = new System.Drawing.Size(191, 21);
            this.txbMessage.TabIndex = 0;
            // 
            // cbxType
            // 
            this.cbxType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxType.FormattingEnabled = true;
            this.cbxType.Location = new System.Drawing.Point(209, 511);
            this.cbxType.Name = "cbxType";
            this.cbxType.Size = new System.Drawing.Size(98, 21);
            this.cbxType.TabIndex = 1;
            // 
            // txbChat
            // 
            this.txbChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbChat.Location = new System.Drawing.Point(12, 12);
            this.txbChat.Multiline = true;
            this.txbChat.Name = "txbChat";
            this.txbChat.Size = new System.Drawing.Size(295, 493);
            this.txbChat.TabIndex = 2;
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(12, 538);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(295, 23);
            this.btnSendMessage.TabIndex = 3;
            this.btnSendMessage.Text = "Send";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            // 
            // ChatWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 566);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.txbChat);
            this.Controls.Add(this.cbxType);
            this.Controls.Add(this.txbMessage);
            this.Name = "ChatWindow";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ChatWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbMessage;
        private System.Windows.Forms.ComboBox cbxType;
        private System.Windows.Forms.TextBox txbChat;
        private System.Windows.Forms.Button btnSendMessage;
    }
}

