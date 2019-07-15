using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace Quiziet
{
	public partial class TeacherFormManager : Form
	{
		public TeacherFormManager()
		{
			InitializeComponent();
		}

		private void TeacherFormManager_Load(object sender, EventArgs e) //Event handler for when the form loads
		{
			ListDirectory();
        }

		public void ListDirectory() //Subroutine to list the directory of the currently logged in user
		{
            listBox1.Items.Clear();
			FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://82.10.84.171";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            foreach (FtpListItem item in clientFTP.GetListing($@"Classes/{Global.currentUser}")) //Runs through each directory, adding them to the list box
            {
                listBox1.Items.Add(item.FullName);
            }
        }

        public void ListMinorDirectory() //Subroutine to list the directory of the selected item in the first list box
        {
            listBox2.Items.Clear();
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://82.10.84.171";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            foreach (FtpListItem item in clientFTP.GetListing(listBox1.GetItemText(listBox1.SelectedItem))) //Runs through each file in the directory selected, adding the file names to the second list box
            {
                listBox2.Items.Add(item.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) //Event handler for when the selection of a different item occurs in the first list box
        {
            ListMinorDirectory();
        }

        private void addClassBtn_Click(object sender, EventArgs e) //Event handler for when the Add Class button is clicked
		{
			FtpClient clientFTP = new FtpClient();
			clientFTP.Host = "ftp://82.10.84.171";
			clientFTP.Port = 54443;
			clientFTP.Credentials.UserName = "user";
			clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.CreateDirectory($@"Classes\{Global.currentUser}\{classNameBox.Text}"); //Simply creates the directory of the inputted name in the logged in users directory
			clientFTP.Disconnect();
            ListDirectory(); //Refreshed the first list box input
		}

        private void addStudentBtn_Click(object sender, EventArgs e) //Event handler for when the Add Student button is clicked
        {
            string className; //Instantiates the class name
            string studentID = pinBox.Text; //initialises the student ID to be added and the username to be added
            string userName = "placeholder";
            if (listBox1.SelectedItem != null) //Ensures a class name has been selected
            {
                FtpClient clientFTP = new FtpClient();
                clientFTP.Host = "ftp://82.10.84.171";
                clientFTP.Port = 54443;
                clientFTP.Credentials.UserName = "user";
                clientFTP.Credentials.Password = "qwerty";
                clientFTP.DataConnectionReadTimeout = 2147483645;
                clientFTP.ConnectTimeout = 2147483645;
                clientFTP.DataConnectionConnectTimeout = 2147483645;
                clientFTP.ReadTimeout = 2147483645;
                string ConnectionString = "Server=82.10.84.171;Database=prototypedatabase;Uid=Killian;Pwd=blackrock;"; //MySQL Connection string
                using (MySqlConnection con = new MySqlConnection(ConnectionString)) //Scope for the MySQL commands
                {
                    con.Open();
                    MySqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = @"select count(*) from userpass where idUser=@studentID"; //Query used to obtain the number of students with the same user ID
                    cmd.Parameters.Add(new MySqlParameter("studentID", studentID));
                    int i = Convert.ToInt32(cmd.ExecuteScalar()); //Assigns the variable i to the output/result of the MySQL query
                    if (i > 0) //If a user exists with the user ID
                    {
                        cmd.CommandText = @"select UserName from prototypedatabase.userpass where idUser=@studentID"; //Another command query, obtains the user name of the user with matching user ID
                        userName = cmd.ExecuteScalar().ToString(); //Assigns the user name to the variable userName
                        StringBuilder csvContent = new StringBuilder(); //Creates a StringBuilder object
                        csvContent.Append($"{studentID}, {userName}"); //Appends this StringBuilder object with the content obtained from the MySQL queries
                        string csvPath = Path.GetTempPath() + "tempcsv.csv"; //Creates a temporary CSV file
                        File.AppendAllText(csvPath, csvContent.ToString()); //Inserts this StringBuilder object's contents to the csv
                        foreach (FtpListItem item in clientFTP.GetListing($@"Classes/{Global.currentUser}")) //Runs through the directory of the teacher for comparisons
                        {
                            if (listBox1.SelectedItem.ToString() == item.FullName) //Finds the matching directory of the one where a student is to be added
                            {
                                className = item.Name;
                                if (!(clientFTP.FileExists($@"\Classes\{ Global.currentUser}\{className}\{userName}.csv"))) //Checks that the student hasnt already been added to the class
                                {
                                    clientFTP.UploadFile(csvPath, $@"\Classes\{Global.currentUser}\{className}\{userName}.csv"); //If not, then adds student CSV file to teacher's directory under the name of the students user name
                                    MessageBox.Show("Success, student added!"); //Informs the user that the student was successfully added
                                    ListDirectory(); //Refresh of the first list box
                                }
                                else //Otherwise
                                {
                                    MessageBox.Show("Student already added to this class"); //Informs the user that the student already exists in this class
                                }
                            }
                        }
                        clientFTP.Disconnect();
                        File.Delete(csvPath); //Deletes temporary CSV file
                    }
                    else //Otherwise
                    {
                        MessageBox.Show("No student exists with this ID"); //Informs the user that the student does not exist
                    }
                }
            }
            else //Otherwise
            {
                MessageBox.Show("Please select a class"); //Informs user to select a class in the first list box
            }
        }

		private void delBtn_Click(object sender, EventArgs e) //Event handler for when the deletion button is clicked
		{
            if (listBox2.SelectedItem == null && listBox1.SelectedItem != null) //The following occurs when the class is selected to be deleted
            {
                FtpClient clientFTP = new FtpClient();
                clientFTP.Host = "ftp://82.10.84.171";
                clientFTP.Port = 54443;
                clientFTP.Credentials.UserName = "user";
                clientFTP.Credentials.Password = "qwerty";
                clientFTP.DataConnectionReadTimeout = 2147483645;
                clientFTP.ConnectTimeout = 2147483645;
                clientFTP.DataConnectionConnectTimeout = 2147483645;
                clientFTP.ReadTimeout = 2147483645;
                clientFTP.DeleteDirectory(listBox1.SelectedItem.ToString()); //Deletes the directory that was selected
                clientFTP.Disconnect();
                ListDirectory(); //Refreshed the list boxes contents
            }
            else //Otherwise, the selected student inside the classroom is removed
            {
                FtpClient clientFTP = new FtpClient();
                clientFTP.Host = "ftp://82.10.84.171";
                clientFTP.Port = 54443;
                clientFTP.Credentials.UserName = "user";
                clientFTP.Credentials.Password = "qwerty";
                clientFTP.DataConnectionReadTimeout = 2147483645;
                clientFTP.ConnectTimeout = 2147483645;
                clientFTP.DataConnectionConnectTimeout = 2147483645;
                clientFTP.ReadTimeout = 2147483645;
                clientFTP.DeleteFile(listBox1.SelectedItem.ToString() + "/" + listBox2.SelectedItem.ToString()); //Deletes the specific file from the path
                clientFTP.Disconnect();
                ListDirectory(); //Refreshed the list boxes contents
            }
		}       

        private void btnBack_Click(object sender, EventArgs e) //Event handler for when the back button is clicked
        {
            TeacherFormIndex indexForm = new TeacherFormIndex(); //Initialises the index form, shows it, then closes this form
            indexForm.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e) //Event handler for when the form is closed
        {
            base.OnClosed(e);
            if (Application.OpenForms.Count == 1) //If this is the only currently open form, becuase the form was closed manually, not from code
            {
                TeacherFormIndex indexForm = new TeacherFormIndex(); //Initialises the index form and shows it
                indexForm.Show();
            }         
        }
    }
}
