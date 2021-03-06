﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace _96_ChangePasswordPage
{
    /* Şifre değiştirme Şayfası
      
      CHANGEPASSWORD.ASPX sayfasının bir QUERYSTRING "GUID" var. 
      Bu GIUD değiştirilmesi gereken şifrenini USERID'sı ne bakmak için kullanılır. 
      Şifre güncelemesinden sonra TBLRESETPASSWORDREQUESTS'dan satırı sil,
      yani link şifre değişimi sonrası geçersiz olur.
      kullanıcı id leri integer olduğundan, Onlar diğer kullanıcıların şifrelerini değiştirmek için ve QUERYSTRING değeri olarak rasgele int'ler kullanması çok kolay olduğu için  suistimal          edebilirler.
      Kısaca link için kullanılan GIUD in sürekli değişmesi gerekiyor
      
      In the next video, we will discuss about changing password by providing the current password. 
     */
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            AuthenticateUser(txtUserName.Text, txtPassword.Text);
        }
        private void AuthenticateUser(string username, string password)
        {
            string CS = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spAuthenticateUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                string encryptedpassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");

                SqlParameter paramUsername = new SqlParameter("@UserName", username);
                SqlParameter paramPassword = new SqlParameter("@Password", encryptedpassword);

                cmd.Parameters.Add(paramUsername);
                cmd.Parameters.Add(paramPassword);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    int RetryAttempts = Convert.ToInt32(rdr["RetryAttempts"]);
                    if (Convert.ToBoolean(rdr["AccountLocked"]))
                    {
                        lblMessage.Text = "Account locked. Please contact administrator";
                    }
                    else if (RetryAttempts > 0)
                    {
                        int AttemptsLeft = (4 - RetryAttempts);
                        lblMessage.Text = "Invalid user name and/or password. " +
                            AttemptsLeft.ToString() + "attempt(s) left";
                    }
                    else if (Convert.ToBoolean(rdr["Authenticated"]))
                    {
                        FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, chkBoxRememberMe.Checked);
                    }
                }
            }
        }
    }
}