using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Configuration;

namespace AS_assign1
{
    public partial class Registration : System.Web.UI.Page

    {
        public string f;
        string MYDBConnectionString =
System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        static string code;
        static string  smtpemail;
                        static string smtppwd;
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (!(String.IsNullOrEmpty(pwd.Text.Trim())))
                {
                    pwd.Attributes["value"] = pwd.Text;
                }
                if (!(String.IsNullOrEmpty(cfmpwd.Text.Trim())))
                {
                    cfmpwd.Attributes["value"] = cfmpwd.Text;
                }
            }
        }
        //protected void dob_SelectionChanged(object sender, EventArgs e)
        //{

        //}
        private int password_checking(string password)
        {
            int score = 0;
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
            return score;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(FName.Text))
            {
                fnamechecker.Text = "first name is required";
            }
            else if (Regex.IsMatch(FName.Text.ToString(), "[^A-Za-z]"))
            {
                fnamechecker.Text = "only allow alphabet ";
            }
            else
            {
                fnamechecker.Text = "";
            }

            if (String.IsNullOrEmpty(LName.Text))
            {
                lnchecker.Text = "last name is required";
            }
            else if (Regex.IsMatch(LName.Text.ToString().Trim(), "[^a-zA-Z]"))
            {
                lnchecker.Text = "only allow alphabet";
            }
            else
            {
                lnchecker.Text = "";
            }

            if (String.IsNullOrEmpty(cc.Text))
            {
                ccchecker.Text = "credit card info is required";

            }
            else if (cc.Text.ToString().Length >= 13)
            {
                ccchecker.Text = "exceeds 12 numbers";

            }
            else if (cc.Text.ToString().Length < 12)
            {
                ccchecker.Text = "need 12 numbers";

            }

            if (Regex.IsMatch(cc.Text, "[^0-9]"))
            {
                ccchecker.Text = "only numbers allowed";

            }
            else if (Regex.IsMatch(cc.Text, "[0-9]"))
            {
                ccchecker.Text = "";
            }


            if (String.IsNullOrEmpty(email.Text))
            {
                emailchecker.Text = "email is required";

            }
            else
            {
                emailchecker.Text = "";
            }




            if (String.IsNullOrEmpty(pwd.Text))
            {
                passchecker.Text = "password is required";
            }
            else
            {
                passchecker.Text = "";
            }
            int scores = password_checking(HttpUtility.HtmlEncode(pwd.Text));


            if (String.IsNullOrEmpty(cfmpwd.Text))
            {
                cfmchecker.Text = "confirm password is required";

            }
            else if (pwd.Text.ToString().Trim() != cfmpwd.Text.ToString().Trim())
            {
                cfmpwdchecker.ForeColor = Color.Red;
                cfmpwdchecker.Text = "password not match!";
            }
            else
            {
                cfmchecker.Text = "";
            }

            if (String.IsNullOrEmpty(dob.Text))
            {
                dobchecker.Text = "dob is required";
                //return;

            }
            else
            {
                dobchecker.Text = "";
            }

            if (String.IsNullOrEmpty(photo.ImageUrl))
            {
                photochecker.Text = "photo is required";
                //return;

            }
            else
            {
                photochecker.Text = "";
            }

            string status = "";
            string feedback = f;
            string password = pwd.Text.ToString().Trim();
            string cfmpassword = cfmpwd.Text.ToString().Trim();



            switch (scores)
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

            if (scores < 4)
            {
                error_message.ForeColor = Color.Red;
                passchecker.Text = "Status: " + status + "<br>" + feedback;

                return;
            }

            passchecker.ForeColor = Color.Green;
            passchecker.Text = "Status: " + status;



            if (password != cfmpassword)
            {
                cfmpwdchecker.ForeColor = Color.Red;
                cfmpwdchecker.Text = "password not match!";
                //return;
            }
            else
            {
                //password hasing
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdNSalt = password + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(password));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdNSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                //verification code
                Random generator = new Random();
                code = generator.Next(0, 1000000).ToString("D6");
                
                createAccount();
            }


            

            //            Response.Redirect("Success.aspx?Data=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode(FName.Text + LName.Text + cc.Text + email.Text + pwd.Text  + photo.ImageUrl)));

        }


        protected void uploadbtn_Click(object sender, EventArgs e)
        {
            string photoPath = Server.MapPath("~/uploadPhoto/");

            if (!Directory.Exists(photoPath))
            {
                Directory.CreateDirectory(photoPath);
            }

            try
            {
                string fileExtension = Path.GetExtension(Path.GetFileName(FileUpload1.FileName));
                if (fileExtension.ToLower() == ".jpg" || fileExtension.ToLower() == ".bmp" ||
                   fileExtension.ToLower() == ".jpeg" || fileExtension.ToLower() == ".gif" ||
                   fileExtension.ToLower() == ".png")
                {
                    FileUpload1.SaveAs(photoPath + Path.GetFileName(FileUpload1.FileName));

                    photo.ImageUrl = "~/uploadPhoto/" + Path.GetFileName(FileUpload1.FileName);
                    photochecker.Text = "";

                }
                else
                {
                    photochecker.Text = "invalid photo format! only .gif/.jpg/.jpeg/.png/.bmp are allowed!";
                }
            }
            catch (Exception)
            {
                photochecker.Text = "invalid photo!";

            }
        }

        protected void createAccount()
        {

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FName, @LName, @CCInfo, " +
                        "@Email, @PasswordSaltHash, @Salt, @DateTimeRegistered, @DateOfBirth, @Photo, @EmailVerified," +
                        "@IV,@Key,@Status,@LockedTime,@Password1,@Password2,@Code,@pwdtimecreated)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FName", FName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LName", LName.Text.Trim());
                            cmd.Parameters.AddWithValue("@CCInfo", Convert.ToBase64String(encryptData(cc.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", email.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordSaltHash", finalHash);
                            cmd.Parameters.AddWithValue("@Salt", salt);
                            cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                            cmd.Parameters.AddWithValue("@DateOfBirth", dob.Text);
                            cmd.Parameters.AddWithValue("@Photo", photo.ImageUrl);
                            cmd.Parameters.AddWithValue("@EmailVerified", "unverified");
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@Status", DBNull.Value);
                            cmd.Parameters.AddWithValue("@LockedTime", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Password1", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Password2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Code", code);
                            cmd.Parameters.AddWithValue("@pwdtimecreated", DateTime.Now);


                            try
                            {
                                cmd.Connection = sqlcon;
                                sqlcon.Open();
                                cmd.ExecuteNonQuery();
                                sendcode();

                                Response.Redirect("Verification.aspx?Email=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode(email.Text.Trim())),false);
                            }
                            catch (Exception ex)
                            {
                                lbl_error.Text = "invalid input";


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






        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        private void sendcode()
        {

            smtpemail = Environment.GetEnvironmentVariable("smtpemail");
            // If necessary, create it.
            if (smtpemail == null)
            {
                Environment.SetEnvironmentVariable("smtpemail", "eduyuki19@gmail.com");

                // Now retrieve it.
                smtpemail = Environment.GetEnvironmentVariable("smtpemail");
            }
            if (smtppwd == null)
            {
                Environment.SetEnvironmentVariable("smtppwd", "3DUyuk!19");

                // Now retrieve it.
                smtppwd = Environment.GetEnvironmentVariable("smtppwd");
            }

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential(smtpemail,smtppwd);
            smtp.EnableSsl=true;
            MailMessage msg = new MailMessage();
            msg.Subject = "Verification code to verify your account";
            msg.Body = "Dear " + FName.Text.Trim() + " " + LName.Text.Trim() + ", Your verification code is "
                + code + ".\n\n\nThank you";
            msg.To.Add(email.Text.Trim());
            string fromaddress = "AS assignment <eduyuki19@gmail.com>";
            msg.From = new MailAddress(fromaddress);
            try
            {
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
        
}