﻿using System;
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
        private static TcpClient client;
        private const string hostName = "127.0.0.1";
        private static int Port { get; set; }

        static void Main(string[] args)
        {
            // Set Host
            IPHostEntry host = Dns.GetHostEntry(hostName);
            
            // Get IPv4 address back of host machine
            IPAddress ipAddress = host.AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            // Set Port
            int defaultPort = 8888;
            try
            {
                Port = int.Parse(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} Default port set at {defaultPort}");
                Port = defaultPort;
            }

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
                var listener = new TcpListener(tmp_address, Port);
                //var listener = new TcpListener(tmp_address, defaultPort);

                listener.Start();
                Console.WriteLine($"Listening on address: {ipAddress}, port: {Port}");

                // Buffer
                byte[] bytes = new byte[1024];
                string data;

                // Enter listening loop
                while (true)
                {
                    try
                    {
                        // Perform a blocking call to accept requests.
                        if (!listener.Pending())
                        {

                            //Console.WriteLine("Sorry, no connection requests have arrived");
                        }
                        else
                        {
                            client = listener.AcceptTcpClient();
                            Console.Write("Waiting for a connection... ");
                            //Console.WriteLine("Connected!");

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
