using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
            public int? Port { get; set; }
            public DateTime? LastOnline { get; set; }
        }

        public string ConnectionString { get; set; }
        public List<User> Users { get; set; }
        public string CurrentUserName { get; set; }
        public int CurrentPort { get; set; }

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;
            Users = new List<User>();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (CurrentUserName == null)
            {
                try
                {
                    // Open dialog
                    SetUserName();
                    // Update view
                    userNameLabel.Text += " " + CurrentUserName;
                    RefreshViewList();
                    toolStripStatusLabel1.Text = $"Connected on port: {CurrentPort}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show($"Error: Could not connect to database.");
                    button2.Enabled = false;
                }
            }
        }

        private List<User> GetUsersFromDB()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var users = new List<User>();
                try
                {
                    var command = new SqlCommand("select [UserName]" +
                                                " , [OnlineStatus]" +
                                                " , [Port]" +
                                                " , [OnlineDate]" +
                                                " from [Users]",
                                                connection);
                    // Open connection
                    connection.Open();

                    // Execute SELECT
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            // User Name
                            string userName = reader[0].ToString();

                            // Online Status
                            bool onlineStatus = reader.GetBoolean(1);
                            
                            // Port
                            int number;
                            int? port;
                            if (Int32.TryParse(reader[2].ToString(), out number))
                            {
                                port = number;
                            }
                            else
                            {
                                port = null;
                            }

                            // Last Online Date
                            int x = reader.GetOrdinal("OnlineDate");
                            DateTime? onlineDate = reader.IsDBNull(x) ? (DateTime?)null : reader.GetDateTime(x);

                            users.Add(new User { UserName = userName, OnlineStatus = onlineStatus, Port = port, LastOnline = onlineDate });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception();
                }

                return users;
            }
        }

        private void SetUserInDB(string userName, bool status, int port)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    var command = new SqlCommand("insert into [Users] ([Username], [OnlineStatus], [Port])" +
                                                " values (@UserName, @Status, @Port)",
                                                connection);

                    // Define parameters and their values
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Port", port);

                    // Open connection and execute INSERT
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    throw new Exception();
                }
            }
        }

        private void UpdateUserInDB(string userName, bool status, int? port)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    var command = new SqlCommand("update [Users]" +
                                                " set [OnlineStatus] = @Status," +
                                                " [Port] = @Port," +
                                                " [OnlineDate] = @Date" +
                                                " where [UserName] = @UserName",
                                                connection);

                    // Define parameters and their values
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Port", (object)port ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Date", DateTime.Now);

                    // Open connection and execute UPDATE
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    throw new Exception();
                }
            }
        }

        private void SetUserName()
        {
            using (var form = new UserNameDialogue())
            {

                // Get dialog result and get value when user chose OK
                DialogResult dialogResult = form.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    CurrentUserName = form.UserName;
                    CurrentPort = form.Port;

                    try
                    {
                        // Load users from Database
                        Users.AddRange(GetUsersFromDB());

                        // If user name already exists, pad with random digits to make unique
                        if (Users.Any(user => user.UserName.Equals(CurrentUserName) && user.OnlineStatus == true))
                        {
                            CurrentUserName += new Random().Next(11, 999);
                            SetUserInDB(CurrentUserName, true, CurrentPort);
                        }
                        // If user name exists, but user is offline, the current user can use this name.
                        else if (Users.Any(user => user.UserName.Equals(CurrentUserName) && user.OnlineStatus == false))
                        {
                            UpdateUserInDB(CurrentUserName, true, CurrentPort);
                        }
                        // Set as new user
                        else
                        {
                            SetUserInDB(CurrentUserName, true, CurrentPort);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new Exception();
                    }

                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    DialogResult d = MessageBox.Show("Are you sure you want quit?", "Are you sure?", MessageBoxButtons.YesNo);
                    if (d == DialogResult.Yes)
                    {
                        this.Close();
                    }
                    else if (d == DialogResult.No)
                    {
                        SetUserName();
                    }
                }
            }
        }

        private void RefreshViewList()
        {
            listView1.Clear();
            Users.Clear();
            // Load users from Database
            Users.AddRange(GetUsersFromDB());
            // Sort by online status
            List<User> sortedByStatus = Users.OrderByDescending(user => user.OnlineStatus).ToList();
            // Format
            foreach(User user in sortedByStatus)
            {
                var li = new ListViewItem((user.OnlineStatus) ? $"{user.UserName}" : $"{user.UserName} (offline)");
                if (user.OnlineStatus)
                {
                    li.ForeColor = Color.FromArgb(32, 214, 4);
                    li.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                }
                string date = (user.LastOnline != null) ? $"{user.LastOnline:yyyy/MM/dd}" : "unknown";
                li.ToolTipText = $"{(user.OnlineStatus ? "" : $"User was last online on: {date}")}";
                listView1.Items.Add(li);
            }
            // Update view
            listView1.Refresh();
        }

        private void UserListView_DoubleClick(object sender, EventArgs e)
        {
            // Get selected username from ViewList
            string otherUserName = listView1.SelectedItems[0].Text.Split()[0];

            // Dereference selected username's port
            int? otherUserPort = Users.Find(user => user.UserName.Equals(otherUserName)).Port;

            // Open client to other user
            try
            {
                if (otherUserPort != null)
                {
                    int clientPort = (int)otherUserPort;
                    int serverPort = CurrentPort;
                    var chatForm = new ChatForm(otherUserName, clientPort, serverPort);
                    chatForm.MdiParent = this;
                    splitContainer1.Panel2.Controls.Add(chatForm);
                    chatForm.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshViewList();
        }

        private void MainForm_FormClosing(Object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want quit?", "Are you sure?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    UpdateUserInDB(CurrentUserName, false, null);
                    Application.Exit();
                }
                else if (dialogResult == DialogResult.No)
                {
                    // Don't do anything
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Silent
            }
        }

        /*
         * Menu Toolstrip Section
         */
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Programmeren 3 - School year 2019-2020");
            sb.AppendLine("By Bram Lobbens");
            sb.AppendLine("Latest update: 2020/01/11");
            MessageBox.Show(sb.ToString(), "About");
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new ColorDialog())
            {
                DialogResult dialogResult = form.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    splitContainer1.Panel2.BackColor = form.Color;
                }
            }
        }

        private void reportAProblemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/BramLobbens/EhB-Semester-1-Project/issues");
        }
    }
}
