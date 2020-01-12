namespace SocketClient
{
    partial class ChatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxOut = new System.Windows.Forms.TextBox();
            this.textBoxIn = new System.Windows.Forms.TextBox();
            this.sendMessageButton = new System.Windows.Forms.Button();
            this.topLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxOut
            // 
            this.textBoxOut.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOut.Location = new System.Drawing.Point(12, 51);
            this.textBoxOut.Multiline = true;
            this.textBoxOut.Name = "textBoxOut";
            this.textBoxOut.ReadOnly = true;
            this.textBoxOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxOut.Size = new System.Drawing.Size(422, 245);
            this.textBoxOut.TabIndex = 0;
            // 
            // textBoxIn
            // 
            this.textBoxIn.Location = new System.Drawing.Point(12, 377);
            this.textBoxIn.Name = "textBoxIn";
            this.textBoxIn.Size = new System.Drawing.Size(422, 20);
            this.textBoxIn.TabIndex = 1;
            this.textBoxIn.TextChanged += new System.EventHandler(this.TextBoxIn_TextChanged);
            this.textBoxIn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxIn_KeyPress);
            // 
            // sendMessageButton
            // 
            this.sendMessageButton.Location = new System.Drawing.Point(335, 415);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(99, 23);
            this.sendMessageButton.TabIndex = 2;
            this.sendMessageButton.Text = "Send message";
            this.sendMessageButton.UseVisualStyleBackColor = true;
            this.sendMessageButton.Click += new System.EventHandler(this.SendMessageButton_Click);
            // 
            // topLabel
            // 
            this.topLabel.AutoSize = true;
            this.topLabel.Location = new System.Drawing.Point(9, 18);
            this.topLabel.Name = "topLabel";
            this.topLabel.Size = new System.Drawing.Size(35, 13);
            this.topLabel.TabIndex = 3;
            this.topLabel.Text = "label1";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(15, 415);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(99, 23);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.Text = "Disconnect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 361);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Enter message:";
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(446, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.topLabel);
            this.Controls.Add(this.sendMessageButton);
            this.Controls.Add(this.textBoxIn);
            this.Controls.Add(this.textBoxOut);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ChatForm";
            this.Text = "Chat Window";
            this.Load += new System.EventHandler(this.ChatForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxOut;
        private System.Windows.Forms.TextBox textBoxIn;
        private System.Windows.Forms.Button sendMessageButton;
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label label2;
    }
}

