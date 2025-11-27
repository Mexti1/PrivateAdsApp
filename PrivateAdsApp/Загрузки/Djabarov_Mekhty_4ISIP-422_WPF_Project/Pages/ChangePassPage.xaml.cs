using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace Djabarov_Mekhty_4ISIP_422.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbLogin.Text) || string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) || string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("Все поля обязательны"); return;
            }

            if (NewPasswordBox.Password.Length < 6) { MessageBox.Show("Пароль слишком короткий"); return; }
            bool en = true; bool number = false;
            foreach (var ch in NewPasswordBox.Password)
            {
                if (ch >= '0' && ch <= '9') number = true;
                else if (!((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))) en = false;
            }
            if (!en) { MessageBox.Show("Используйте только английскую раскладку"); return; }
            if (!number) { MessageBox.Show("Добавьте хотя бы одну цифру"); return; }
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password) { MessageBox.Show("Пароли не совпадают"); return; }

            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    conn.Open();
                    string hashedCurrent = GetHash(CurrentPasswordBox.Password);
                    string sql = "SELECT ID FROM [User] WHERE Login=@login AND Password=@pass";
                    int? userId = null;
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", TbLogin.Text);
                        cmd.Parameters.AddWithValue("@pass", hashedCurrent);
                        var obj = cmd.ExecuteScalar();
                        if (obj == null) { MessageBox.Show("Текущий логин/пароль неверны"); return; }
                        userId = Convert.ToInt32(obj);
                    }
                    string upd = "UPDATE [User] SET Password=@newpass WHERE ID=@id";
                    using (var cmd = new SqlCommand(upd, conn))
                    {
                        cmd.Parameters.AddWithValue("@newpass", GetHash(NewPasswordBox.Password));
                        cmd.Parameters.AddWithValue("@id", userId.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Пароль успешно изменен");
                    NavigationService?.Navigate(new AuthPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
