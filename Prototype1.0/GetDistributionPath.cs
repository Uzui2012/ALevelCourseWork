using FluentFTP;
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

namespace Quiziet
{
    public partial class GetDistributionPath : Form
    {
        public string className; //Class name variable with public scope
        public GetDistributionPath()
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
            foreach (FtpListItem item in clientFTP.GetListing($@"/Classes/{Global.currentUser}/")) //Runs through all directorys under the teachers name, and adds each to the list box for displaying
            {
                listBox1.Items.Add(item.Name);
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e) //Event handler when submit button is clicked
        {
            if (listBox1.SelectedItem != null) //checks that the user has selected a class name
            {
                className = listBox1.GetItemText(listBox1.SelectedItem); //Assigns variable to that class name
            }
            else
            {
                className = null;
            }
        }        
    }
}
