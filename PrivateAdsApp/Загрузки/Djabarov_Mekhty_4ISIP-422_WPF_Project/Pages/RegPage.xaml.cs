using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace Djabarov_Mekhty_4ISIP_422.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) || string.IsNullOrEmpty(passBxFrst.Password) ||
                string.IsNullOrEmpty(passBxScnd.Password) || string.IsNullOrEmpty(txtbxFIO.Text))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (passBxFrst.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий (минимум 6 символов)");
                return;
            }

            bool en = true; bool number = false;
            foreach (var ch in passBxFrst.Password)
            {
                if (ch >= '0' && ch <= '9') number = true;
                else if (!((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))) en = false;
            }
            if (!en) { MessageBox.Show("Используйте только английскую раскладку"); return; }
            if (!number) { MessageBox.Show("Добавьте хотя бы одну цифру"); return; }
            if (passBxFrst.Password != passBxScnd.Password) { MessageBox.Show("Пароли не совпадают"); return; }

            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    conn.Open();
                    string check = "SELECT COUNT(*) FROM [User] WHERE Login=@login";
                    using (var cmd = new SqlCommand(check, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", txtbxLog.Text);
                        int exists = (int)cmd.ExecuteScalar();
                        if (exists > 0) { MessageBox.Show("Пользователь с таким логином уже существует"); return; }
                    }

                    string insert = "INSERT INTO [User] (Login, Password, Role, FIO) VALUES (@login, @pass, @role, @fio)";
                    using (var cmd = new SqlCommand(insert, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", txtbxLog.Text);
                        cmd.Parameters.AddWithValue("@pass", GetHash(passBxFrst.Password));
                        cmd.Parameters.AddWithValue("@role", ((ComboBoxItem)comboBxRole.SelectedItem).Content.ToString());
                        cmd.Parameters.AddWithValue("@fio", txtbxFIO.Text);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Пользователь успешно зарегистрирован");
                    NavigationService?.Navigate(new AuthPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при работе с БД: " + ex.Message);
            }
        }
    }
}
