using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClient
{
    public partial class ChatForm : Form
    {
        TcpClient client;
        TcpListener listener;
        public int ClientPort { get; set; }
        public int ServerPort { get; set; }
        public string OtherUser { get; set; }
        private const string hostName = "127.0.0.1";

        /*
         * note: initial message string required
         */
        string message = "hello";
        public ChatForm()
        {
            InitializeComponent();
            topLabel.Text = "Connect with user...";
            ConnectButton.Text = "Connect";
        }

        public ChatForm(string userName, int clientPort, int serverPort) : this()
        {
            OtherUser = userName;
            ClientPort = clientPort;
            ServerPort = serverPort;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            // Initialise send button
            sendMessageButton.Enabled = (textBoxIn.Text == "") ? false : true;

            // Connect
            try
            {
                //// Set Host
                //IPHostEntry host = Dns.GetHostEntry(hostName);

                //// Get IPv4 address back of host machine
                //IPAddress ipAddress = host.AddressList
                //    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                //IPAddress tmp_address = IPAddress.Parse(hostName);
                //listener = new TcpListener(tmp_address, ServerPort);
                //listener.Start();

                //var sb = new StringBuilder();
                //sb.AppendLine($"// Listening for incoming messages on: {ServerPort}");
                //textBoxOut.AppendText(sb.ToString());

                //ListenForMessages();

                using (var process = new Process())
                {
                    ProcessStartInfo info = new ProcessStartInfo(@"SocketServer.exe");
                    info.UseShellExecute = false;
                    info.Arguments = $"{ServerPort}";
                    //info.Arguments = "8000"; // A & B listen to same port
                    Process.Start(info);
                }

                ListenForMessages_();
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket Exception: {ex.Message}");
                textBoxOut.AppendText($"Socket Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                textBoxOut.AppendText(ex.Message);
            }
            
        }

        private void ListenForMessages_()
        {
            // Buffer
            byte[] bytes = new byte[1024];
            string data;
            try
            {
                var sb = new StringBuilder();

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;
                i = stream.Read(bytes, 0, bytes.Length);
                while (i != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes);

                    data = data.ToUpper();
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    //stream.ReadAsync(data, 0, data.Length);
                    string serverMessage = System.Text.Encoding.ASCII.GetString(msg);
                    sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] {OtherUser}: {serverMessage}");

                    // Send back a response.
                    //stream.WriteAsync(msg, 0, msg.Length);

                    // Read if any new data in stream
                    //i = stream.Read(bytes, 0, bytes.Length);
                    i = 0;
                }
                textBoxOut.AppendText(sb.ToString());
                // Shutdown and end connection
                //client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private async Task ListenForMessages()
        {
            var s = new StringBuilder();
            TcpClient serverClient = await listener.AcceptTcpClientAsync();
            s.AppendLine($"// Success, connection received.");
            textBoxOut.AppendText(s.ToString());

            // Buffer
            byte[] bytes = new byte[1024];
            string data;
            try
            {
                var sb = new StringBuilder();

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;
                i = await stream.ReadAsync(bytes, 0, bytes.Length);
                while (i != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes);

                    data = data.ToUpper();
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    //stream.ReadAsync(data, 0, data.Length);
                    string serverMessage = System.Text.Encoding.ASCII.GetString(msg);
                    sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] {OtherUser}: {serverMessage}");
                    
                    // Send back a response.
                    await stream.WriteAsync(msg, 0, msg.Length);

                    // Read if any new data in stream
                    i = await stream.ReadAsync(bytes, 0, bytes.Length);   
                }
                textBoxOut.AppendText(sb.ToString());
                // Shutdown and end connection
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void WriteMessage()
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
                //await stream.WriteAsync(data, 0, data.Length);
                stream.Write(data, 0, data.Length);

                sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] You: {message}");

                //await ListenForMessages();
                //await stream.ReadAsync(data, 0, data.Length);
                //stream.Read(data, 0, data.Length);
                //string serverMessage = System.Text.Encoding.ASCII.GetString(data);
                //sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] {OtherUser}: {serverMessage}");
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
            }

            // Output to screen
            textBoxOut.AppendText(sb.ToString());
            //await ListenForMessages();
            ListenForMessages_();
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            HandleMessage(sender, e);
        }

        private async void HandleMessage(object sender, EventArgs e)
        {
            message = textBoxIn.Text;
            if (message != "")
            {
                WriteMessage();
                textBoxIn.Clear();
            }
        }
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            if (client != null)
            {
                client.Close();
                client = null;
                ConnectButton.Text = "Connect";
                topLabel.Text = "Disconnected from user...";

                sb.AppendLine($"// Disconnected with {OtherUser}");
                textBoxOut.AppendText(sb.ToString());
            }
            else
            {
                try
                {
                    client = new TcpClient("127.0.0.1", ClientPort);
                    sb.AppendLine($"// Success! Connected with {OtherUser} on {ClientPort}");
                    textBoxOut.AppendText(sb.ToString());

                    topLabel.Text = $"Connected with {OtherUser}";
                    ConnectButton.Text = "Disconnect";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Other user is not available.");
                }
            }
        }

        private void TextBoxIn_TextChanged(object sender, EventArgs e)
        {
            sendMessageButton.Enabled = textBoxIn.Text != "";
        }

        private void TextBoxIn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                HandleMessage(sender, e);
                e.Handled = true; // suppress ding sound
            }
        }
    }
}
