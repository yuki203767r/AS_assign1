using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Text;

namespace AS_assign1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string MYDBConnectionString =
System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] credit = null;
        string userID = null;
        byte[] Key;
        byte[] IV;
        static string email;
        static string f;
        static int timeout;
        string finalHash;
        static string pwdtime;
        static int maxpwdtime = 5;
        static int minpwdtime = 3;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    if (Request.QueryString["Status"] == "yes"){
                        Label1.Text = "Please change your password as it has exceeds the password policy.";
                    }
                    email = Session["LoggedIn"].ToString();
                }

            }

            else
            {
                Response.Redirect("Login.aspx", false);
            }

        }



        protected void Button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(changepwd.Text.ToString()))
            {
                change_error.Text = "no input, failed to update password!";
                return;
            }
            else if (changepwd.Text.ToString() != changepwd.Text.ToString()){
                change_error.Text = "Both password does not match!";
            }
            else
            {
                var boolean = password_checking(HttpUtility.HtmlEncode(changepwd.Text.ToString()));
                if (boolean == "true")
                {
                    var salt = getDBSalt(email);
                    var passwordNsalt = getPWDNSALT(email);
                    var pwd1 = getPWD1(email);
                    var pwd2 = getPWD2(email);
                    string newpwdNSalt = changepwd.Text + salt;

                    var pwdtimecreated = Convert.ToDateTime(pwdtime);
                    DateTime timenow = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    TimeSpan ts = timenow.Subtract(pwdtimecreated);
                    int minutesleft = Convert.ToInt32(ts.TotalMinutes);
                    int pendingminutes = minpwdtime - minutesleft;

                    if (pendingminutes <= 0)
                    {

                        SHA512Managed hashing = new SHA512Managed();
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdNSalt));
                        finalHash = Convert.ToBase64String(hashWithSalt);
                        if (finalHash == passwordNsalt || finalHash == pwd1 || finalHash == pwd2)
                        {
                            change_error.Text = "Please do not reuse password";
                            return;
                        }
                        else
                        {
                            updatePWD(email, pwd2, passwordNsalt, finalHash);
                            Response.Redirect("Home.aspx", false);
                        }
                    }
                    else
                    {
                        change_error.Text = "You need to wait for 3 minutes to reset your password" ;
                        return;

                    }
                }
                else
                {
                    change_error.Text = boolean;
                    return;
                }


            }
        }


        private string password_checking(string password)
        {
            int score = 0; string status = "";

            if (password.Length < 12)
            {
                f += " Password not enough length!\n ";
                //return 1;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            else
            {
                f += " Password need to have at least 1 lowercase! ";
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            else
            {
                f += " Password need to have at least 1 uppercase!\n ";
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            else
            {
                f += " Password need to have at least 1 digit!\n ";
            }

            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }
            else
            {
                f += " Password need to have at least 1 special symbol! \n";
            }
            string feedback = f;
            //return score;
            switch (score)
            {
                case 0:
                    status = "Very Weak";
                    break;
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }

            if (score < 4)
            {
                change_error.ForeColor = Color.Red;
                f = "Status: " + status + "<br>" + feedback;

                return f;
            }
            return "true";
        }

        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select SALT FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Salt"] != null)
                        {
                            if (reader["Salt"] != DBNull.Value)
                            {
                                s = reader["Salt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected string getPWDNSALT(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALTHASH FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALTHASH"] != null)
                        {
                            if (reader["PASSWORDSALTHASH"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALTHASH"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }



        protected string getPWD1(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Password1 FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Password1"] != null)
                        {
                            if (reader["Password1"] != DBNull.Value)
                            {
                                s = reader["Password1"].ToString();
                            }
                            else
                            {
                                s = "null";
                            }

                        }
                        else
                        {
                            s = "null";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected string getPWD2(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Password2,PwdCreatedTime FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Password2"] != null)
                        {
                            if (reader["Password2"] != DBNull.Value)
                            {
                                s = reader["Password2"].ToString();
                            }
                            else
                            {
                                s = "null";
                            }
                        }
                        else
                        {
                            s = "null";
                        }
                        if (reader["PwdCreatedTime"] != null)
                        {
                            if (reader["PwdCreatedTime"] != DBNull.Value)
                            {
                                pwdtime = reader["PwdCreatedTime"].ToString();
                            }
                            else
                            {
                                pwdtime = "null";
                            }
                        }
                        else
                        {
                            pwdtime = "null";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }


        private void updatePWD(String email, String pwd1, String pwd2, String passwordNSalt)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account set Password1=@pwd1, Password2=@pwd2, PasswordSaltHash=@passwordNSalt, PwdCreatedTime = @time WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@pwd1", pwd1);
            cmd.Parameters.AddWithValue("@pwd2", pwd2);
            cmd.Parameters.AddWithValue("@passwordNSalt", passwordNSalt);
            cmd.Parameters.AddWithValue("@time", DateTime.Now);



            try
            {
                connection.Open();
                cmd.CommandText = sql;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                change_error.Text = ex.ToString();
            }
            finally
            {
                connection.Close();
            }
        }
    }
}