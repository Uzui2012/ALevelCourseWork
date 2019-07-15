using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentFTP;

namespace Quiziet
{
    public partial class QuizForm : Form
    {
        
        private List<string[]> listData = new List<string[]>();
        //Score variable to keep track of student score while partaing the quiz
        private int score;
        //Used for the rest of the program to operate under the correct case type as the passed quizFormType varible is only visible to the initialising method
        private int formType;
        //Keeps track of the current question to be presented to the user, initially set to 1. Is basically a pointer
        private int currentQuestion = 1;
        //Tracks currently selected answer for comparison to actual answer when student partakes in a quiz
        private string selectedAnswer;
        //Main initialising method
        public QuizForm(int quizFormType, List<string[]> quizData)                                          // Quiz Form Types: 
        {                                                                                                   //
            InitializeComponent();                                                                          //  0 = QuizForm when a student is taking the quiz
            formType = quizFormType;//Assigns form type to the class visible variable                       //  1 = QuizForm when a teacher wishes to open and view quiz
            switch(quizFormType)//Takes form type as case selection operitive                               //  2 = QuizForm when a teacher wishes to create and submit a quiz
            {
                case 0: 
                    listData = quizData; //Assigned the passed parameter array to class visible jagged array
                    QuizTypeStudent(); //Calls the initialising method of the type for student
                    this.Text = listData[0][0]; 
                    this.Show();
                    break;
                case 1:
                    listData = quizData;
                    QuizTypeView();
                    txtQuiz.Hide();
                    this.Text = listData[0][0];
                    this.Show();
                    break;
                case 2:
                    QuizTypeCreate();
                    this.Text = "Question " + currentQuestion;
                    break;
            }
        }

        public void QuizTypeView() //Called to initialise the QuizForm in the type 1
        {
            txtQuiz.Hide();  //Hides all textboxes and radio button from user 
            txtBtn1.Hide();
            txtBtn2.Hide();
            txtBtn3.Hide();
            txtBtn4.Hide();
            radBtnBoolean.Hide();
            radBtnMultChoi.Hide();
            radBtn1.Hide();
            radBtn2.Hide();
            radBtn3.Hide();
            radBtn4.Hide();
            btnSubmit.Hide();
            UpdateView(); //Calls to update the form with the list of string array data
        }

        public void UpdateView() //Called to update the view with data in the current question pointer's index
        {
            lblQuiz.Text = listData[currentQuestion][1];
            if (listData[currentQuestion][0] == "MultChoi") //Handles when the quesiton is a multiple choice question, showing appropriate buttons and passing data from the list of string arrays
            {
                btn3.Show();
                btn4.Show();
                btn1.Text = listData[currentQuestion][2];
                btn2.Text = listData[currentQuestion][3];
                btn3.Text = listData[currentQuestion][4];
                btn4.Text = listData[currentQuestion][5];
                if (listData[currentQuestion][6] == listData[currentQuestion][2]) //Checks which answer is corerct, colurs that button green, the rest default
                {
                    btn1.BackColor = Color.Green;
                    btn2.BackColor = default(Color);
                    btn3.BackColor = default(Color);
                    btn4.BackColor = default(Color);
                }
                else if (listData[currentQuestion][6] == listData[currentQuestion][3])
                {
                    btn1.BackColor = default(Color);
                    btn2.BackColor = Color.Green;
                    btn3.BackColor = default(Color);
                    btn4.BackColor = default(Color);
                }
                else if (listData[currentQuestion][6] == listData[currentQuestion][4])
                {
                    btn1.BackColor = default(Color);
                    btn2.BackColor = default(Color);
                    btn3.BackColor = Color.Green;
                    btn4.BackColor = default(Color);
                }
                else if (listData[currentQuestion][6] == listData[currentQuestion][5])
                {
                    btn1.BackColor = default(Color);
                    btn2.BackColor = default(Color);
                    btn3.BackColor = default(Color);
                    btn4.BackColor = Color.Green;
                }
            }
            else //Handles when the question is of boolean true or false, hiding appropriate buttons and passing data from the list of string arrays
            {
                btn1.Text = listData[currentQuestion][2];
                btn2.Text = listData[currentQuestion][3];
                btn3.Hide();
                btn4.Hide();
                if (listData[currentQuestion][6] == "True")//Checks which answer is corerct, colurs that button green, the rest default
                {
                    btn1.BackColor = Color.Green;
                    btn2.BackColor = default(Color);
                }
                else if (listData[currentQuestion][6] == "False")
                {
                    btn1.BackColor = default(Color);
                    btn2.BackColor = Color.Green;
                }
            }
        }

