using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentFTP;
using MySql.Data.MySqlClient;

namespace Quiziet
{
    public partial class StudentFormIndex : Form
    {
        public StudentFormIndex()
        {
            InitializeComponent();
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://127.0.0.1";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.Connect();
            foreach (FtpListItem item in clientFTP.GetListing($@"/Students/{Global.currentUser}")) //Obtain all distributed quiz directories and present them in the list box
            {
                listbox.Items.Add(item.Name);
            }
            clientFTP.Disconnect();
        }

        private void btnBack_Click(object sender, EventArgs e) //Event handler for when the back button is clicked
        {
            LoginForm loginForm = new LoginForm(); //Instatiate a LoginForm, show it, close this form
            loginForm.Show();
            this.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e) //Event handler for when the open button is clicked
        {
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://127.0.0.1";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.Connect();
            string csvPath = Path.GetTempPath() + "tempcsv.csv";
            if (clientFTP.FileExists($@"/Students/{Global.currentUser}/{listbox.GetItemText(listbox.SelectedItem)}")) //Ensure that the file selected still exists
            {
                clientFTP.DownloadFile(csvPath, $@"/Students/{Global.currentUser}/{listbox.GetItemText(listbox.SelectedItem)}"); //If it does
                QuizForm studentQuiz = new QuizForm(0, Global.CSVToArray(csvPath)); //Creates a temporary csv, converts that to a list of string arrays, passes into an instantiated QuizForm form
                this.Close(); //Close this form
            }
            else //Otherwise
            {
                MessageBox.Show("Error, quiz is no longer in your depository"); //Inform user the file no longer exists in their Depository
            }
        }
        protected override void OnClosed(EventArgs e) //Event handler for when the form is closed
        {
            base.OnClosed(e);       
            if (Application.OpenForms.Count == 1)   //If the form is the only one open, the application shuts down
                Application.Exit();
        }

        private void btnRefresh_Click(object sender, EventArgs e) //Event handler for when the refresh button is clicked
        {
            listbox.Items.Clear();
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://127.0.0.1";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.Connect();
            foreach (FtpListItem item in clientFTP.GetListing($@"/Students/{Global.currentUser}")) //Performs the same obtaining of directorys to quizzes as when the form is initialised
            {
                listbox.Items.Add(item.Name);
            }
            clientFTP.Disconnect();
        }

        private void StudentFormIndex_Load(object sender, EventArgs e) //Event handler for when the form loads
        {
            string ConnectionString = "Server=127.0.0.1;Database=prototypedatabase;Uid=Killian;Pwd=blackrock;"; //MySQL conncetion string
            using (MySqlConnection con = new MySqlConnection(ConnectionString)) //Scope for the MySQL connection
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.Parameters.Add(new MySqlParameter("userName", Global.currentUser));
                cmd.CommandText = $@"select idUser from prototypedatabase.userpass where UserName=@userName"; //Selects the User's ID number (primary key) and presents it to the user so that their teacher can add them to any classrooms
                string userID = cmd.ExecuteScalar().ToString();
                lblStudentID.Text = $@"Student ID number is: {userID}";
            }
        }

    }
}
