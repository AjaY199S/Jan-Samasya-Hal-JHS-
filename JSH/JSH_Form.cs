using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

// For Firebase connecting Librarys
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace JSH
{
    public partial class JSH_Form : Form
    {
        // Firebase connection conction with security
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "xkIeGAjTC8RzDJhTfZlevnXDonOAJmDgVOUqmelh",
            BasePath = "https://jshfirebaseproject.firebaseio.com/"
        };

        // Firense client
        IFirebaseClient client;
        public JSH_Form()
        {
            InitializeComponent();
        }

        // SQL_Connection Class Decleare Here
        SQL_Connections SQL = new SQL_Connections();

        // global variabal
        string U_Reg_no;
        string U_Problem_Domain;

        private void JSH_Form_Load(object sender, EventArgs e)
        {
           
            // Firebse configure
            try
            {
                client = new FireSharp.FirebaseClient(config);
               /* if (client != null)
                {
                    MessageBox.Show("connection is stablished");
                }
                else
                {
                    MessageBox.Show("connection not");
                }*/
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sothing Internet connection Problem Plese check your internet connection"+ex);
            }

            // Innitial Deletion Opration
            if(Table_Delete_Function())
            {
                MessageBox.Show(" Error ocured Recorders are not deleted ");
            }

            // Panal Visibalty
            Sign_Up_Pannal.Visible = false;

            //User panal
            User.Visible = false;
            U_Home.Visible = false;
            U_Profile.Visible = false;
            U_Problem.Visible = false;
            U_Status.Visible = false;

            //Admin Panals
            Admin.Visible = false;
            A_Home.Visible = false;
            A_Profile.Visible = false;
            A_View_Problem_Data.Visible = false;
            A_Entity_Info.Visible = false;
            A_Add_Update.Visible = false;

            //Referal Panals
            Referal.Visible = false;
            R_Home.Visible = false;
            R_Profile.Visible = false;
            R_View_Problem_Data.Visible = false;
            R_Update_Status.Visible = false;


        }

        // Import Problem Data From Firebase
        private async void Problem_Data_Import()
        {
            try
            {
                bool flag = true;
                User_Data D = new User_Data();

                // Imort Problem count
                FirebaseResponse p_respo = await client.GetTaskAsync("Counting/Node");
                User_Data p_data = p_respo.ResultAs<User_Data>();

                //conver string to inteager
                string u_count = p_data.User_count;
                int p_count = Int32.Parse(p_data.Problem_count);

                for (int i = 1; i <= p_count; i++)
                {
                    // Get User Probelm Data to firebase import
                    FirebaseResponse up_respo = await client.GetTaskAsync("User_Problem_Data/" + i);
                    User_Data up_data = up_respo.ResultAs<User_Data>();

                    // Data assign up_data to User_data
                    var sql_data = new User_Data
                    {
                        User_Reg_no = up_data.User_Reg_no,
                        Referal_Reg_no = up_data.Referal_Reg_no,
                        problem_id = up_data.problem_id,
                        Block = up_data.Block,
                        Room_no = up_data.Room_no,
                        Problem = up_data.Problem,
                        Discription = up_data.Discription,
                        Status = up_data.Status

                    };

                    if (SQL.Problem_Data_Insert(sql_data))
                    {
                        flag = false;
                    }
                }

                if(flag)
                {
                    MessageBox.Show("Insertion Faild");
                }
                else
                {
                    MessageBox.Show(" Insertion Scussefull ");
                }

        }
        catch (Exception ex)
        {
             MessageBox.Show(" Error : " + ex);
        }


        }

        // Import User Data From Firebase
        private async void User_Data_Import()
        {
            try
            {
                bool flag = true;
                User_Data D = new User_Data();

                // Imort User count
                FirebaseResponse u_respo = await client.GetTaskAsync("Counting/Node");
                User_Data u_data = u_respo.ResultAs<User_Data>();

                //conver string to inteager
                string p_count = u_data.Problem_count;
                int u_count = Int32.Parse(u_data.User_count);

                for (int i = 1; i <= u_count; i++)
                {
                    // Get User Uniqe id to firebase import
                    FirebaseResponse uq_respo = await client.GetTaskAsync("Uniqe_User_Id/" + i);
                    User_Data uq_data = uq_respo.ResultAs<User_Data>();

                    // Get User Data to firebase import
                    FirebaseResponse up_respo = await client.GetTaskAsync("User_Data/"+uq_data.Uniqe_Reg_no);
                    User_Data up_data = up_respo.ResultAs<User_Data>();

                    // Data assign up_data to User_data
                    var sql_data = new User_Data
                    {
                        Reg_no = up_data.Reg_no,
                        Full_name = up_data.Full_name,
                        Contact = up_data.Contact,
                        Email_id = up_data.Email_id,
                        Gender = up_data.Gender,
                        Designation = up_data.Designation,
                        Problem_Domain = up_data.Problem_Domain

                    };

                    if (SQL.User_Data_Insert(sql_data))
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    MessageBox.Show("Insertion Faild");
                }
                else
                {
                    MessageBox.Show(" Insertion Scussefull ");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }


        }

        // Intial Table data I sdeleted into  databasen all record
        private bool Table_Delete_Function()
        {
            bool flag = false;
            try
            {
                if(SQL.Problem_Data_Delete() && SQL.User_Data_Delete() && SQL.Delete_User_View())
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
            return flag;
        }
        
        /// <summary>
        /// -------------------------------- Entity Oprations ----------------------------
        /// </summary>
        
        // Entiry Login button
        private async void Login_btn_ClickAsync(object sender, EventArgs e)
        {
            
            try
            {
                bool flag = true;

                if(Regno_input.Text == "" || Password_input.Text == "")
                {
                    MessageBox.Show(" ** Please fill All Fields ");
                }
                else
                {
                    // Get User Count to firebase import
                    FirebaseResponse c_respo = await client.GetTaskAsync("Counting/Node");
                    User_Data c_data = c_respo.ResultAs<User_Data>();

                    //conver string to inteager
                    int count = Int32.Parse(c_data.User_count);

                    // check Reg no duplicatin firebase
                    for (int i = 1; i <= count; i++)
                    {
                        // Get User Uniqe id to firebase import
                        FirebaseResponse u_respo = await client.GetTaskAsync("Uniqe_User_Id/" + i);
                        User_Data u_data = u_respo.ResultAs<User_Data>();

                        //check dupicat Reg no
                        if (u_data.Uniqe_Reg_no == Regno_input.Text)
                        {
                            flag = false;
                        }
                    }

                    if(flag)
                    {
                        MessageBox.Show(" This Reg no is not exist Plaese Enter Valid Reg no ", " Warning ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        FirebaseResponse respo = await client.GetTaskAsync("User_Data/" + Regno_input.Text);
                        User_Data data = respo.ResultAs<User_Data>();

                        if(data.User_Password == Password_input.Text)
                        {
                            if (data.Reg_no == Regno_input.Text && data.Designation == "Admin")
                            {
                                // Import Problem_Data in Local database
                                Problem_Data_Import();

                                // Import User_Data in Local database
                                User_Data_Import();

                                // Assign Uniqe Reg_No
                                U_Reg_no = data.Reg_no;

                                // Peneal Visibility
                                Admin.Visible = true;
                                A_Home.Visible = true;
                                Login.Visible = false;
                                Logo.Visible = false;


                                // Inputs are clear
                                Regno_input.Clear();
                                Password_input.Clear();
                            }

                            else if(data.Reg_no == Regno_input.Text && data.Designation == "User")
                            {
                                // Import Problem_Data in Local database
                                Problem_Data_Import();

                                // Assign Uniqe Reg_No
                                U_Reg_no = data.Reg_no;

                                // DataGrid View Opration
                                bool a = SQL.Delete_User_View();
                                if (a)
                                {
                                    // Create User View for user perpose
                                    if(SQL.Create_User_View(U_Reg_no))
                                    {
                                        DataTable dt = SQL.User_View_Select();
                                        U_Problem_Data_Grid_View.DataSource = dt;
                                    }
                                    
                                }

                                // Peneal Visibility
                                User.Visible = true;
                                U_Home.Visible = true;
                                Login.Visible = false;
                                Logo.Visible = false;

                                // Inputs are clear
                                Regno_input.Clear();
                                Password_input.Clear();
                            }
                            else if (data.Reg_no == Regno_input.Text && data.Designation == "Referal")
                            {
                                // Import Problem_Data in Local database
                                Problem_Data_Import();

                                // Assign Uniqe Reg_No
                                U_Reg_no = data.Reg_no;
                                U_Problem_Domain = data.Problem_Domain;

                                // DataGrid View Opration
                                bool a = SQL.Delete_User_View();
                                if (a)
                                {
                                    // Create User View for user perpose
                                    if (SQL.R_Create_User_View(U_Problem_Domain))
                                    {
                                        DataTable dt = SQL.User_View_Select();
                                        R_View_Problem_Data_DataGrid.DataSource = dt;
                                    }

                                }

                                // Peneal Visibility
                                Referal.Visible = true;
                                R_Home.Visible = true;
                                Login.Visible = false;
                                Logo.Visible = false;

                                // Inputs are clear
                                Regno_input.Clear();
                                Password_input.Clear();

                            }
                        }
                        else
                        {
                            MessageBox.Show(" **Wrong Password ");
                            Password_input.Clear();
                        }
                    }

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
                  
        }

        // New User Sign Up Button  
        private async void Sign_up_btn_Click(object sender, EventArgs e)
        {
            try
            {
                bool flag = true;
                // Check All Fild are Fill or  not
                if (S_Reg_no.Text =="" || S_Name.Text == "" || S_Contact_no.Text == "" || S_Email.Text == "" || S_Gender.Text == "" || S_Password.Text == "" || S_Re_Password.Text == "")
                {
                    MessageBox.Show(" **Please Fill All Area ");
                }
                else
                {
                    // Get User Count to firebase import
                    FirebaseResponse c_respo = await client.GetTaskAsync("Counting/Node");
                    User_Data c_data = c_respo.ResultAs<User_Data>();

                    //conver string to inteager
                    int count = Int32.Parse(c_data.User_count);
                    string p = c_data.Problem_count;


                    // check Reg no duplicatin firebase
                    for (int i = 1; i <= count; i++)
                    {
                        // Get User Uniqe id to firebase import
                        FirebaseResponse u_respo = await client.GetTaskAsync("Uniqe_User_Id/" + i);
                        User_Data u_data = u_respo.ResultAs<User_Data>();

                        //check dupicat Reg no
                        if (u_data.Uniqe_Reg_no == S_Reg_no.Text)
                        {
                            MessageBox.Show(" This Reg no is Already exist Plaese Enter Valid Reg no ", " Warning ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            S_Reg_no.Clear();
                            flag = false;
                        }

                    }

                    // check Reg no.
                    if(flag)
                    {
                        //check password maching
                        if (S_Re_Password.Text != S_Password.Text)
                        {
                            errorProvider1.SetError(this.S_Re_Password, " ** Password is not match ");
                            S_Re_Password.Clear();
                        }

                        else if (S_Re_Password.Text == S_Password.Text)
                        {
                            errorProvider1.Clear();

                            var s_data = new User_Data
                            {
                                Reg_no = S_Reg_no.Text,
                                Full_name = S_Name.Text,
                                Contact = S_Contact_no.Text,
                                Email_id = S_Email.Text,
                                Gender = S_Gender.Text,
                                User_Password = S_Password.Text,
                                Designation = "User",
                                Problem_Domain = ""
                            };

                            SetResponse S_response = await client.SetTaskAsync("User_Data/" + S_Reg_no.Text, s_data);
                            User_Data result = S_response.ResultAs<User_Data>();
                            // Count Inceased
                            count++;
                            string count1 = count.ToString();

                            var c1_data = new User_Data
                            {
                                User_count = count1,
                                Problem_count = p
                                
                            };
                            // Set User Count to firebase Export
                            FirebaseResponse c1_respo = await client.SetTaskAsync("Counting/Node",c1_data);
                            User_Data c1_result = c1_respo.ResultAs<User_Data>();

                            // Unire Id updated
                            var u1_data = new User_Data
                            {
                                Uniqe_Reg_no = S_Reg_no.Text,
                                User_Id = count1
                            };
                            // Set User Count to firebase Export
                            FirebaseResponse u1_respo = await client.SetTaskAsync("Uniqe_User_Id/"+count1, u1_data);
                            User_Data u1_result = u1_respo.ResultAs<User_Data>();

                            // clear all  fileds
                            S_Reg_no.Clear();
                            S_Name.Clear();
                            S_Contact_no.Clear();
                            S_Email.Clear();
                            S_Gender.ResetText();
                            S_Password.Clear();
                            S_Re_Password.Clear();

                            MessageBox.Show("User Singup Succesfully");
                        }
                    }
                    
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
                
        }

        // For New User link or Signup Page open
        private void New_user_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Sign_Up_Pannal.Visible = true;
            Login.Visible = false;
            Logo.Visible = true;

        }

        // SignUp to Login Page Link
        private void M_Login_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Sign_Up_Pannal.Visible = false;
            Login.Visible = true;
            Logo.Visible = true;
        }

        // Froget Password Opration
        private void Forget_Password_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Forget Button
            
           
                MessageBox.Show(" Forget you password ");
           

        }

        /// <summary>
        /// -------------------------- User Start ---------------------------------------------
        /// </summary>

        // User Home Pie Chart Create Function
        private void User_Home_Pie_Chart()
        {
            DataTable dt = SQL.User_View_Pie_Chart();

            string[] Problem = new string[dt.Rows.Count];
            int[] problem_count = new int[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Problem[i] = dt.Rows[i][1].ToString();
                problem_count[i] = Convert.ToInt32(dt.Rows[i][0]);
            }

            User_Chart.Series["Series1"].Points.DataBindXY(Problem, problem_count);
        }

        // User Pananl Create Pie Chart for User Opration
        private void U_Home_Paint(object sender, PaintEventArgs e)
        {
            User_Home_Pie_Chart();
        }

        // User -> Home Button
        private void U_Home_btn_Click(object sender, EventArgs e)
        {
            // User Home Pie chart Created 
            User_Home_Pie_Chart();

            // Panal Visibility Opration
            U_Home.Visible = true;
            U_Profile.Visible = false;
            U_Problem.Visible = false;
            U_Status.Visible = false;
        }
        
        // User -> Profile Button Opration
        private async void U_Profile_btn_Click(object sender, EventArgs e)
        {
            // Data Extract  from fiebase import
            FirebaseResponse respo = await client.GetTaskAsync("User_Data/" + U_Reg_no);
            User_Data data = respo.ResultAs<User_Data>();

            // data assing in text
            U_Profile_Reg_no.Text = data.Reg_no;
            U_Profile_Name.Text = data.Full_name;
            U_Profile_Contact_no.Text = data.Contact;
            U_Profile_Email.Text = data.Email_id;
            U_Profile_Gender.Text = data.Gender;

            // Panals visibility
            U_Home.Visible = false;
            U_Profile.Visible = true;
            U_Problem.Visible = false;
            U_Status.Visible = false;


        }

        // User -> Profile -> Password Change -> Submit Button Opration
        private async void U_Profile_Change_Submit_btn_Click(object sender, EventArgs e)
        {
            if (U_Profile_Change_Password.Text == "" || U_Profile_Change_Re_Password.Text == "")
            {
                MessageBox.Show(" **Please Fill these field ");

            }
            else if (U_Profile_Change_Password.Text != U_Profile_Change_Re_Password.Text)
            {
                MessageBox.Show(" **Passwords dose not match ");
                U_Profile_Change_Re_Password.Clear();
            }
            else if (U_Profile_Change_Password.Text == U_Profile_Change_Re_Password.Text)
            {
                // Update new password
                var s_data = new User_Data
                {
                    Reg_no = U_Profile_Reg_no.Text,
                    Full_name = U_Profile_Name.Text,
                    Contact = U_Profile_Contact_no.Text,
                    Email_id = U_Profile_Email.Text,
                    Gender = U_Profile_Gender.Text,
                    User_Password = U_Profile_Change_Password.Text,
                    Designation = "User",
                    Problem_Domain = ""
                };

                SetResponse S_response = await client.SetTaskAsync("User_Data/" + U_Reg_no, s_data);
                User_Data result = S_response.ResultAs<User_Data>();

                // Inputs clear
                U_Profile_Change_Password.Clear();
                U_Profile_Change_Re_Password.Clear();
            }

        }

        // User -> Profile -> Password Change -> Reset Button Opration
        private void U_Profile_Change_Reset_btn_Click(object sender, EventArgs e)
        {
            // Inputs clear
            U_Profile_Change_Password.Clear();
            U_Profile_Change_Re_Password.Clear();
        }

        // User -> Problem Button Opration
        private void U_Problem_btn_Click(object sender, EventArgs e)
        {
            U_Home.Visible = false;
            U_Profile.Visible = false;
            U_Problem.Visible = true;
            U_Status.Visible = false;
        }

        // User -> Problem -> Submit Button Opration
        private async void U_P_Submit_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Input check fill or not
                if (U_P_Block.Text == "" || U_P_Room_no.Text == "" || U_P_Problem.Text == "" || U_P_Discripption_rich.Text == "")
                {
                    MessageBox.Show(" ** Plese  Fill All Field ");
                }
                else
                {
                    // Get User Count to firebase import
                    FirebaseResponse c_respo = await client.GetTaskAsync("Counting/Node");
                    User_Data c_data = c_respo.ResultAs<User_Data>();

                    //conver string to inteager
                    int p_count = Int32.Parse(c_data.Problem_count);
                    string c = c_data.User_count;

                    // Problem Count Inceased
                    p_count++;
                    string p_count1 = p_count.ToString();

                    // Data Decliaration For sending on database
                    var p_data = new User_Data
                    {
                        User_Reg_no = U_Reg_no,
                        Referal_Reg_no = "",
                        problem_id = p_count1,
                        Block = U_P_Block.Text,
                        Room_no = U_P_Room_no.Text,
                        Problem = U_P_Problem.Text,
                        Discription = U_P_Discripption_rich.Text,
                        Status = "No Status"

                    };

                    SetResponse S_response = await client.SetTaskAsync("User_Problem_Data/" + p_count1, p_data);
                    User_Data result = S_response.ResultAs<User_Data>();




                    //Defineing counting data
                    var c1_data = new User_Data
                    {
                        User_count = c,
                        Problem_count = p_count1

                    };
                    // Set User Count to firebase Export
                    FirebaseResponse c1_respo = await client.SetTaskAsync("Counting/Node", c1_data);
                    User_Data c1_result = c1_respo.ResultAs<User_Data>();

                    MessageBox.Show(" Problem Submited Successfully ");

                    //Inputs Clear
                    U_P_Block.ResetText();
                    U_P_Room_no.Clear();
                    U_P_Problem.ResetText();
                    U_P_Discripption_rich.Clear();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
        }

        // User -> Problem -> Reset Button Opration
        private void U_P_Reset_btn_Click(object sender, EventArgs e)
        {
            //Inputs Clear
            U_P_Block.ResetText();
            U_P_Room_no.Clear();
            U_P_Problem.ResetText();
            U_P_Discripption_rich.Clear();
        }

        // User-> Status Button Opration
        private void U_Status_btn_Click(object sender, EventArgs e)
        {
            // DataGrid View Opration
            if (SQL.Delete_User_View())
            {
                // Create User View for user perpose
                if (SQL.Create_User_View(U_Reg_no))
                {
                    DataTable dt = SQL.User_View_Select();
                    U_Problem_Data_Grid_View.DataSource = dt;
                }

            }

            //Panal Visibility
            U_Home.Visible = false;
            U_Profile.Visible = false;
            U_Problem.Visible = false;
            U_Status.Visible = true; 

        }

        // User -> Status -> Search Opration
        private void U_Status_Search_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = SQL.User_View_Search(U_Status_Search.Text);
            U_Problem_Data_Grid_View.DataSource = dt;
        }

        //User -> Reload Button Opration
        private void U_Reload_btn_Click(object sender, EventArgs e)
        {
            // Innitial Deletion Opration
            bool a = SQL.Problem_Data_Delete();
            bool b = SQL.Delete_User_View();
            if (a && b)
            {
                //Reload Problem data to the firebase
                Problem_Data_Import();
                MessageBox.Show(" Reloaded ");
            }
            else
            {

                MessageBox.Show(" Their is a Problem ");

            }

        }

        // User -> Logout Button Opration
        private void U_logout_btn_Click(object sender, EventArgs e)
        {
            // Innitial Deletion Opration
            bool a = SQL.Problem_Data_Delete();
            bool b = SQL.Delete_User_View();
            if (a && b)
            {
                //Univeasral Reg_no is null if log out
                U_Reg_no = null;

                MessageBox.Show(" Logout Succesfully ");
            }

            
            // Panal Visibility after Logout
            User.Visible = false;
            Login.Visible = true;
            Logo.Visible = true;

        }

        /// <summary>
        /// -------------------------- User End ---------------------------------------------
        /// </summary>

        /// <summary>
        /// -------------------------- Admin Start ---------------------------------------------
        /// </summary>

        // User Home Pie Chart Create Function
        private void Admin_Home_Pie_Chart()
        {
            // ----------------- Block 33 Pie chart -----------------------

            DataTable dt33 = SQL.Admin_Block_Pie_Chart("33");

            string[] Problem33 = new string[dt33.Rows.Count];
            int[] Problem_count33 = new int[dt33.Rows.Count];

            for (int i = 0; i < dt33.Rows.Count; i++)
            {
                Problem33[i] = dt33.Rows[i][1].ToString();
                Problem_count33[i] = Convert.ToInt32(dt33.Rows[i][0]);
            }

            A_Home_Block_33_Chart.Series["Series1"].Points.DataBindXY(Problem33, Problem_count33);

            // -----------------Block 34 Pie chart -----------------------

           DataTable dt34 = SQL.Admin_Block_Pie_Chart("34");

            string[] Problem34 = new string[dt34.Rows.Count];
            int[] Problem_count34 = new int[dt34.Rows.Count];

            for (int i = 0; i < dt34.Rows.Count; i++)
            {
                Problem34[i] = dt34.Rows[i][1].ToString();
                Problem_count34[i] = Convert.ToInt32(dt34.Rows[i][0]);
            }

            A_Home_Block_34_Chart.Series["Series1"].Points.DataBindXY(Problem34, Problem_count34);

            // -----------------Block 37 Pie chart -----------------------

            DataTable dt37 = SQL.Admin_Block_Pie_Chart("37");

            string[] Problem37 = new string[dt37.Rows.Count];
            int[] Problem_count37 = new int[dt37.Rows.Count];

            for (int i = 0; i < dt37.Rows.Count; i++)
            {
                Problem37[i] = dt37.Rows[i][1].ToString();
                Problem_count37[i] = Convert.ToInt32(dt37.Rows[i][0]);
            }

            A_Home_Block_37_Chart.Series["Series1"].Points.DataBindXY(Problem37, Problem_count37);

            // -----------------Block 38 Pie chart -----------------------

            DataTable dt38 = SQL.Admin_Block_Pie_Chart("38");

            string[] Problem38 = new string[dt38.Rows.Count];
            int[] Problem_count38 = new int[dt38.Rows.Count];

            for (int i = 0; i < dt38.Rows.Count; i++)
            {
                Problem38[i] = dt38.Rows[i][1].ToString();
                Problem_count38[i] = Convert.ToInt32(dt38.Rows[i][0]);
            }

            A_Home_Block_38_Chart.Series["Series1"].Points.DataBindXY(Problem38, Problem_count38);

        }

        // Admin -> Home Button Opration
        private void A_Home_Btn_Click(object sender, EventArgs e)
        {
            Admin_Home_Pie_Chart();
            // Panal Visibility Opration
            A_Home.Visible = true;
            A_Profile.Visible = false;
            A_View_Problem_Data.Visible = false;
            A_Add_Update.Visible = false;
            A_Entity_Info.Visible = false;

        }

        // Admin -> Profile Button Opration
        private async void A_Profile_Btn_Click(object sender, EventArgs e)
        {
            // Data Extract  from fiebase import
            FirebaseResponse respo = await client.GetTaskAsync("User_Data/" + U_Reg_no);
            User_Data data = respo.ResultAs<User_Data>();

            // data assing in text
            A_Profile_Reg_No.Text = data.Reg_no;
            A_Profile_Name.Text = data.Full_name;
            A_Profile_Contact_No.Text = data.Contact;
            A_Profile_Email.Text = data.Email_id;
            A_Profile_Gender.Text = data.Gender;


            // Panal Visibility Opration
            A_Home.Visible = false;
            A_Profile.Visible = true;
            A_View_Problem_Data.Visible = false;
            A_Add_Update.Visible = false;
            A_Entity_Info.Visible = false;
        }

        // Admin -> Profile -> Submit Button Opration
        private async void A_Profile_Change_Password_Submit_Btn_Click(object sender, EventArgs e)
        {
            if (A_Profile_Change_Password.Text == "" || A_Profile_Change_Re_Password.Text == "")
            {
                MessageBox.Show(" **Please Fill these field ");

            }
            else if (A_Profile_Change_Password.Text != A_Profile_Change_Re_Password.Text)
            {
                MessageBox.Show(" **Passwords dose not match ");
                A_Profile_Change_Re_Password.Clear();
            }
            else if (A_Profile_Change_Password.Text == A_Profile_Change_Re_Password.Text)
            {
                // Update new password
                var s_data = new User_Data
                {
                    Reg_no = A_Profile_Reg_No.Text,
                    Full_name = A_Profile_Name.Text,
                    Contact = A_Profile_Contact_No.Text,
                    Email_id = A_Profile_Email.Text,
                    Gender = A_Profile_Gender.Text,
                    User_Password = A_Profile_Change_Password.Text,
                    Designation = "Admin",
                    Problem_Domain = ""
                };

                SetResponse S_response = await client.SetTaskAsync("User_Data/" + U_Reg_no, s_data);
                User_Data result = S_response.ResultAs<User_Data>();

                // Inputs clear
                A_Profile_Change_Password.Clear();
                A_Profile_Change_Re_Password.Clear();

                MessageBox.Show(" Password Changed ");
            }
        }

        // Admin -> Profile -> Reset Button Opration
        private void A_Profile_Change_Password_Reset_Btn_Click(object sender, EventArgs e)
        {
            // Inputs clear
            A_Profile_Change_Password.Clear();
            A_Profile_Change_Re_Password.Clear();
        }

        // Admin -> View Problem Data Button Opration
        private void A_View_Problem_Data_Btn_Click(object sender, EventArgs e)
        {
            DataTable dt = SQL.Problem_Data_Select();
            A_Problem_View_DataGrid.DataSource = dt;

            // Panal Visibility Opration
            A_Home.Visible = false;
            A_Profile.Visible = false;
            A_View_Problem_Data.Visible = true;
            A_Add_Update.Visible = false;
            A_Entity_Info.Visible = false;
        }

        // Admin -> View Problem Data -> Search Opration
        private void A_Problem_View_Search_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = SQL.Problem_Data_Search(this.A_Problem_View_Search.Text);
            A_Problem_View_DataGrid.DataSource = dt;
        }

        // Admin -> Add/Update Button Opration
        private void A_Add_Update_Btn_Click(object sender, EventArgs e)
        {
            // Panal Visibility Opration
            A_Home.Visible = false;
            A_Profile.Visible = false;
            A_View_Problem_Data.Visible = false;
            A_Add_Update.Visible = true;
            A_Entity_Info.Visible = false;
        }

        // Admin -> Add/Update Entity -> Submit Button Opration
        private async void A_Add_Update_Submit_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                // For Null Field 
                if (A_Add_Update_Type.Text == "")
                {
                    MessageBox.Show(" **Please Fill Type ");
                }

                // ---------------------------------------- For New Entity ---------------------------------------
                else if (A_Add_Update_Type.Text == "New")
                {
                    
                    // Check All Fild are Fill or  not
                    if ( A_Add_Update_Designation.Text == "" || A_Add_Update_Reg_No.Text == "" || A_Add_Update_Name.Text == "" || A_Add_Update_Contact_No.Text == "" || A_Add_Update_Email.Text == "" || A_Add_Update_Gender.Text == "" || A_Add_Update_Password.Text == "" || A_Add_Update_Re_Password.Text == "")
                    {
                        MessageBox.Show(" **Please Fill All Area ");
                    }
                    else
                    {
                        bool flag = true;
                        // Get User Count to firebase import
                        FirebaseResponse c_respo = await client.GetTaskAsync("Counting/Node");
                        User_Data c_data = c_respo.ResultAs<User_Data>();

                        //conver string to inteager
                        int count = Int32.Parse(c_data.User_count);
                        string p = c_data.Problem_count;


                        // check Reg no duplicatin firebase
                        for (int i = 1; i <= count; i++)
                        {
                            // Get User Uniqe id to firebase import
                            FirebaseResponse u_respo = await client.GetTaskAsync("Uniqe_User_Id/" + i);
                            User_Data u_data = u_respo.ResultAs<User_Data>();

                            //check dupicat Reg no
                            if (u_data.Uniqe_Reg_no == A_Add_Update_Reg_No.Text)
                            {
                                MessageBox.Show(" This Reg no is Already exist Plaese Enter Valid Reg no ", " Warning ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                A_Add_Update_Reg_No.Clear();
                                flag = false;
                            }

                        }

                        // check Reg no.
                        if (flag)
                        {
                            //check password maching
                            if (A_Add_Update_Re_Password.Text != A_Add_Update_Password.Text)
                            {
                                errorProvider1.SetError(this.A_Add_Update_Re_Password, " ** Password is not match ");
                                A_Add_Update_Re_Password.Clear();
                            }

                            else if (A_Add_Update_Re_Password.Text == A_Add_Update_Password.Text)
                            {
                                errorProvider1.Clear();

                                var u_data = new User_Data
                                {
                                    Reg_no = A_Add_Update_Reg_No.Text,
                                    Full_name = A_Add_Update_Name.Text,
                                    Contact = A_Add_Update_Contact_No.Text,
                                    Email_id = A_Add_Update_Email.Text,
                                    Gender = A_Add_Update_Gender.Text,
                                    User_Password = A_Add_Update_Password.Text,
                                    Designation = A_Add_Update_Designation.Text,
                                    Problem_Domain = A_Add_Update_Problem_Domain.Text
                                };

                                SetResponse a_response = await client.SetTaskAsync("User_Data/" + A_Add_Update_Reg_No.Text, u_data);
                                User_Data result = a_response.ResultAs<User_Data>();
                                
                                // Count Inceased
                                count++;
                                string count1 = count.ToString();

                                var c1_data = new User_Data
                                {
                                    User_count = count1,
                                    Problem_count = p

                                };
                                // Set User Count to firebase Export
                                FirebaseResponse c1_respo = await client.SetTaskAsync("Counting/Node", c1_data);
                                User_Data c1_result = c1_respo.ResultAs<User_Data>();

                                // Unire Id updated
                                var u1_data = new User_Data
                                {
                                    Uniqe_Reg_no = A_Add_Update_Reg_No.Text,
                                    User_Id = count1
                                };
                                // Set User Count to firebase Export
                                FirebaseResponse u1_respo = await client.SetTaskAsync("Uniqe_User_Id/" + count1, u1_data);
                                User_Data u1_result = u1_respo.ResultAs<User_Data>();

                                // clear all  fileds
                                A_Add_Update_Reset_inputs();

                                MessageBox.Show(" Entity Add Succesfully ");
                            }
                        }

                    }


                }

                // ----------------------------------------------------- For Update Entity ----------------------------------
                else if (A_Add_Update_Type.Text == "Update")
                {


                    // Check All Fild are Fill or  not
                    if (A_Add_Update_Designation.Text == "" || A_Add_Update_Reg_No.Text == "" || A_Add_Update_Name.Text == "" || A_Add_Update_Contact_No.Text == "" || A_Add_Update_Email.Text == "" || A_Add_Update_Gender.Text == "" || A_Add_Update_Password.Text == "" || A_Add_Update_Re_Password.Text == "")
                    {
                        MessageBox.Show(" **Please Fill All Area ");
                    }
                    else
                    {
                        bool flag = false;
                        // Get User Count to firebase import
                        FirebaseResponse c_respo = await client.GetTaskAsync("Counting/Node");
                        User_Data c_data = c_respo.ResultAs<User_Data>();

                        //conver string to inteager
                        int count = Int32.Parse(c_data.User_count);
                        string p = c_data.Problem_count;


                        // check Reg no duplicatin firebase
                        for (int i = 1; i <= count; i++)
                        {
                            // Get User Uniqe id to firebase import
                            FirebaseResponse u_respo = await client.GetTaskAsync("Uniqe_User_Id/" + i);
                            User_Data u_data = u_respo.ResultAs<User_Data>();

                            //check dupicat Reg no
                            if (u_data.Uniqe_Reg_no == A_Add_Update_Reg_No.Text)
                            {
                               
                                flag = true;
                            }

                        }
                        //chaking reg no valid or not
                        if(flag)
                        {
                            //check password maching
                            if (A_Add_Update_Re_Password.Text != A_Add_Update_Password.Text)
                            {
                                errorProvider1.SetError(this.A_Add_Update_Re_Password, " ** Password is not match ");
                                A_Add_Update_Re_Password.Clear();
                            }

                            else if (A_Add_Update_Re_Password.Text == A_Add_Update_Password.Text)
                            {
                                errorProvider1.Clear();

                                var s_data = new User_Data
                                {
                                    Reg_no = A_Add_Update_Reg_No.Text,
                                    Full_name = A_Add_Update_Name.Text,
                                    Contact = A_Add_Update_Contact_No.Text,
                                    Email_id = A_Add_Update_Email.Text,
                                    Gender = A_Add_Update_Gender.Text,
                                    User_Password = A_Add_Update_Password.Text,
                                    Designation = A_Add_Update_Designation.Text,
                                    Problem_Domain = A_Add_Update_Problem_Domain.Text
                                };

                                SetResponse S_response = await client.SetTaskAsync("User_Data/" + A_Add_Update_Reg_No.Text, s_data);
                                User_Data result = S_response.ResultAs<User_Data>();

                                // clear all  fileds
                                A_Add_Update_Reset_inputs();

                                MessageBox.Show(" Entity Update Succesfully ");
                            }
                        }
                        else
                        {
                            MessageBox.Show(" This Reg_no is NOT Valid ** Plese fill Right Reg_no", " Warning ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            A_Add_Update_Reg_No.Clear();
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
        }

        // Admin -> Add/Update Entity -> Reset Button Opration
        private void A_Add_Update_Reset_Btn_Click(object sender, EventArgs e)
        {
            A_Add_Update_Reset_inputs();
        }

        // For only resert purpose
        private void A_Add_Update_Reset_inputs()
        {
            // clear all  fileds
            A_Add_Update_Type.ResetText();
            A_Add_Update_Designation.ResetText();
            A_Add_Update_Problem_Domain.ResetText();
            A_Add_Update_Reg_No.Clear();
            A_Add_Update_Name.Clear();
            A_Add_Update_Contact_No.Clear();
            A_Add_Update_Email.Clear();
            A_Add_Update_Gender.ResetText();
            A_Add_Update_Password.Clear();
            A_Add_Update_Re_Password.Clear();

        }

        // Admin -> Entity Info Button Opration
        private void A_Entity_Info_Btn_Click(object sender, EventArgs e)
        {
            // Panal Visibility Opration
            A_Home.Visible = false;
            A_Profile.Visible = false;
            A_View_Problem_Data.Visible = false;
            A_Add_Update.Visible = false;
            A_Entity_Info.Visible = true;
            
            A_Entity_Info_Admin_Info.Visible = false;
            A_Entity_Info_Referal_Info.Visible = false;
            A_Entity_Info_User_Info.Visible = false;
        }

        // Admin -> Entity Info -> Admi Button Opration
        private void A_Entity_Info_Admin_Btn_Click(object sender, EventArgs e)
        {
            DataTable dt = SQL.User_Data_Info("Admin");
            A_Entity_Info_Admin_Info_DataGrid.DataSource = dt;

            // Panal Visibility
            A_Entity_Info_Admin_Info.Visible = true;
            A_Entity_Info_Referal_Info.Visible = false;
            A_Entity_Info_User_Info.Visible = false;
        }

        // Admin -> Entity Info -> Referal Button Opration
        private void A_Entity_Info_Referal_Btn_Click(object sender, EventArgs e)
        {
            DataTable dt = SQL.User_Data_Info("Referal");
            A_Entity_Info_Referal_Info_DataGrid.DataSource = dt;

            // Panal Visibility
            A_Entity_Info_Admin_Info.Visible = false;
            A_Entity_Info_Referal_Info.Visible = true;
            A_Entity_Info_User_Info.Visible = false;
        }

        // Admin -> Entity Info -> User Button Opration
        private void A_Entity_Info_User_Btn_Click(object sender, EventArgs e)
        {
            DataTable dt = SQL.User_Data_Info("User");
            A_Entity_Info_User_Info_DataGrid.DataSource = dt;

            // Panal Visibility
            A_Entity_Info_Admin_Info.Visible = false;
            A_Entity_Info_Referal_Info.Visible = false;
            A_Entity_Info_User_Info.Visible = true;
        }

        // Admin -> Reload Button Opration
        private void A_Reload_Btn_Click(object sender, EventArgs e)
        {
            // Innitial Deletion Opration
            bool p = SQL.Problem_Data_Delete();
            bool u = SQL.User_Data_Delete();
            if(p && u)
            {
                //Reload Problem data to the firebase
                Problem_Data_Import();
                User_Data_Import();
                MessageBox.Show(" Reloaded ");
            }
            else
            {
                MessageBox.Show(" Error ocured Recorders are not deleted ");

            }
        }

        // Admin -> Logout Button Opration
        private void A_Logout_Btn_Click(object sender, EventArgs e)
        {
            bool p = SQL.Problem_Data_Delete();
            bool u = SQL.User_Data_Delete();

            if (p && u)
            {
                U_Reg_no = null;
                
                MessageBox.Show(" Loguot Successfully ");
            }

            // Panal Visibility Opration
            Admin.Visible = false;
            Login.Visible = true;
            Logo.Visible = true;

        }


        
        /// <summary>
        /// -------------------------- Admin End ---------------------------------------------
        /// </summary>


        /// <summary>
        /// -------------------------- Referal Start ---------------------------------------------
        /// </summary>

        // User Home Pie Chart Create Function
        private void Referal_Home_Pie_Chart()
        {
            DataTable dt = SQL.Referal_View_Pie_Chart();

            string[] Block = new string[dt.Rows.Count];
            int[] Block_count = new int[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Block[i] = dt.Rows[i][1].ToString();
                Block_count[i] = Convert.ToInt32(dt.Rows[i][0]);
            }

            R_Block_Problem_Chart.Series["Series1"].Points.DataBindXY(Block, Block_count);
        }

        // Referal Home Panal Pie chart
        private void R_Home_Paint(object sender, PaintEventArgs e)
        {
            Referal_Home_Pie_Chart();
        }

        // Referal -> Home Button Opration 
        private void R_Home_Btn_Click(object sender, EventArgs e)
        {
            R_Home_Problem_Domain.Text = U_Problem_Domain;
            // Referal Home Pie chart Created 
            Referal_Home_Pie_Chart();

            // Panal Visibility Opration
            R_Home.Visible = true;
            R_Profile.Visible = false;
            R_View_Problem_Data.Visible = false;
            R_Update_Status.Visible = false;
        }

        // Referal -> Profile Button Opration
        private async void R_Profile_Btn_Click(object sender, EventArgs e)
        {
            // Data Extract  from fiebase import
            FirebaseResponse respo = await client.GetTaskAsync("User_Data/" + U_Reg_no);
            User_Data data = respo.ResultAs<User_Data>();

            // Data Assing in text
            R_Profile_Problem_Domain.Text = data.Problem_Domain;
            R_Profile_Reg_No.Text = data.Reg_no;
            R_Profile_Name.Text = data.Full_name;
            R_Profile_Contact_No.Text = data.Contact;
            R_Profile_Email.Text = data.Email_id;
            R_Profile_Gender.Text = data.Gender;

            // Panals visibility
            R_Home.Visible = false;
            R_Profile.Visible = true;
            R_View_Problem_Data.Visible = false;
            R_Update_Status.Visible = false;

        }

        // Referal -> Profile -> Change Password -> Submit Button Opration
        private async void R_Profile_Change_Password_Submit_Btn_Click(object sender, EventArgs e)
        {
            if (R_Profile_Change_Password.Text == "" || R_Profile_Change_Re_Password.Text == "")
            {
                MessageBox.Show(" **Please Fill these field ");

            }
            else if (R_Profile_Change_Password.Text != R_Profile_Change_Re_Password.Text)
            {
                MessageBox.Show(" **Passwords dose not match ");
                R_Profile_Change_Re_Password.Clear();
            }
            else if (R_Profile_Change_Password.Text == R_Profile_Change_Re_Password.Text)
            {
                // Update new password
                var s_data = new User_Data
                {
                    Reg_no = R_Profile_Reg_No.Text,
                    Full_name = R_Profile_Name.Text,
                    Contact = R_Profile_Contact_No.Text,
                    Email_id = R_Profile_Email.Text,
                    Gender = R_Profile_Gender.Text,
                    User_Password = R_Profile_Change_Password.Text,
                    Designation = "Referal",
                    Problem_Domain = R_Profile_Problem_Domain.Text
                };

                SetResponse S_response = await client.SetTaskAsync("User_Data/" + U_Reg_no, s_data);
                User_Data result = S_response.ResultAs<User_Data>();

                // Inputs clear
                R_Profile_Change_Password.Clear();
                R_Profile_Change_Re_Password.Clear();
            }
        }

        // Referal -> Profile -> Change Password -> Reset Button Opration
        private void R_Profile_Change_Password_Reset_Btn_Click(object sender, EventArgs e)
        {
            // Inputs clear
            R_Profile_Change_Password.Clear();
            R_Profile_Change_Re_Password.Clear();
        }

        // Referal -> View Problem Data Button Opration 
        private void R_View_Problem_Data_Btn_Click(object sender, EventArgs e)
        {
            // DataGrid View Opration
            if (SQL.Delete_User_View())
            {
                // Create User View for user perpose
                if (SQL.R_Create_User_View(U_Problem_Domain))
                {
                    DataTable dt = SQL.User_View_Select();
                    R_View_Problem_Data_DataGrid.DataSource = dt;
                }

            }

            // Panal Visibility Opration
            R_Home.Visible = false;
            R_Profile.Visible = false;
            R_View_Problem_Data.Visible = true;
            R_Update_Status.Visible = false;
        }

        // Referal -> View Problem Data -> Search Opration
        private void R_View_Problem_Data_Search_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = SQL.User_View_Search(R_View_Problem_Data_Search.Text);
            R_View_Problem_Data_DataGrid.DataSource = dt;
        }

        // Referal -> Update Staus Button Opration 
        private void R_Update_Status_Btn_Click(object sender, EventArgs e)
        {
            R_Update_Status_Problem_Domain.Text = U_Problem_Domain;

            // Panal Visibility Opration
            R_Home.Visible = false;
            R_Profile.Visible = false;
            R_View_Problem_Data.Visible = false;
            R_Update_Status.Visible = true;
        }

        // Referal -> Update Staus -> Submit Button Opration
        private async void R_Update_Status_Submit_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if(R_Update_Satus_Problem_Id.Text == "" || R_Update_Status_Discription.Text == "")
                {
                    MessageBox.Show(" ** Please Fill all the filds");
                }
                else
                {
                    // Chacking the Problem Id

                    DataTable dt = SQL.User_View_Select();
                    bool flag = false;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (R_Update_Satus_Problem_Id.Text == dt.Rows[i][0].ToString())
                        {
                            flag = true;
                        }

                    }

                    // After Confirmition Problem Id  Update on firebase
                    if(flag)
                    {
                        // Get User Probelm Data to firebase import
                        FirebaseResponse up_respo = await client.GetTaskAsync("User_Problem_Data/" + R_Update_Satus_Problem_Id.Text);
                        User_Data up_data = up_respo.ResultAs<User_Data>();

                        // Data assign up_data to User_data
                        var sql_data = new User_Data
                        {
                            User_Reg_no = up_data.User_Reg_no,
                            Referal_Reg_no = U_Reg_no,
                            problem_id = up_data.problem_id,
                            Block = up_data.Block,
                            Room_no = up_data.Room_no,
                            Problem = up_data.Problem,
                            Discription = up_data.Discription,
                            Status = R_Update_Status_Discription.Text

                        };
                        // Get User Probelm Data to firebase export
                        SetResponse S_response = await client.SetTaskAsync("User_Problem_Data/" + R_Update_Satus_Problem_Id.Text, sql_data);
                        User_Data result = S_response.ResultAs<User_Data>();

                    }
                    else
                    {
                        MessageBox.Show(" ** Incoorect Problem Id ");
                        R_Update_Satus_Problem_Id.Clear();
                    }

                }

                MessageBox.Show(" Status Updated Succesfully ");
                // Input Clear
                R_Update_Satus_Problem_Id.Clear();
                R_Update_Status_Discription.Clear();

            }
            catch(Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }

        }

        // referal -> Update Status -> Reset Button Opration
        private void R_Update_Status_Reset_Btn_Click(object sender, EventArgs e)
        {
            // Input clear
            R_Update_Satus_Problem_Id.Clear();
            R_Update_Status_Discription.Clear();
        }

        // Referal -> Reload Button Opration 
        private void R_Reload_Btn_Click(object sender, EventArgs e)
        {
            // Innitial Deletion Opration

            bool a = SQL.Problem_Data_Delete();
            bool b = SQL.Delete_User_View();
            if (a && b)
            {
                //Reload Problem data to the firebase
                Problem_Data_Import();
                if(SQL.R_Create_User_View(U_Problem_Domain))
                {
                    MessageBox.Show(" Reload Successfully ");
                }
            }
            else
            {
                MessageBox.Show(" Error Occur Recorders are not deleted ");
                
            }
        }

        // Referal -> Logout Button Opration 
        private void R_Logout_Btn_Click(object sender, EventArgs e)
        {
            // Innitial Deletion Opration
            bool a = SQL.Problem_Data_Delete();
            bool b = SQL.Delete_User_View();
            if (a && b)
            {
                //Univeasral Reg_no is null if log out
                U_Reg_no = null;
                U_Problem_Domain = null;

                MessageBox.Show(" Logout Succesfully ");
            }


            // Panal Visibility after Logout
            Referal.Visible = false;
            Login.Visible = true;
            Logo.Visible = true;
        }


        /// <summary>
        ///  -------------------------------- Validation For Inputs ---------------------------
        /// </summary>

        // SignUp Form -> Reg_no validation
        private void S_Reg_no_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char ch = e.KeyChar;

            if(!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                errorProvider1.SetError(this.S_Reg_no, " ** Valid Only Digits 0-9 ");
                e.Handled = true;
            }
            else
            {
                errorProvider1.Clear();
                return;
            }

        }

        // SignUp Form -> Email Validation
        private void S_Email_Leave(object sender, EventArgs e)
        {
            string pattarn = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

            if(Regex.IsMatch(S_Email.Text,pattarn))
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(this.S_Email, " **Please Enter Valid Email Id ");
                return;
            }
        }

        // SignUp Form -> Contact no validation
        private void S_Contact_no_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                errorProvider1.SetError(this.S_Contact_no, " ** Valid Only Digits 0-9 ");
                e.Handled = true;
            }
            else
            {
                errorProvider1.Clear();
                return;
            }

        }

        //Login Form -> Regno Validation
        private void Regno_input_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                errorProvider1.SetError(this.Regno_input, " ** Valid Only Digits 0-9 ");
                e.Handled = true;
            }
            else
            {
                errorProvider1.Clear();
                return;
            }
        }
       
        // User Problem -> Room No Input valiidation Only Digits
        private void U_P_Room_no_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                errorProvider1.SetError(this.U_P_Room_no, " ** Valid Only Digits 0-9 ");
                e.Handled = true;
            }
            else
            {
                errorProvider1.Clear();
                return;
            }
        }

        // Referal -> Update Status -> Problem Id Text Input Validation
        private void R_Update_Satus_Problem_Id_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                errorProvider1.SetError(this.R_Update_Satus_Problem_Id, " ** Valid Only Digits 0-9 ");
                e.Handled = true;
            }
            else
            {
                errorProvider1.Clear();
                return;
            }
        }

        
        
        // Admin -> Add/Update Entity -> Reg_no Input Validation
        private void A_Add_Update_Reg_No_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                errorProvider1.SetError(this.A_Add_Update_Reg_No, " ** Valid Only Digits 0-9 ");
                e.Handled = true;
            }
            else
            {
                errorProvider1.Clear();
                return;
            }
        }

        // Admin -> Add/Update Entity -> Contact No. Input Validation
        private void A_Add_Update_Contact_No_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                errorProvider1.SetError(this.A_Add_Update_Contact_No, " ** Valid Only Digits 0-9 ");
                e.Handled = true;
            }
            else
            {
                errorProvider1.Clear();
                return;
            }
        }

        // Admin -> Add/Update Entity -> Contact No. Input Validation
        private void A_Add_Update_Email_Leave(object sender, EventArgs e)
        {
            string pattarn = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

            if (Regex.IsMatch(A_Add_Update_Email.Text, pattarn))
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(this.A_Add_Update_Email, " **Please Enter Valid Email Id ");
                return;
            }
        }

        // Admin -> Add/Update Entity -> Designation Input Validation
        private void A_Add_Update_Designation_SelectedValueChanged(object sender, EventArgs e)
        {
            if(A_Add_Update_Designation.Text == "Referal")
            {
                A_Add_Update_Problem_Domain.Visible = true;
            }
            else
            {
                A_Add_Update_Problem_Domain.Visible = false;
            }
        }

        
    }
}
