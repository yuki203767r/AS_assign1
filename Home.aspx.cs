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
    public partial class Home : System.Web.UI.Page
    {
        string MYDBConnectionString =
System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] credit = null;
        string userID = null;
        byte[] Key;
        byte[] IV;
        static string logMessage;
        static string f;
        static int timeout;
        string finalHash;

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");
        }

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
                    displayProfile(Session["LoggedIn"].ToString());
                    message.Text = "Congratulations! you are logged in .";
                    message.ForeColor = System.Drawing.Color.Green;
                    //btn_logout.Visible = true;
                }

            }

            else
            {
                Response.Redirect("Login.aspx", false);
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!this.IsPostBack)
            {
                Session["Reset"] = true;
   
            }
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~/Web.Config");
            SessionStateSection section = (SessionStateSection)config.GetSection("system.web/sessionState");
            timeout = (int)section.Timeout.TotalMinutes * 1000 * 60;
            ClientScript.RegisterStartupScript(this.GetType(), "SessionAlert", "SessionExpireAlert(" + timeout + ");", true);

        }

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();


            Response.Redirect("Login.aspx", false);
            try
            {
                logMessage = "successful log out";

                InsertLog(dis_email.Text, logMessage);
            }
            catch 
            {
                logMessage = "failed log out";

                InsertLog(dis_email.Text, logMessage);
            }


            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
            //            Response.Cookies.Clear();
        }

        protected void displayProfile(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value)
                        {
                            dis_email.Text = reader["Email"].ToString();
                        }
                        if (reader["FName"] != DBNull.Value && reader["LName"] != DBNull.Value)
                        {

                            dis_name.Text = reader["FName"].ToString() + " " + reader["LName"].ToString();
                        }
                        if (reader["CCInfo"] != DBNull.Value)
                        {
                            credit = Convert.FromBase64String(reader["CCInfo"].ToString());

                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                        if (reader["Photo"] != DBNull.Value)
                        {
                            Image1.ImageUrl = reader["Photo"].ToString();
                        }
                        if (reader["DateOfBirth"] != DBNull.Value)
                        {
                            dis_dob.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("dd MMM yyyy ");
                            //dis_dob.Text = DateTime.ParseExact(reader["DateOfBirth"].ToString() , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("MMM. dd, yyyy HH:mm:ss");

                        }


                        dis_credit.Text = decryptData(credit);

                    }
                }
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { }
            return plainText;
        }

        private void InsertLog(string email, string logMessage)
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Auditlog VALUES(@email, @timenow, @event)"))

                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@timenow", DateTime.Now);
                            cmd.Parameters.AddWithValue("@event", logMessage);

                            try
                            {
                                cmd.Connection = sqlcon;
                                sqlcon.Open();
                                cmd.ExecuteNonQuery();
                                return;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.ToString());
                                //home_error.Text = "failed to log out due to " + ex.ToString();
                                //emailchecker.Text = "USE ANOTHER EMAIL PLS LAH SO TIRED";
                            }
                            finally
                            {
                                sqlcon.Close();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //protected void Button1_Click(object sender, EventArgs e)
        //{
            //if (String.IsNullOrEmpty(changepwd.Text.ToString()))
            //{
            //    return;
            //}
            //else
            //{
            //    if (true)
            //    {
            //        var salt = getDBSalt(dis_email.Text);
            //        var passwordNsalt = getPWDNSALT(dis_email.Text);
            //        var pwd1 = getPWD1(dis_email.Text);
            //        var pwd2 = getPWD2(dis_email.Text);
            //        string newpwdNSalt = changepwd.Text + salt;

            //        SHA512Managed hashing = new SHA512Managed();
            //        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdNSalt));
            //        finalHash = Convert.ToBase64String(hashWithSalt);
            //        if (finalHash == passwordNsalt || finalHash == pwd1 || finalHash == pwd2)
            //        {
            //            return;
            //        }
            //        else
            //        {
            //            updatePWD(dis_email.Text, pwd2, passwordNsalt, finalHash);
            //        }
            //    }
            //    else
            //    {
            //        return;
            //    }


            //}
        //}


        //private string password_checking(string password)
        //{
        //    int score = 0; string status = "";

        //    if (password.Length < 12)
        //    {
        //        f += " Password not enough length!\n ";
        //        //return 1;
        //    }
        //    else
        //    {
        //        score = 1;
        //    }

        //    if (Regex.IsMatch(password, "[a-z]"))
        //    {
        //        score++;
        //    }
        //    else
        //    {
        //        f += " Password need to have at least 1 lowercase! ";
        //    }

        //    if (Regex.IsMatch(password, "[A-Z]"))
        //    {
        //        score++;
        //    }
        //    else
        //    {
        //        f += " Password need to have at least 1 uppercase!\n ";
        //    }

        //    if (Regex.IsMatch(password, "[0-9]"))
        //    {
        //        score++;
        //    }
        //    else
        //    {
        //        f += " Password need to have at least 1 digit!\n ";
        //    }

        //    if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
        //    {
        //        score++;
        //    }
        //    else
        //    {
        //        f += " Password need to have at least 1 special symbol! \n";
        //    }
        //    string feedback = f;
        //    //return score;
        //    switch (score)
        //    {
        //        case 0:
        //            status = "Very Weak";
        //            break;
        //        case 1:
        //            status = "Very Weak";
        //            break;
        //        case 2:
        //            status = "Weak";
        //            break;
        //        case 3:
        //            status = "Medium";
        //            break;
        //        case 4:
        //            status = "Strong";
        //            break;
        //        case 5:
        //            status = "Excellent";
        //            break;
        //        default:
        //            break;
        //    }

        //    if (score < 4)
        //    {
        //        lbl_pwd.ForeColor = Color.Red;
        //        f = "Status: " + status + "<br>" + feedback;

        //        return f;
        //    }
        //    return "true";
        //}

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
            string sql = "select Password2 FROM ACCOUNT WHERE Email=@Email";
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


        private void updatePWD(String email,String pwd1, String pwd2, String passwordNSalt)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account set Password1=@pwd1, Password2=@pwd2, PasswordSaltHash=@passwordNSalt WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@pwd1", pwd1);
            cmd.Parameters.AddWithValue("@pwd2", pwd2);
            cmd.Parameters.AddWithValue("@passwordNSalt", passwordNSalt);


            try
            {
                connection.Open();
                cmd.CommandText = sql;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change.aspx", false);

        }
    }
}