        public void QuizTypeStudent() //Hides textboxes and radio buttons from user, aswell as the submit button
        {            
            txtQuiz.Hide();
            txtBtn1.Hide();
            txtBtn2.Hide();
            txtBtn3.Hide();
            txtBtn4.Hide();
            radBtnBoolean.Hide();
            radBtnMultChoi.Hide();
            radBtn1.Hide();
            radBtn2.Hide();
            radBtn3.Hide();
            radBtn4.Hide();
            btnBack.Hide();
            btnSubmit.Hide();
            UpdateStudent();
        }

        public void UpdateStudent() //assigns text of buttons and labels where appropriate for current quesiton
        {
            lblQuiz.Text = listData[currentQuestion][1];
            if (listData[currentQuestion][0] == "MultChoi")// Performs appropriate assignment dependant on question type
            {
                btn3.Show();
                btn4.Show();
                btn1.Text = listData[currentQuestion][2];
                btn2.Text = listData[currentQuestion][3];
                btn3.Text = listData[currentQuestion][4];
                btn4.Text = listData[currentQuestion][5];
            }
            else
            {
                btn1.Text = listData[currentQuestion][2];
                btn2.Text = listData[currentQuestion][3];
                btn3.Hide();
                btn4.Hide();
            }
        }

        public void QuizTypeCreate()
        {
            radBtnMultChoi.Checked = true;
            btn1.Hide();
            btn2.Hide();
            btn3.Hide();
            btn4.Hide();
            btnSubmit.Hide();
            lblQuiz.Text = "Please enter question:";
            using (DetailEntryForm detailForm = new DetailEntryForm())
            {
                if (detailForm.ShowDialog() == DialogResult.OK)
                {
                    if ((detailForm.quizTitle != null) && (detailForm.quizDesc != null))
                    {
                        this.Show();
                        listData.Add(new string[] { "", "", "", "", "", "", "" }); //Made 3 question indexes with the edge cases appropriate to initialise list
                        listData.Add(new string[] { "", "", "", "", "", "", "" });
                        listData[0][0] = detailForm.quizTitle;
                        listData[listData.Count - 1][1] = detailForm.quizDesc;
                        listData[listData.Count - 1][0] = Global.currentUser;
                        string uniqueCode = Global.GenerateRandomString();
                        listData[listData.Count - 1][2] = uniqueCode.Substring(0, uniqueCode.Length - 2);
                    }
                    else
                    {
                        MessageBox.Show("Please enter all the details for the quiz");
                    }
                }
            }
        }        

        public void DrawNextCreate() //Called to draw/refresh the QuizForm form for the next question 
        {
            this.Text = "Question " + (currentQuestion);//readies form for next question
            txtBtn1.Text = "";
            txtBtn2.Text = "";
            txtBtn3.Text = "";
            txtBtn4.Text = "";
            txtQuiz.Text = "";
            radBtnMultChoi.Checked = true;
            radBtn1.Checked = true;
        }

        public void DrawBackCreate() //Called to draw/refresh the QuizForm form for the previous question
        {
            if (listData[currentQuestion-1][0] == "Boolean") //Checks the question type and approriately assigns text boxes + radio buttons with the list of string arrays data where appropriate
            {
                radBtnBoolean.Checked = true;
                txtQuiz.Text = listData[currentQuestion-1][1];
                txtBtn1.Text = listData[currentQuestion-1][2];
                txtBtn2.Text = listData[currentQuestion-1][3];
                if (listData[currentQuestion-1][6] == "False")
                {
                    radBtn2.Checked = true;
                }
                else
                {
                    radBtn1.Checked = true;
                }
            }
            else
            {
                radBtnMultChoi.Checked = true;
                txtQuiz.Text = listData[currentQuestion-1][1];
                txtBtn1.Text = listData[currentQuestion-1][2];
                txtBtn2.Text = listData[currentQuestion-1][3];
                txtBtn3.Text = listData[currentQuestion-1][4];
                txtBtn4.Text = listData[currentQuestion-1][5];
                for (int i = 2; i < 6; i++)
                {
                    if (listData[currentQuestion-1][i] == listData[currentQuestion-1][6])
                    {
                        switch (i)
                        {
                            case 2:
                                radBtn1.Checked = true;
                                break;
                            case 3:
                                radBtn2.Checked = true;
                                break;
                            case 4:
                                radBtn3.Checked = true;
                                break;
                            case 5:
                                radBtn4.Checked = true;
                                break;
                        }
                    }
                }
            }
            if (currentQuestion-1 == 2) //When going backwards past question 3, hides the submit button (handling input validity)
            {
                btnSubmit.Hide();
            }
            this.Text = "Question " + (currentQuestion-1);
        }

