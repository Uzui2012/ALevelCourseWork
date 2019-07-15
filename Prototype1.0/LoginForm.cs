using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Quiziet
{
	public partial class LoginForm : Form
	{
		public LoginForm()
		{
			InitializeComponent();
		}
		
		private void Label3_Click(object sender, EventArgs e) //Opens registration form + closes this form
		{
			RegForm regWindow = new RegForm();
			regWindow.Show();
			this.Close();
		}

		private void BtnLog_Click(object sender, EventArgs e) //Event handler when login button is clicked
        {
            if (checkBoxUser.Checked) //Checks to see if the remember username check box is checked, if so then saves the text from the username input text box to appropriate application properties variables
            {
                Properties.Settings.Default.userName = txtUser.Text;
                Properties.Settings.Default.checkboxRemember = true;
                Properties.Settings.Default.Save();
            }
            else //Else clears the appropraite application properties variables
            {
                Properties.Settings.Default.userName = "";
                Properties.Settings.Default.checkboxRemember = false;
                Properties.Settings.Default.Save();
            }

            string ConnectionString = "Server=127.0.0.1;Database=prototypedatabase;Uid=Killian;Pwd=blackrock;"; //MySQL connection string
            string passHash; //holds hashed password
            string encryptedHash; //holds password after encrypting temporarily
            Global.currentUser = txtUser.Text;
			using (MySqlConnection con = new MySqlConnection(ConnectionString)) //Handles the execution of MySQL scoped commands
			{
				con.Open();
				using (MySqlCommand cmd = con.CreateCommand())
				{
                    cmd.CommandText = "select HashSalt from userpass where UserName=@UserName"; //Input Query to database
                    cmd.Parameters.Add(new MySqlParameter("UserName", txtUser.Text));
                    passHash = Convert.ToBase64String(Global.GetHashFunc(txtPass.Text, Convert.ToString(cmd.ExecuteScalar())));//gets the resulting output of query, converts to string, the obtains the hash, again to a string of the equivalent base 64 digits
                }
                con.Close();
			}
            encryptedHash = Global.EncryptString(passHash); //encrypts password hash 
            using (MySqlConnection con = new MySqlConnection(ConnectionString)) //Again, scope for MySQL query commands
            {
                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select count(*) from userpass where UserName=@UserName and HashPass=@Password";//Input Query to database, will check the number of users with the correct username and encrypted password
                    cmd.Parameters.Add(new MySqlParameter("UserName", txtUser.Text));
                    cmd.Parameters.Add(new MySqlParameter("Password", encryptedHash));
                    int i = Convert.ToInt32(cmd.ExecuteScalar()); //Gets resultant output of query
                    if (i > 0) //If there does exists a correct username to password match
                    {
                        cmd.CommandText = @"select AccountType from prototypedatabase.userpass where username=@UserName";//Input Query to database
                        string accountType = cmd.ExecuteScalar().ToString(); //Gets resultant output of query

                        if (accountType == "teacher")//Figures whether the account is a teacher or student account and opens appropriate form
                        {
                            TeacherFormIndex teacherFormIndex = new TeacherFormIndex();
                            teacherFormIndex.Show();
                            this.Close();
                        }
                        else
                        {
                            StudentFormIndex studentFormIndex = new StudentFormIndex();
                            studentFormIndex.Show();
                            this.Close();
                        }
                    }
                    else //Presents user with error message
                    {
                        MessageBox.Show("UserName or Password Doesn't Exist.");
                    }
                }                
            }
        }

		protected override void OnClosed(EventArgs e) //Event handler when form closes, if it is the only form open at the moment then shuts down application
		{
			base.OnClosed(e);
			if (Application.OpenForms.Count == 1)
				Application.Exit();
		}

        private void LoginForm_Load(object sender, EventArgs e) //Event handler when form is loaded
        {
            if(Properties.Settings.Default.userName != string.Empty)//If there is data in the application property variable "userName"
            {                                                       //and if so, then fills in the text box as appropriate + automatically checks check box
                txtUser.Text = Properties.Settings.Default.userName;
                txtPass.Select();
                checkBoxUser.Checked = true;
            }
        }
    }
}
