using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_assign1
{
    public partial class Verification : System.Web.UI.Page
    {
        string MYDBConnectionString =
System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string email;

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //if (Request.QueryString["email"] == null)
            //{
            //    Response.Redirect("Registration.aspx", false);

            //}
            //else
            //{
            //    email = Request.QueryString["email"];
            //}

            if (Session["Registered"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Registration.aspx", false);
                }
                else
                {

                    email = Session["Registered"].ToString();
                }

            }
            else
            {
                Response.Redirect("Registration.aspx", false);
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var code = getDBcode(email);
            if(code  == tb_code.Text.Trim())
            {
                updateVerified(email);
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();

                Response.Redirect("Login.aspx", false);

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

            }
            else
            {
                lbl_error.Text = "You have entered an invalid code. Kindly check your email again.";
            }

        }

        protected string getDBcode(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Code FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["Code"] != DBNull.Value)
                        {
                            h = reader["Code"].ToString();
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

        private void updateVerified(String email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account set EmailVerified=@verified WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@verified", "verified");



            try
            {
                connection.Open();
                cmd.CommandText = sql;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }


    }
}