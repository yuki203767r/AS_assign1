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

            if (Request.QueryString["email"] == null)
            {
                Response.Redirect("Registration.aspx", false);

            }
            else
            {
                email = Request.QueryString["email"];
            }
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var code = getDBcode(email);
            if(code  == tb_code.Text)
            {
                updateVerified(email);
                Response.Redirect("Login.aspx", false);

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