        public void CreateNext() //Performs the assigning and logic when the next button is clicked
        {
            listData.Insert(listData.Count-1, new string[] { "", "", "", "", "", "", "" }); //Adds new string array into the list of string arrays
            if ((txtQuiz.Text == "") || (txtBtn1.Text == "") || (txtBtn2.Text == "") || (txtBtn3.Text == "") || (txtBtn4.Text == ""))//checks all data is full
            {
                listData.RemoveAt(currentQuestion);
                MessageBox.Show("Please complete all details of question!");
                currentQuestion--; //increments pointer
            }
            else//if full of data then assigns list with data from text boxes and radio buttons
            {
                if (radBtnBoolean.Checked == true)
                {
                    listData[currentQuestion][0] = "Boolean";
                }
                else
                {
                    listData[currentQuestion][0] = "MultChoi";
                }
                listData[currentQuestion][1] = txtQuiz.Text;
                listData[currentQuestion][2] = txtBtn1.Text;
                listData[currentQuestion][3] = txtBtn2.Text;
                listData[currentQuestion][4] = txtBtn3.Text;
                listData[currentQuestion][5] = txtBtn4.Text;
                if (radBtn1.Checked == true)
                {
                    listData[currentQuestion][6] = txtBtn1.Text;
                }
                else if (radBtn2.Checked == true)
                {
                    listData[currentQuestion][6] = txtBtn2.Text;
                }
                else if (radBtn3.Checked == true)

                {
                    listData[currentQuestion][6] = txtBtn3.Text;
                }
                else
                {
                    listData[currentQuestion][6] = txtBtn4.Text;
                }
            }
        }

        public void CreateUndo() //Performs the logic when the back button is clicked
        {
            listData.RemoveAt(listData.Count - 2); //Simply removes latest question's string array, then increments pointer
            currentQuestion--;
        }

        private void BtnNext_Click(object sender, EventArgs e) //Event handler for when the next button is clicked
        {            
            switch (formType) //Switch case statement for the initialised quiz type 
            { 
                case 0: //If it is a Student quiz type
                    btnBack.Text = "Back";
                    if (currentQuestion == listData.Count - 2) //If the current question is the last, then it submits the score
                    {
                        if (selectedAnswer == listData[currentQuestion][6])
                        {
                            score++;
                        }
                        using (SubmitConfirmForm submit = new SubmitConfirmForm())
                        {
                            if (submit.ShowDialog() == DialogResult.OK)
                            {
                                string teacherAccount = listData[listData.Count - 1][3]; //Gets required data from the list of string arrays
                                string className = listData[listData.Count - 1][4];
                                string csvPath = Path.GetTempPath() + "tempcsv.csv";    
                                FtpClient clientFTP = new FtpClient();
                                clientFTP.Host = "ftp://82.10.84.171";
                                clientFTP.Port = 54443;
                                clientFTP.Credentials.UserName = "user";
                                clientFTP.Credentials.Password = "qwerty";
                                clientFTP.DataConnectionReadTimeout = 2147483645;
                                clientFTP.ConnectTimeout = 2147483645;
                                clientFTP.DataConnectionConnectTimeout = 2147483645;
                                clientFTP.ReadTimeout = 2147483645;
                                clientFTP.DownloadFile(csvPath, $@"/Classes/{teacherAccount}/{className}/{Global.currentUser}.csv"); //obtains the current state of the students particular csv
                                using (StreamWriter csvData = File.AppendText(csvPath))
                                {
                                    csvData.WriteLine($"\n{listData[0][0]},{score}"); //Writes the quiz title and resultant score
                                }
                                clientFTP.DeleteFile($@"/Students/{Global.currentUser}/{listData[0][0]}.csv"); //Delete appropriate files 
                                clientFTP.DeleteFile( $@"/Classes/{teacherAccount}/{className}/{Global.currentUser}.csv");
                                clientFTP.UploadFile(csvPath, $@"/Classes/{teacherAccount}/{className}/{Global.currentUser}.csv"); //Upload edited file                               
                                File.Delete(csvPath);
                                MessageBox.Show("Successfully Submitted"); //Inform user the score was successfully submitted
                                StudentFormIndex studentFormIndex = new StudentFormIndex(); //Go back to student index form and close this form
                                studentFormIndex.Show();
                                this.Close();
                            }
                        }
                    }
                    else if (currentQuestion == listData.Count - 3) //If the current question is the second to last
                    {
                        btn1.Enabled = true; //Perform the usual
                        btn2.Enabled = true;
                        btn3.Enabled = true;
                        btn4.Enabled = true;
                        if (selectedAnswer == listData[currentQuestion][6])
                        {
                            score++;
                        }
                        btnNext.Text = "Submit"; //Simply rename the next button text Submit
                        currentQuestion++;
                        UpdateStudent();
                    }
                    else
                    {
                        btn1.Enabled = true; //Re-enable all buttons
                        btn2.Enabled = true;
                        btn3.Enabled = true;
                        btn4.Enabled = true;
                        if (selectedAnswer == listData[currentQuestion][6]) //Check if the answer was correct, if so then increment the score counter
                        {
                            score++;
                        }
                        currentQuestion++; //increment current question pointer
                        UpdateStudent();
                    }
                    break;
                case 1: //If it is a View quiz type
                    currentQuestion++;
                    btnBack.Text = "Back";
                    if (currentQuestion == listData.Count - 1) //If current question is on the last question
                    {
                        currentQuestion = 1; //Loop back to the first
                        btnBack.Text = "Close";
                    }
                    UpdateView();
                    break;
                case 2: //If it is a Creation quiz type                   
                    btnBack.Text = "Back";
                    if (currentQuestion == 19) 
                    {
                        btnNext.Hide(); //When on last question, hide the next button so only back and submit are options for the user
                        CreateNext();
                        currentQuestion++;
                        DrawNextCreate();
                    }
                    else if (currentQuestion == 2)
                    {
                        btnSubmit.Show(); //When past the 2nd question, enable user to submit whenever they want
                        CreateNext();
                        currentQuestion++;
                        DrawNextCreate();
                    }
                    else
                    {
                        CreateNext(); //Applie creating next logic
                        currentQuestion++; //Increment current question pointer
                        DrawNextCreate(); //Draw a fresh question on form
                    }                   
                    break;
            }
        }       

