using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace Djabarov_Mekhty_4ISIP_422.Pages
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;
        public AuthPage()
        {
            InitializeComponent();
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            string hashedPassword = GetHash(PasswordBox.Password);
            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT ID, Role FROM [User] WHERE Login=@login AND Password=@pass";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", TextBoxLogin.Text);
                        cmd.Parameters.AddWithValue("@pass", hashedPassword);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader[1].ToString();
                                if (role == "Admin")
                                    NavigationService?.Navigate(new AdminPage());
                                else
                                    NavigationService?.Navigate(new UserPage());
                            }
                            else
                            {
                                failedAttempts++;
                                txtStatus.Text = $"Неверные данные. Попыток: {failedAttempts}";
                                if (failedAttempts >= 3)
                                {
                                    MessageBox.Show("Требуется капча (не реализована в минимальной версии). Попробуйте позже.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка доступа к БД: " + ex.Message);
            }
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangePassPage());
        }
    }
}
