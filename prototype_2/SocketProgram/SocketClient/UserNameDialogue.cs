using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClient
{
    public partial class UserNameDialogue : Form
    {
        public string UserName { get; set; }
        public int Port { get; set; }
        private ToolTip PortToolTip { get; set; }
        public UserNameDialogue()
        {
            InitializeComponent();

            // Set tooltip
            PortToolTip = new ToolTip();
            PortToolTip.ToolTipIcon = ToolTipIcon.Info;
            PortToolTip.IsBalloon = true;
            PortToolTip.ShowAlways = true;
            PortToolTip.ToolTipTitle = "What is this?";
            PortToolTip.SetToolTip(label2, "Selecting a port number enables you to connect with another user.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserName = textBox1.Text.Trim();
            Port = (int) portValue.Value;
        }

        private void portValue_Enter(object sender, EventArgs e)
        {
            portValue.Select(0, portValue.Text.Length);
        }
    }
}
