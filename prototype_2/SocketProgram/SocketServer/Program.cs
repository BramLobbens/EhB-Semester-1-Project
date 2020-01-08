using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    class Program
    {
        private const string hostName = "127.0.0.1";
        private const int port = 8888;
        public static string ConnectionString { get; set; }

        static void CreateDbConnection()
        {
            /*
             * Set DbConnectionstring
             */
            ConnectionString = SocketServer.Properties.Settings.Default.ProjectDb_BramConnectionString;
        }

        static List<string> GetUsersFromDB()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var users = new List<string>();
                var adapter = new SqlDataAdapter();

                try
                {
                    var command = new SqlCommand("select [UserName]" +
                                                " from [Users]" +
                                                " order by [OnlineStatus]",
                                                connection);
                    // Open connection
                    connection.Open();

                    // Read SelectQuery results and append to StringBuilder object
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            users.Add(reader[0].ToString());
                        }
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }

                return users;
            }
        }

        static void Main(string[] args)
        {
            // Create connection with Database
            CreateDbConnection();

            // Get users from Database
            foreach (var user in GetUsersFromDB())
            {
                Console.WriteLine(user);
            }

            // 
            IPHostEntry host = Dns.GetHostEntry(hostName);
            
            // Get IPv4 address back of host machine
            IPAddress ipAddress = host.AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            /*
             * research notes: alternatively use the following
             */
            // set the TcpListener on port 13000
            //int port = 13000;
            //TcpListener server = new TcpListener(IPAddress.Any, port);

            // Start listening for client requests
            //server.Start();
            try
            {
                // Create TcpListener and start listening
                //var server = new TcpListener(ipAddress, port);

                /*
                 * research notes: currently not working with ipAddress from AddressList
                 */
                IPAddress tmp_address = IPAddress.Parse(hostName);
                var server = new TcpListener(tmp_address, port);

                server.Start();
                Console.WriteLine($"Listening on address: {ipAddress}, port: {port}");

                // Buffer
                byte[] bytes = new byte[1024];
                string data;

                // Enter listening loop
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

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
#if DEBUG
                        Console.WriteLine("Inside Exception");
#endif
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Hit enter to continue...");
            Console.Read();
        }
    }
}
