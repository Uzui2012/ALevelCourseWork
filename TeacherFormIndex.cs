using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quiziet
{
	public partial class TeacherFormIndex : Form
	{
		public TeacherFormIndex()
		{
			InitializeComponent();
		}

		private void btnQuiz_Click(object sender, EventArgs e) //Event handler for when the quiz button is clicked
		{
			TeacherFormQuiz teacherFormQuiz = new TeacherFormQuiz(); //Initialises a teacher quiz form, shows it, closes this form
			teacherFormQuiz.Show();
			this.Close();
		}

		private void btnManager_Click(object sender, EventArgs e) //Event handler for when the manager button is clicked
        {
			TeacherFormManager teacherFormManager = new TeacherFormManager(); //Initialises a teacher manager form, shows it, closes this form
            teacherFormManager.Show();
			this.Close();
		}

        protected override void OnClosed(EventArgs e) //Event handler for when the form is closed
        {
            base.OnClosed(e);
            if (Application.OpenForms.Count == 1) //If this form is closed any other way, i.e manually closed by user, simply initialises a login form and shows it
            {
                LoginForm loginWindow = new LoginForm();
                loginWindow.Show();
            }
        }

        private void btnLogOut_Click(object sender, EventArgs e) //Event handler for when the logout button is clicked
        {
            LoginForm loginWindow = new LoginForm(); ////Initialises a login form, shows it, closes this form
            loginWindow.Show();
            this.Close();
        }
    }
}
