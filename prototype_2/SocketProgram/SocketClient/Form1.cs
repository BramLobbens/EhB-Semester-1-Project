using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClient
{
    public partial class Form1 : Form
    {
        TcpClient client;
        /*
         * note: will fail to load form if message not initialised
         */
        string message = "hello";
        public Form1()
        {
            InitializeComponent();
            label1.Text = "Disconnected from server...";
            button2.Text = "Connect";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialise send button
            button1.Enabled = (textBox2.Text == "") ? false : true;

            // -> show message
            // Connect
            //clientSocket.Connect("127.0.0.1", 8888);
            try
            {
                client = new TcpClient("127.0.0.1", 8888);
                //clientSocket = new TcpClient("196.168.1.6", 8888);
                label1.Text = "Connected with server...";
                button2.Text = "Disconnect";
                Communicate();

            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket Exception: {ex.Message}");
                textBox1.Text = $"Socket Exception: {ex.Message}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBox1.Text = ex.Message;
            }
            
        }

        private void Communicate()
        {
            var sb = new StringBuilder();

            // Send to server
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            try
            {
                NetworkStream stream = client.GetStream();
                /*
                 * note: to sanitise stream input/output
                 * currently server message gets jumbled up
                 */
                stream.Write(data, 0, data.Length);

                sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] Client: {message}");

                // Get server response
                /*
                * note: not working correctly
                * read: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netframework-4.8
                */
                stream.Read(data, 0, data.Length);
                string serverMessage = System.Text.Encoding.ASCII.GetString(data);

                sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] Server: {serverMessage}");
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
            }

            // Output to screen
            textBox1.AppendText(sb.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HandleMessage(sender, e);
        }

        private void HandleMessage(object sender, EventArgs e)
        {
            message = textBox2.Text;
            if (message != "")
            {
                Communicate();
                textBox2.Clear();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (client != null && client.Connected)
            {
                client.Close();
                button2.Text = "Connect";
                label1.Text = "Disconnected from server...";
            }
            else
            {
                Form1_Load(sender, e);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox2.Text != "";
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                HandleMessage(sender, e);
                e.Handled = true; // suppress ding sound
            }
        }
    }
}