        private void BtnBack_Click(object sender, EventArgs e) //Event handler for when the next button is clicked
        {
            if (currentQuestion-1 == 0) //If the prior question is 0 (currently on question 1)
            {
                this.Close();   //Then simply close the form
            }else if (currentQuestion-1 == 1)   //If the prior question is 1 (currently on 2)
            {
                btnBack.Text = "Close"; //Change text on the back button to "Close"
            }
            else //Anything else
            {
                btnNext.Text = "Next"; //Ensure next buttons text is "Next" (changes when it says submit)
            }
            switch (formType) 
            {
                case 1: //If formType is the View form type
                    currentQuestion--; //Increment current question pointer backwards 1
                    UpdateView(); //Update/Redraw the form to the previous quesiton
                    break;
                case 2: //If formType is the Creation form type
                    DrawBackCreate(); //Draw the previous question with the list of string arrays data
                    CreateUndo(); //Perform the logic to go back a question                               
                    break;
            }
        }

        private void RadBtnBoolean_CheckedChanged(object sender, EventArgs e)//Event handler for when the question type radio buttons are changed
        {
            if (radBtnBoolean.Checked == true) //If it has been switched to the boolean radio button
            {
                txtBtn3.Hide(); //Hide and edit text boxes and radio buttons where appropriate
                txtBtn4.Hide();
                radBtn1.Checked = true;
                radBtn3.Hide();
                radBtn4.Hide();
                txtBtn1.Text = "True";
                txtBtn2.Text = "False";
                txtBtn1.ReadOnly = true;
                txtBtn2.ReadOnly = true;
                txtBtn3.Text = "null";
                txtBtn4.Text = "null";
            }
            else //If it has been switched to the multiple choice radio button
            {
                radBtn1.Checked = true; //Hide and edit text boxes and radio buttons where appropriate
                txtBtn3.Show();
                txtBtn4.Show();
                radBtn3.Show();
                radBtn4.Show();
                txtBtn1.ReadOnly = false;
                txtBtn2.ReadOnly = false;
                txtBtn1.Text = "";
                txtBtn2.Text = "";
                txtBtn3.Text = "";
                txtBtn4.Text = "";
            }
        }

        private void Btn1_Click(object sender, EventArgs e) //Event handler for when the button 1 is clicked
        {
            if (formType == 0) //Only in the case that the form type is the Student quiz form
            {
                btn1.Enabled = false; //Enable/disable appropiate buttons, assign the selected answer variable to the selected button's text
                btn2.Enabled = true;
                btn3.Enabled = true;
                btn4.Enabled = true;
                selectedAnswer = btn1.Text;
            }
        }

