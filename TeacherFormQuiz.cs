using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using FluentFTP;

namespace Quiziet
{
	public partial class TeacherFormQuiz : Form
	{
        private List<string[]> listData = new List<string[]>(); //instantiated list of string arrays for use when passing data into the QuizForm form

        public TeacherFormQuiz()
		{
			InitializeComponent();
            ListDirectory(); //Calls the list directory subroutine
        }
		
		private void btnRefresh_Click(object sender, EventArgs e) //Event handler for when the refresh button is clicked
		{
            ListDirectory(); //Calls the list directory subroutine
            label1.Text = "Quiz Title"; //Clears the label to the default text
            label2.Text = "Decription";
        }

        public void ListDirectory() //Subroutine to refresh the list box to the directory of the 
        {
            listBox.Items.Clear(); //Clears the list boxes current items
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://82.10.84.171";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            foreach (FtpListItem item in clientFTP.GetListing("Quizzes/")) //Runs through all the directory listings in the Quizzes directory of the FTP server
            {
                listBox.Items.Add(item.FullName); //Adds each directory to the list box
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e) //Event handler for when a list box item is selected
        {
            string filePath = listBox.GetItemText(listBox.SelectedItem); //Assigns the currently selected directory to a string variable
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://82.10.84.171";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.Connect();
            if (clientFTP.FileExists($@"\{filePath}")) //Checks the quiz exists in the dirctory on the FTP server
            {
                string csvPath = Path.GetTempPath() + "tempcsv.csv";
                clientFTP.DownloadFile(csvPath, $@"\{filePath}"); //Downloads the selected quiz to a temporary CSV file
                List<string[]> quizArray = Global.CSVToArray(csvPath); //passes the CSV data to a list of string arrays
                label1.Text = quizArray[0][0]; //Reads off the quiz title and description, presents that data on the labels of the form
                label2.Text = quizArray[quizArray.Count - 1][1];
                File.Delete(csvPath); //Deletes temporary file
                clientFTP.Disconnect();
            }
            else if(filePath == "") //Checks the file path is empty
            {
                label1.Text = "Quiz Title"; //Simply assigns the labels to their default values
                label2.Text = "Decription";
            }
            else //Otherwise
            {
                MessageBox.Show("File Not Found!"); //Inform the user that the quiz was not found on the FTP server
            }
        }

        private void btnOpen_Click(object sender, EventArgs e) //Event handler for when the open button is clicked 
        {
            string filePath = listBox.GetItemText(listBox.SelectedItem); //Assigns the currently selected directory to a string variable
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://82.10.84.171";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.Connect();
            if (clientFTP.FileExists($@"\{filePath}")) //Checks the quiz exists in the dirctory on the FTP server
            {
                string csvPath = Path.GetTempPath() + "tempcsv.csv";
                clientFTP.DownloadFile(csvPath, $@"\{filePath}"); //Downloads the selected quiz to a temporary CSV file
                QuizForm quizForm = new QuizForm(1, Global.CSVToArray(csvPath)); //Initialises a QuizForm form with the passed data that this is View form type and the list of string array data from the CSV path
                quizForm.Show(); //Shows the QuizFOrm form
                File.Delete(csvPath); //Deletes the temporary CSV file                
            }
            else
            {
                MessageBox.Show("File Not Found!"); //Informs the user that the file was not found                
            }
            clientFTP.Disconnect();
        }             

        private void btnCreate_Click(object sender, EventArgs e) //Event handler for when the create button is clicked
        {
            QuizForm quizForm = new QuizForm(2, null); //Initialises a QuizForm form with the type Create, and null list of string arrays
        }

        private void btnBack_Click(object sender, EventArgs e) //Event handler for when the back button is clicked
        {
            TeacherFormIndex teacherFormIndex = new TeacherFormIndex(); //Initialises a teacher index form, shows it, then closes this form
            teacherFormIndex.Show();
            this.Close();
        }

        private void btnDistribute_Click(object sender, EventArgs e) //Event handler for when the ditribute button is clicked
        {            
            string filePathGet = listBox.GetItemText(listBox.SelectedItem); //Assigns the currently selected directory to a string variable
            if (filePathGet != null) //Checks that the filepath is not empty
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
                if (clientFTP.FileExists($@"/{filePathGet}")) //Checsk the file exists in the file directory of the FTP server
                {
                    string csvPath = Path.GetTempPath() + "tempcsv.csv";
                    clientFTP.DownloadFile(csvPath, filePathGet); //Downloads the file at the direcotry to the temporary CSV file
                    using (GetDistributionPath distriPath = new GetDistributionPath()) //Using the scope of a dialog box
                    {
                        if (distriPath.ShowDialog() == DialogResult.OK) //Given the user selects submit
                        {
                            if (distriPath.className != null) //And has selected a class name
                            {
                                listData = Global.CSVToArray(csvPath); //Passes the downlaoded CSV to a list of string arrays 
                                listData[listData.Count - 1][3] = Global.currentUser; //Adds the data of the teacher that distributed the quiz, and the class name distributed to
                                listData[listData.Count - 1][4] = distriPath.className;
                                Global.ArrayToCSV(listData); //Overwrites that temporary CSV file data with the edit
                                foreach (FtpListItem item in clientFTP.GetListing("/Classes/" + Global.currentUser + "/" + distriPath.className)) //Runs through each existing student in the distribution target class
                                {
                                    clientFTP.UploadFile(csvPath, $@"/Students/{item.Name.Substring(0, item.Name.Length - 4)}/{filePathGet.Substring(8)}"); //Uploads the CSV to every student account under the quiz title
                                }

                                MessageBox.Show("Quiz Distributed Successfully!"); //Informs user that the distribution was successful
                            }
                        }
                    }
                    File.Delete(csvPath); //Deletes the temporary CSV file
                    clientFTP.Disconnect();
                }
                else //Otherwise 
                {
                    MessageBox.Show("File Not Found!"); //Informs the user that the file was not found
                }
            }
        }

        protected override void OnClosed(EventArgs e) //Event handler for when the form is closed
        {
            base.OnClosed(e);
            if (Application.OpenForms.Count == 1) //Shuts the program down when this is the last form open
                Application.Exit();
        }

    }
}
