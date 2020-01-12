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
            public bool Favorited { get; set; }
        }

        public string ConnectionString { get; set; }
        public List<User> Users { get; set; }
        public List<User> Favorites { get; set; }
        public string CurrentUserName { get; set; }
        public int CurrentPort { get; set; }

        public delegate void ViewListDelegate(string s);

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;
            Users = new List<User>();
            Favorites = new List<User>();
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
                    toolStripStatusLabelLeft.Text = $"Connected on port: {CurrentPort}";
                    tabControl1.TabPages[0].Text = "All";
                    tabControl1.TabPages[1].Text = "Favorites";
                    tabControl1.TabPages[1].ToolTipText = "Set a user to your favorites tab by right-clicking on their name.";
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
                                                " , [Favorited]" +
                                                " from [Users] as u" +
                                                " inner join [Favorites] as f" +
                                                " on u.Id = f.UserId",
                                                connection);
                    // Open connection
                    connection.Open();

                    // Execute SELECT
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            // 1. User Name
                            string userName = reader[0].ToString();

                            // 2. Online Status
                            bool onlineStatus = reader.GetBoolean(1);
                            
                            // 3. Port
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

                            // 4. Last Online Date
                            int x = reader.GetOrdinal("OnlineDate");
                            DateTime? onlineDate = reader.IsDBNull(x) ? (DateTime?)null : reader.GetDateTime(x);

                            // 5. Favorited
                            bool favorited = reader.GetBoolean(4);

                            users.Add(new User { UserName = userName, OnlineStatus = onlineStatus, Port = port, LastOnline = onlineDate, Favorited = favorited });
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
                    // Users Table
                    var command = new SqlCommand("insert into [Users] ([Username], [OnlineStatus], [Port])" +
                                                " values (@UserName, @Status, @Port)",
                                                connection);

                    // Define parameters and their values
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Port", port);

                    // Update Favorites Table
                    var command2 = new SqlCommand("insert into [Favorites] ([UserId])" +
                                                 " select u.Id" +
                                                 " from Users as u" +
                                                 " where not exists (select f.UserId from Favorites as f where f.UserId = u.Id)",
                                                 connection);

                    // Open connection and execute INSERT
                    connection.Open();
                    command.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    throw new Exception();
                }
            }
        }

        private void UpdateUserInDB(string userName, bool status, int? port, bool favorited=false)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Update Users table
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

                    // Update Favorites
                    var command2 = new SqlCommand("update [Favorites]" +
                                                 " set [Favorited] = @Favorited" +
                                                 " where UserId =" +
                                                 " (select u.Id from Users as u where u.UserName = @UserName)",
                                                 connection);

                    // Define parameters and their values
                    command2.Parameters.AddWithValue("@Favorited", favorited);
                    command2.Parameters.AddWithValue("@UserName", userName);

                    // Open connection and execute UPDATE
                    connection.Open();
                    command.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
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

                        // Set favorites
                        Users.Where(user => user.Favorited).ToList().ForEach(user => Favorites.Add(user));
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
            // Clear data
            listView1.Clear();
            listView2.Clear();
            Users.Clear();
            Favorites.Clear();

            var messageHandler = new ViewListDelegate(Foo);
            messageHandler("Tip: You can refresh your user list to see if anyone has come online.");

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

            Users.Where(user => user.Favorited).ToList().ForEach(user => Favorites.Add(user));
            // Format
            foreach (User user in Favorites)
            {
                var li = new ListViewItem((user.OnlineStatus) ? $"{user.UserName}" : $"{user.UserName} (offline)");
                if (user.OnlineStatus)
                {
                    messageHandler("Notice: Favorites has changed.");

                    li.ForeColor = Color.FromArgb(32, 214, 4);
                    li.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                }
                string date = (user.LastOnline != null) ? $"{user.LastOnline:yyyy/MM/dd}" : "unknown";
                li.ToolTipText = $"{(user.OnlineStatus ? "" : $"User was last online on: {date}")}";
                listView2.Items.Add(li);
            }

            // Update view
            listView1.Refresh();
            listView2.Refresh();

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

        private void Foo(string message)
        {
            toolStripStatusLabelRight.Text = $"{message}";
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
            }
        }

        /*
         * Menu Toolstrip Section
         */
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Properties.Resources.ProjectName}");
            sb.AppendLine();
            sb.AppendLine($"Author: {Properties.Resources.Author}");
            sb.AppendLine($"Latest Build: {Properties.Resources.BuildDate}");
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
            System.Diagnostics.Process.Start(Properties.Resources.ExternalLinkGitHub_Issues);
        }

        /*
         * Context Menu
         */
        private void setAsFavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set selected user as a favorite
            var selectedUser = listView1.SelectedItems[0].Text.Split()[0];
            var status = Users.Find(user => user.UserName.Equals(selectedUser)).OnlineStatus;
            UpdateUserInDB(selectedUser, status, null, true);
            // Update view
            RefreshViewList();
        }
    }
}
