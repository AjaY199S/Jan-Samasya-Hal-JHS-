using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace JSH
{
    class SQL_Connections
    {
        


        // Connection to database
        static string myconnstring = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
        
        // Insert Problem data into a database 
        public bool Problem_Data_Insert (User_Data D)
        {
            // Create a Default Return
            bool flag = false;

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);

            try
            {
                // Step 2 : Create a SQL Query to Insert data
                string sql = "insert into User_Problem_Data_Table (problem_id, User_Reg_no, Referal_Reg_no, Block, Room_no, Problem, Discription, Status) VALUES (@problem_id, @User_Reg_no, @Referal_Reg_no, @Block, @Room_no, @Problem, @Discription, @Status)";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Create Parameters to add data
                cmd.Parameters.AddWithValue("@problem_id", Int32.Parse(D.problem_id));
                cmd.Parameters.AddWithValue("@User_Reg_no", D.User_Reg_no);
                cmd.Parameters.AddWithValue("@Referal_Reg_no", D.Referal_Reg_no);
                cmd.Parameters.AddWithValue("@Block", D.Block);
                cmd.Parameters.AddWithValue("@Room_no", D.Room_no);
                cmd.Parameters.AddWithValue("@Problem", D.Problem);
                cmd.Parameters.AddWithValue("@Discription", D.Discription);
                cmd.Parameters.AddWithValue("@Status", D.Status);

                // Connection Open Here
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                // If query runs successfully than the value of rows is grater than zero
                if(rows>0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }

        // Insert Problem data into a database 
        public bool User_Data_Insert(User_Data D)
        {
            // Create a Default Return
            bool flag = false;

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);

            try
            {
                // Step 2 : Create a SQL Query to Insert data
                string sql = "insert into User_Data_Table (Reg_no,Name,Email_Id,Contact_no,Gender,Designation,Problem_Domain) VALUES(@Reg_no,@Name,@Email_Id,@Contact_no,@Gender,@Designation,@Problem_Domain)";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Create Parameters to add data
                cmd.Parameters.AddWithValue("@Reg_no", D.Reg_no);
                cmd.Parameters.AddWithValue("@Name", D.Full_name);
                cmd.Parameters.AddWithValue("@Email_Id", D.Email_id);
                cmd.Parameters.AddWithValue("@Contact_no", D.Contact);
                cmd.Parameters.AddWithValue("@Gender", D.Gender);
                cmd.Parameters.AddWithValue("@Designation", D.Designation);
                cmd.Parameters.AddWithValue("@Problem_Domain", D.Problem_Domain);
               

                // Connection Open Here
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                // If query runs successfully than the value of rows is grater than zero
                if (rows > 0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }

        // Delete Problem data into a database 
        public bool Problem_Data_Delete()
        {
            // Create a Default Return
            bool flag = true;

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);

            try
            {
                // Step 2 : Create a SQL Query to Delete data
                string sql = "Delete From User_Problem_Data_Table";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Connection Open Here
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                // If query runs successfully than the value of rows is grater than zero
                if (rows > 0)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }

        // Delete User Data into a database 
        public bool User_Data_Delete()
        {
            // Create a Default Return
            bool flag = true;

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);

            try
            {
                // Step 2 : Create a SQL Query to Delete data
                string sql = "Delete From User_Data_Table";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Connection Open Here
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                // If query runs successfully than the value of rows is grater than zero
                if (rows > 0)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }

        // User Status Grid View table hole table show on status
        public DataTable Problem_Data_Select()
        {
            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "SELECT * FROM User_Problem_Data_Table";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        // User Status Grid View table hole table show on status
        public DataTable User_Data_Select()
        {
            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "SELECT * FROM User_Data_Table";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        /// <summary>
        /// ------------------- User Perspective Creatied A View and All Opration ------------------
        /// </summary>

        // User Status Grid View table only for Create New View For User use
        public bool Create_User_View(string U_Reg_no)
        {
            // Create a Default Return
            bool flag = true;

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);

            try
            {
                // Step 2: Write Sql Query
                string sql = "CREATE VIEW User_View AS SELECT * FROM User_Problem_Data_Table WHERE User_Reg_no LIKE '%" + U_Reg_no + "%' ";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Connection Open Here
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                // If query runs successfully than the value of rows is grater than zero
                if (rows > 0)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }

        // User Status Grid View table only for Delete User View opration
        public bool Delete_User_View()
        {
            // Create a Default Return
            bool flag = true;

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);

            try
            {
                // Step 2 : Create a SQL Query to Delete data
                string sql = "DROP VIEW User_View";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Connection Open Here
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                // If query runs successfully than the value of rows is grater than zero
                if (rows > 0)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = true;
                return flag;
                //MessageBox.Show(" Error : " + ex);
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }

        // User Status Grid View table hole table show on status
        public DataTable User_View_Select()
        {
            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "SELECT * FROM User_View";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        // User Status Grid View table only for Search opration
        public DataTable User_View_Search(string search)
        {
            
            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "SELECT * FROM User_View where problem_id like '%" + search + "%' OR Referal_Reg_no like '%" + search + "%' OR Block like '%" + search + "%' OR Room_no like '%" + search + "%' OR Problem like '%" + search + "%' ";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        // User Status Grid View table only for Search opration
        public DataTable User_View_Pie_Chart()
        {
            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "Select count(problem_id) As 'Problem_Count',Problem from User_View WHERE Status = 'No Status' Group By Problem";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        /// <summary>
        /// ------------------- Referal Perspective Creatied A View and All Opration ------------------
        /// </summary>

        // User Status Grid View table only for Create New View For User use
        public bool R_Create_User_View(string U_Problem_Domain)
        {
            // Create a Default Return
            bool flag = true;

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);

            try
            {
                // Step 2: Write Sql Query
                string sql = "CREATE VIEW User_View AS SELECT * FROM User_Problem_Data_Table WHERE Problem LIKE '%" + U_Problem_Domain + "%' ";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Connection Open Here
                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                // If query runs successfully than the value of rows is grater than zero
                if (rows > 0)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Error : " + ex);
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }

        // User Status Grid View table only for Search opration
        public DataTable Referal_View_Pie_Chart()
        {
            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "Select count(problem_id) As 'Block_Count',Block from User_View WHERE Status = 'No Status' Group By Block";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        /// <summary>
        /// ------------------- Admin Perspective Creatied A View and All Opration ------------------
        /// </summary>

        // User Status Grid View table only for Search opration
        public DataTable Problem_Data_Search(string search)
        {

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "SELECT * FROM User_Problem_Data_Table WHERE problem_id like '%" + search + "%' OR Referal_Reg_no like '%" + search + "%' OR Block like '%" + search + "%' OR Room_no like '%" + search + "%' OR Problem like '%" + search + "%' ";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        // User Status Grid View table only for Search opration
        public DataTable User_Data_Info(string Designation)
        {

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "SELECT * FROM User_Data_Table WHERE Designation Like '%" + Designation + "%'";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }

        // User Status Grid View table only for Search opration
        public DataTable Admin_Block_Pie_Chart(string Block)
        {

            //  Step 1 : Connecton to database
            SqlConnection conn = new SqlConnection(myconnstring);
            DataTable dt = new DataTable();

            try
            {
                // Step 2: Write Sql Query
                string sql = "Select count(problem_id) As 'Problem_Count',Problem from User_Problem_Data_Table Where Block Like '%" + Block + "%' And Status = 'No Status' Group By Problem";

                // Step 3 : Create Sql Command using sql and conn
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Step 4 : Creating SQL DataAdapter using cmd
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("  Error : " + ex);
            }
            finally
            {

            }
            return dt;
        }
    }

}
