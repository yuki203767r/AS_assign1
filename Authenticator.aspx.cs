using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Web.Profile;
using System.Web.Security;
using Google.Authenticator;
using System.Data.SqlClient;
using System.Data;

namespace AS_assign1
{
    public partial class Authenticator : System.Web.UI.Page
    {
        string MYDBConnectionString =
System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static int maxpwdtime = 5;
        static string pwdtime;
        static string email;
        static string logMessage;
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");
        }

        String AuthenticationCode
        {
            get
            {
                if (ViewState["AuthenticationCode"] != null)
                    return ViewState["AuthenticationCode"].ToString().Trim();
                return String.Empty;
            }
            set
            {
                ViewState["AuthenticationCode"] = value.Trim();
            }
        }

        String AuthenticationTitle
        {
            get
            {
                return "AS ASSIGNMENT" ;
            }
        }


        String AuthenticationBarCodeImage
        {
            get;
            set;
        }

        String AuthenticationManualCode
        {
            get;
            set;
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
  
                    email = Session["LoggedIn"].ToString();
                }

            }

            else
            {
                Response.Redirect("Login.aspx", false);
            }

            if (!Page.IsPostBack)
            {
                lblResult.Text = String.Empty;
                lblResult.Visible = false;
                GenerateTwoFactorAuthentication();
                imgQrCode.ImageUrl = AuthenticationBarCodeImage;
                lblManualSetupCode.Text = AuthenticationManualCode;
                lblAccountName.Text = AuthenticationTitle;
            }
        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {
            string data = getDBdata(email);

            var pwdtimecreated = Convert.ToDateTime(pwdtime);
            DateTime timenow = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            TimeSpan nowcreate = timenow.Subtract(pwdtimecreated);
            int minutescreated = Convert.ToInt32(nowcreate.TotalMinutes);
            int pwdusetime = maxpwdtime - minutescreated;

            String pin = txtSecurityCode.Text.Trim();
            Boolean status = ValidateTwoFactorPIN(pin);
            if (status)
            {
                //lblResult.Visible = true;
                //lblResult.Text = "Code Successfully Verified.";
                if (pwdusetime <= 0)
                {
                    //login_error.Text = "Your password has been used for " + minutescreated + " min";
                    Response.Redirect("Change.aspx?Status=yes", false);


                }
                else
                {

                    Response.Redirect("Home.aspx", false);
                }
                logMessage = "Authentication succeed";

                InsertLog(email, logMessage);

            }
            else
            {
                logMessage = "Authentication failed";
                InsertLog(email, logMessage);

                lblResult.Visible = true;
                lblResult.Text = "Invalid Code.";
                return;
            }
        }

        public Boolean ValidateTwoFactorPIN(String pin)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(AuthenticationCode, pin);
        }

        public Boolean GenerateTwoFactorAuthentication()
        {
            Guid guid = Guid.NewGuid();
            String uniqueUserKey = Convert.ToString(guid).Replace("-", "").Substring(0, 10);
            AuthenticationCode = uniqueUserKey;

            Dictionary<String, String> result = new Dictionary<String, String>();
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("Complio", AuthenticationTitle, AuthenticationCode, false, 3); if (setupInfo != null)
            {
                AuthenticationBarCodeImage = setupInfo.QrCodeSetupImageUrl;
                AuthenticationManualCode = setupInfo.ManualEntryKey;
                return true;
            }
            return false;
        }

        protected string getDBdata(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PwdCreatedTime FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
            
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
                                throw ex;
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
    