        private void Btn2_Click(object sender, EventArgs e)//Event handler for when the button 2 is clicked
        {
            if (formType == 0) //Only in the case that the form type is the Student quiz form
            {
                btn2.Enabled = false; //Enable/disable appropiate buttons, assign the selected answer variable to the selected button's text
                btn1.Enabled = true;
                btn3.Enabled = true;
                btn4.Enabled = true;
                selectedAnswer = btn2.Text;
            }
        }

        private void Btn3_Click(object sender, EventArgs e)//Event handler for when the button 3 is clicked
        {
            if (formType == 0) //Only in the case that the form type is the Student quiz form
            {
                btn3.Enabled = false; //Enable/disable appropiate buttons, assign the selected answer variable to the selected button's text
                btn2.Enabled = true;
                btn1.Enabled = true;
                btn4.Enabled = true;
                selectedAnswer = btn3.Text;
            }
        }

        private void Btn4_Click(object sender, EventArgs e)//Event handler for when the button 4 is clicked
        {
            if (formType == 0) //Only in the case that the form type is the Student quiz form
            {
                btn4.Enabled = false; //Enable/disable appropiate buttons, assign the selected answer variable to the selected button's text
                btn1.Enabled = true;
                btn2.Enabled = true;
                btn3.Enabled = true;
                selectedAnswer = btn4.Text;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)//Event handler for when the submit button is clicked
        {
            if (!((txtQuiz.Text == "") || (txtBtn1.Text == "") || (txtBtn2.Text == "") || (txtBtn3.Text == "") || (txtBtn4.Text == ""))) //Ensures that all text input is filled in where required of the data handling
            {
                listData.Insert(listData.Count - 1, new string[] { "", "", "", "", "", "", "" }); //Insert a new empty string array to the list 
                if (radBtnBoolean.Checked == true) //Assign data for question type
                {
                    listData[listData.Count - 2][0] = "Boolean";
                }
                else
                {
                    listData[listData.Count - 2][0] = "MultChoi";
                }
                listData[listData.Count - 2][1] = txtQuiz.Text; //Assign text box data to the string array
                listData[listData.Count - 2][2] = txtBtn1.Text;
                listData[listData.Count - 2][3] = txtBtn2.Text;
                listData[listData.Count - 2][4] = txtBtn3.Text;
                listData[listData.Count - 2][5] = txtBtn4.Text;
                if (radBtn1.Checked == true) //Assign the correct answer data to the string array
                {
                    listData[listData.Count - 2][6] = txtBtn1.Text;
                }
                else if (radBtn2.Checked == true)
                {
                    listData[listData.Count - 2][6] = txtBtn2.Text;
                }
                else if (radBtn3.Checked == true)

                {
                    listData[listData.Count - 2][6] = txtBtn3.Text;
                }
                else
                {
                    listData[listData.Count - 2][6] = txtBtn4.Text;
                }
                using (SubmitConfirmForm submit = new SubmitConfirmForm()) //Submition form scope
                {
                    if (submit.ShowDialog() == DialogResult.OK) //If the confirmation of submission is achieved
                    {                        
                        for (int i = 1; i < 7; i++) //Insert null data into the appropriate places across the created list of string arrays where data must exist to properly enable the CSVs to be read later
                        {
                            listData[0][i] = "null";
                        }
                        for (int i = 3; i < 7; i++)
                        {
                            listData[listData.Count - 1][i] = "null";
                        }
                        Global.ArrayToCSV(listData); //Create a temporary csv with the list of string array data
                        string csvPath = Path.GetTempPath() + "tempcsv.csv"; //Get that path to the temp folder
                        FtpClient clientFTP = new FtpClient();
                        clientFTP.Host = "ftp://82.10.84.171";
                        clientFTP.Port = 54443;
                        clientFTP.Credentials.UserName = "user";
                        clientFTP.Credentials.Password = "qwerty";
                        clientFTP.DataConnectionReadTimeout = 2147483645;
                        clientFTP.ConnectTimeout = 2147483645;
                        clientFTP.DataConnectionConnectTimeout = 2147483645;
                        clientFTP.ReadTimeout = 2147483645;
                        clientFTP.UploadFile(csvPath, $@"\Quizzes\{listData[0][0]}.csv"); //Upload that csv under the Quizzes name found inside the list of string array data
                        File.Delete(csvPath); //Delete that temporary CSV
                        clientFTP.Disconnect();
                        MessageBox.Show("Success, Quiz Uploaded!");//Inform user uploading of the quiz was successful
                        this.Close();
                    }
                    else //Otherwise undo everything occured so far by sinmply removing the inserted array
                    {
                        listData.RemoveAt(listData.Count - 2);
                    }
                }
            }
            else //Informs user to fulfil data input
            {
                MessageBox.Show("Please complete all fields!");
            }
        }        
    }
}
