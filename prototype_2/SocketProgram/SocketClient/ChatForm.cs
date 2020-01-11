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
    public partial class ChatForm : Form
    {
        TcpClient client;
        TcpListener listener;
        public int ClientPort { get; set; }
        public int ServerPort { get; set; }
        public string OtherUser { get; set; }
        private const string hostName = "127.0.0.1";

        /*
         * note: will fail to load form if message not initialised
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
                //client = new TcpClient("127.0.0.1", ClientPort);

                //label1.Text = $"Connected with {OtherUser}...";
                //button2.Text = "Disconnect";
                //Communicate();


                // Set Host
                IPHostEntry host = Dns.GetHostEntry(hostName);

                // Get IPv4 address back of host machine
                IPAddress ipAddress = host.AddressList
                    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                IPAddress tmp_address = IPAddress.Parse(hostName);
                listener = new TcpListener(tmp_address, ServerPort);
                listener.Start();

                var sb = new StringBuilder();
                sb.AppendLine($"// Listening for incoming messages on: {ServerPort}");
                textBoxOut.AppendText(sb.ToString());

                Foo();

                //ServerListenLoop();
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket Exception: {ex.Message}");
                textBoxOut.AppendText($"Socket Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxOut.AppendText(ex.Message);
            }
            
        }
        private async Task Foo()
        {
            var s = new StringBuilder();
            TcpClient serverClient = await listener.AcceptTcpClientAsync();
            s.AppendLine($"// Success, connection received.");
            textBoxOut.AppendText(s.ToString());

            //bool connected = false;
            //while (!connected)
            //{
            //    try
            //    {
            //        var s = new StringBuilder();
            //        if (!listener.Pending())
            //        {
                        
            //            s.AppendLine($"// Sorry, not connection request received.");
            //            textBoxOut.AppendText(s.ToString());
            //        }
            //        else
            //        {
            //            TcpClient serverClient = await listener.AcceptTcpClientAsync();
            //            s.AppendLine($"// Success, connection received.");
            //            textBoxOut.AppendText(s.ToString());
            //            connected = true;
            //        }

            //        //System.Threading.Thread.Sleep(5000);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
                
            //}
        }
        private void ServerListenLoop()
        {

            // Buffer
            byte[] bytes = new byte[1024];
            string data;

            // Enter listening loop
            while (true)
            {
                //Console.Write("Waiting for a connection... ");
                // Perform a blocking call to accept requests.
                TcpClient client = listener.AcceptTcpClient();

                try
                {
                    /*
                     * research notes: throws error when client closes connection
                     */
                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;
                    i = stream.Read(bytes, 0, bytes.Length);

                    while (i != 0)
                    {

                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes);
                        Console.WriteLine($"Received: {data}");

                        /*
                         *  TO-DO: process data
                         */
                        data = data.ToUpper();
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine($"Sent: {data}");

                        i = stream.Read(bytes, 0, bytes.Length);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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

                sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] You: {message}");

                // Get server response
                /*
                * note: not working correctly
                * read: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netframework-4.8
                */
                stream.Read(data, 0, data.Length);
                string serverMessage = System.Text.Encoding.ASCII.GetString(data);

                sb.AppendLine($"[{DateTime.Now.ToString("hh:mm tt")}] {OtherUser}: {serverMessage}");
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
            }

            // Output to screen
            textBoxOut.AppendText(sb.ToString());
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            HandleMessage(sender, e);
        }

        private void HandleMessage(object sender, EventArgs e)
        {
            message = textBoxIn.Text;
            if (message != "")
            {
                Communicate();
                textBoxIn.Clear();
            }
        }
        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            //if (client != null && client.Connected)
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
                client = new TcpClient("127.0.0.1", ClientPort);
                sb.AppendLine($"// Success! Connected with {OtherUser} on {ClientPort}");
                textBoxOut.AppendText(sb.ToString());

                topLabel.Text = $"Connected with {OtherUser}";
                ConnectButton.Text = "Disconnect";
                //await Foo();
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
