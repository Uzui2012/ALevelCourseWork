using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quiziet
{
	public partial class RegForm : Form
	{
		
		public RegForm()
		{
			InitializeComponent();
		}

		protected override void OnClosed(EventArgs e) //Event handler when form is closed
        {
			base.OnClosed(e);
			LoginForm loginWindow = new LoginForm(); //Creates a new login form
			loginWindow.Show();
		}
		
		private void BtnReg_Click(object sender, EventArgs e) //Event handler when registration button is clicked
        {
			string accountType; 
			string hashedPassword = Convert.ToBase64String(Global.GetHashFunc(txtPassword.Text)); //Hashes the password input, creating a new salt for this partnered password too
            string encryptedPassword = Global.EncryptString(hashedPassword); //Encrypts the hashed password
            string accountSalt = Global.salt; //Assigns the salt generated
			if (radStudent.Checked == true && txtEmail.Text.Contains("@")){
				accountType = "student";
                try
                {
                    string ConnectionString = "Server=127.0.0.1;Database=prototypedatabase;Uid=Killian;Pwd=blackrock;"; //Connection string 
                    string Query = "INSERT INTO prototypedatabase.userpass(`UserName`, `HashPass`, `UserEmail`, `AccountType`, `HashSalt`) VALUES ('" + txtUserName.Text + "','" + encryptedPassword + "','" + txtEmail.Text + "','" + accountType + "','" + accountSalt + "')"; //Query string
                    MySqlConnection MyConn2 = new MySqlConnection(ConnectionString);
                    MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                    MyConn2.Open();
                    MyConn2.Close();
                    if (accountType == "teacher") //Checks the account type, creates FTP directorys in the appropriate places with the suitible text box text (usernames)
                    {
                        Global.UploadTeacherFileReg(txtUserName.Text);
                    }
                    else
                    {
                        Global.UploadStudentFileReg(txtUserName.Text);
                    }
                    MessageBox.Show("Created your account successfully.");//Informs user that registerying was successful
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);//Informs user of any error in their credentials to log in
                }
            }
            else if(radTeacher.Checked == true && txtEmail.Text.Contains("@")) //Handles the validity of the inputs
            {
				accountType = "teacher";
                try
                {
                    string ConnectionString = "Server=127.0.0.1;Database=prototypedatabase;Uid=Killian;Pwd=blackrock;";
                    string Query = "INSERT INTO prototypedatabase.userpass(`UserName`, `HashPass`, `UserEmail`, `AccountType`, `HashSalt`) VALUES ('" + txtUserName.Text + "','" + encryptedPassword + "','" + txtEmail.Text + "','" + accountType + "','" + accountSalt + "')";
                    MySqlConnection MyConn2 = new MySqlConnection(ConnectionString);
                    MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                    MySqlDataReader MyReader2;
                    MyConn2.Open();
                    MyReader2 = MyCommand2.ExecuteReader();
                    while (MyReader2.Read())
                    {
                    }
                    MyConn2.Close();
                    if (accountType == "teacher")
                    {
                        Global.UploadTeacherFileReg(txtUserName.Text);
                    }
                    else
                    {
                        Global.UploadStudentFileReg(txtUserName.Text);
                    }
                    MessageBox.Show("Created your account successfully.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if(txtEmail.Text.Contains("@")) //checks if account selection is valid
			{
				MessageBox.Show("Please select Account Type"); //Informs user of error
            }
            else //checks if email is valid
            {
                MessageBox.Show("Please input a valid email address"); //Informs user of error
            }			
            Global.salt = ""; //resets global salt variable
        }
	}
}
