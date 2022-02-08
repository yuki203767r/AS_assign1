using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_assign1
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString =
System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        static string lockstatus;
        static int attemptcount;
        static string logMessage;
        static string emailverified;
        static string pwdtime;
        static int maxpwdtime = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!this.IsPostBack)
            {


                Session["invalidattempt"] = null;

            }

        }

        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6Lc1bTodAAAAAIHsIffLWxwomKG4_rXjh5Q4cq-0 &response=" + captchaResponse);


            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {

                        string captchajsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(captchajsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);//
                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
        protected void Loginbtn(object sender, EventArgs e)
        {
            try
            {
                string password = login_pwd.Text.Trim();
                string email = login_email.Text.ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);
                string status = getStatus(email);

                //var pwdtimecreated = Convert.ToDateTime(pwdtime);
                //DateTime timenow = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                //TimeSpan nowcreate = timenow.Subtract(pwdtimecreated);
                //int minutescreated = Convert.ToInt32(nowcreate.TotalMinutes);
                //int pwdusetime = maxpwdtime - minutescreated;

                if (ValidateCaptcha())
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdNSalt = password + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdNSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        //login_error.Text =userHash+"\n"+dbHash ;

                        if (status == "true" || status == null)
                        {
                            if (emailverified == "verified")
                            {
                                if (userHash.Equals(dbHash))
                                {
                                    Session["LoggedIn"] = login_email.Text.Trim();

                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    // now create a new cookie with this guid value
                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                    Response.Redirect("Authenticator.aspx", false);
                                    logMessage = "login done ";


                                    //if (pwdusetime <= 0)
                                    //{
                                    //    //login_error.Text = "Your password has been used for " + minutescreated + " min";
                                    //    Response.Redirect("Change.aspx?Status=yes", false);


                                    //}
                                    //else
                                    //{
                                    //    logMessage = "successful login";
                                    //    InsertLog(email, logMessage);

                                    //    Response.Redirect("Home.aspx", false);
                                    //}

                                }
                                else
                                {
                                    login_error.Text = "Invalid input" ;
                                        //"No. of Attemps Remaining : " + (2 - attemptcount);
                                    attemptcount = attemptcount + 1;
                                    Session["invalidattempt"] = attemptcount;

                                    logMessage = "Failed to login " + attemptcount + " time";

                                    if (attemptcount == 3)
                                    {
                                        login_error.Text = "Your Account Has Been Locked For Three Minutes Due to Three Invalid Attempts.";
                                        setlockstatus(email);
                                        logMessage = "Failed to login " + attemptcount + " time. Account lockout!";

                                        attemptcount = 0;

                                    }

                                }
                                InsertLog(email, logMessage);

                            }
                            else
                            {
                                login_error.Text = "Kindly check your email for verification code before logging in";
                            }
                        }
                        else
                        {
                            DateTime locktime = Convert.ToDateTime(getlockedtime(email));
                            DateTime timenow = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            TimeSpan ts = timenow.Subtract(locktime);
                            int minutesleft = Convert.ToInt32(ts.TotalMinutes);
                            int pendingminutes = 3 - minutesleft;

                            if (pendingminutes <= 0)
                            {
                                unlockacc(email);
                                if (userHash.Equals(dbHash))
                                {
                                    Session["LoggedIn"] = login_email.Text.Trim();

                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    // now create a new cookie with this guid value
                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                    logMessage = "login done";
                                    InsertLog(email, logMessage);

                                    Response.Redirect("Authenticator.aspx", false);
                                }
                                else
                                {
                                    login_error.Text = "failed validation";
                                }
                            }
                            else
                            {

                                login_error.Text = "Your Account Locked Already! " + pendingminutes + " minutes left";
                                logMessage = "Try to log in but account still locked for " +pendingminutes + " min";
                                InsertLog(email, logMessage);
                            }

                        }

                    }

                    else
                    {
                        login_error.Text = "Invalid input";

                        logMessage = "User entered wrong username / password";
                        InsertLog(email, logMessage);
                    }
                    logMessage = "Captcha validation passed";

                }

                else
                {
                    login_error.Text = "Failed validation";

                    logMessage = "Captcha validation failed";
                    InsertLog(email, logMessage);
                }
            }
            catch
            {
                login_error.Text = "invalid input";

            }
        }

        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordSaltHash FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordSaltHash"] != DBNull.Value)
                        {
                            h = reader["PasswordSaltHash"].ToString();
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
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

        protected string getStatus(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select STATUS, EMAILVERIFIED,PwdCreatedTime FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Status"] != null)
                        {
                            if (reader["Status"] != DBNull.Value)
                            {
                                s = reader["Status"].ToString();
                            }
                            else
                            {
                                s = null;
                            }
                        }
                        if (reader["EMAILVERIFIED"] != null)
                        {
                            if (reader["EMAILVERIFIED"] != DBNull.Value)
                            {
                                emailverified = reader["EMAILVERIFIED"].ToString();
                            }
                            else
                            {
                                emailverified = null;
                            }
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

        protected string getlockedtime(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select lockedtime FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["lockedtime"] != null)
                        {
                            if (reader["lockedtime"] != DBNull.Value)
                            {
                                s = reader["lockedtime"].ToString();
                            }
                            else
                            {
                                s = null;
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

        private void setlockstatus(String email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account set status=@status, lockedtime = @time WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@status", "false");
            cmd.Parameters.AddWithValue("@time", DateTime.Now);

            connection.Open();
            cmd.CommandText = sql;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            connection.Close();

        }

        private void unlockacc(String email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account set status=@status, lockedtime = @time WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@status", DBNull.Value);
            cmd.Parameters.AddWithValue("@time", DBNull.Value);

            connection.Open();
            cmd.CommandText = sql;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            connection.Close();

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

                                //login_error.Text = ex.ToString();
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


    }
}