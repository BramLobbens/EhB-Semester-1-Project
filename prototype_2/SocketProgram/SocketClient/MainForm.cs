using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClient
{
    public partial class MainForm : Form
    {
        public struct User
        {
            public string UserName { get; set; }
            public bool OnlineStatus { get; set; }
        }

        public string ConnectionString { get; set; }
        public List<User> Users { get; set; }
        public string CurrentUserName { get; set; }
        TcpClient client;

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;

            client = null;
            Users = new List<User>();
        }

        private List<User> GetUsersFromDB()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var users = new List<User>();
                var adapter = new SqlDataAdapter();

                try
                {
                    var command = new SqlCommand("select [UserName]" +
                                                " , [OnlineStatus]" +
                                                " from [Users]",
                                                connection);
                    // Open connection
                    connection.Open();

                    // Execute query
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            string userName = reader[0].ToString();
                            bool onlineStatus = reader.GetBoolean(1);
                            users.Add(new User { UserName = userName, OnlineStatus = onlineStatus });
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

        private void SetUserInDB(string userName, bool status)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var adapter = new SqlDataAdapter();
                try
                {
                    var command = new SqlCommand("insert into [Users] ([Username], [OnlineStatus])" +
                                                " values (@UserName, @Status)",
                                                connection);

                    // Define parameters and their values
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Status", status);

                    // Open connection and execute INSERT
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

        private void UpdateUserInDB(string userName, bool status)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var adapter = new SqlDataAdapter();
                try
                {
                    var command = new SqlCommand("update [Users])" +
                                                " set [OnlineStatus] = @Status" +
                                                " where [UserName] = @UserName",
                                                connection);

                    // Define parameters and their values
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Status", status);

                    // Open connection and execute INSERT
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

        private void SetUserName()
        {
            using (var form = new UserNameDialogue())
            {
                // Verkrijg de gekozen dialoogvenster waarde en verkrijg de waarde als de gebruiker voor OK heeft gekozen
                DialogResult dialogResult = form.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    CurrentUserName = form.UserName;
                    // Load users from Database
                    Users.AddRange(GetUsersFromDB());

                    // If user name already exists, pad with random digits to make unique
                    if (Users.Any(user => user.UserName.Equals(CurrentUserName) && user.OnlineStatus == true))
                    {
                        CurrentUserName += new Random().Next(11, 999);
                        SetUserInDB(CurrentUserName, true);
                    }
                    // If user name exists, but user is offline, the current user can use this name.
                    else if (Users.Any(user => user.UserName.Equals(CurrentUserName) && user.OnlineStatus == false))
                    {
                        UpdateUserInDB(CurrentUserName, true);
                    }
                    // Set as new user
                    else
                    {
                        SetUserInDB(CurrentUserName, true);
                    }
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    MessageBox.Show("Goodbye.");
                    this.Close();
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (CurrentUserName == null)
            {
                // Open dialog
                SetUserName();
                // Update view
                userNameLabel.Text += " " + CurrentUserName;
            }

            // Clear possible cache of user names
            Users.Clear();
            // Load users from Database
            Users.AddRange(GetUsersFromDB());
            // Sort by online status
            List<User> sortedByStatus = Users.OrderByDescending(user => user.OnlineStatus).ToList(); // LINQ
            sortedByStatus.ForEach(user => listView1.Items.Add($"{user.UserName} ({(user.OnlineStatus ? "Online" : "Offline")})"));
            // Update view
            listView1.Refresh();

            // TCP Connection
            try
            {
                client = new TcpClient("127.0.0.1", 8888);
                toolStripStatusLabel1.Text = "Connected with server...";
                button2.Text = "Disconnect";
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /*
             * testing purposes
             */
            //var chatForm = new Form1();
            //chatForm.MdiParent = this;

            //splitContainer1.Panel2.Controls.Add(chatForm);
            //chatForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    client.Close();
                    button2.Text = "Connect";
                    toolStripStatusLabel1.Text = "Disconnected from server...";
                    listView1.Clear();
                }
                else
                {
                    Form2_Load(sender, e);
                }
            }
            catch (NullReferenceException err)
            {
                Form2_Load(sender, e);
            }
        }

        private void MainForm_FormClosing(Object sender, EventArgs e)
        {
            UpdateUserInDB(CurrentUserName, false);
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm_FormClosing(sender, e);
        }
    }
}
