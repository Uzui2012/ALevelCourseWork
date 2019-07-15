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
    public partial class DetailEntryForm : Form
    {
        //Stores text box input as public string variables
        public string quizTitle;
        public string quizDesc;
        public DetailEntryForm()
        {
            InitializeComponent();
        }       
        
        private void btnSubmit_Click(object sender, EventArgs e) //Event handler when submit button is clicked
        {
            quizDesc = txtDesc.Text;  //Assigns the text inputted to the globally scoped variables  
            quizTitle = txtTitle.Text;
            this.Close();
        }
    }
}
