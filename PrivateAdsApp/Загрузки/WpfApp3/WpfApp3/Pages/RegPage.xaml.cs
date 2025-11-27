using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfApp3.Data;

namespace WpfApp3.Pages
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
            using (var sha = SHA1.Create())
            {
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) || string.IsNullOrEmpty(passBxFrst.Password) ||
                string.IsNullOrEmpty(passBxScnd.Password) || string.IsNullOrEmpty(txtbxFIO.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (passBxFrst.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, минимум 6 символов!");
                return;
            }

            bool en = true;
            bool number = false;
            foreach (var ch in passBxFrst.Password)
            {
                if (ch >= '0' && ch <= '9') number = true;
                else if (!((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))) en = false;
            }

            if (!en)
            {
                MessageBox.Show("Используйте только английскую раскладку!");
                return;
            }
            if (!number)
            {
                MessageBox.Show("Добавьте хотя бы одну цифру!");
                return;
            }
            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            try
            {
                using (var db = new Entities())
                {
                    var exists = db.Users.FirstOrDefault(u => u.Login == txtbxLog.Text);
                    if (exists != null)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!");
                        return;
                    }

                    var userObject = new WpfApp3.Data.Users
                    {
                        FIO = txtbxFIO.Text,
                        Login = txtbxLog.Text,
                        Password = GetHash(passBxFrst.Password),
                        Role = (comboBxRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "User"
                    };

                    db.Users.Add(userObject);
                    db.SaveChanges();
                    MessageBox.Show("Пользователь успешно зарегистрирован!");
                    // Очистка полей
                    txtbxLog.Clear(); passBxFrst.Clear(); passBxScnd.Clear(); txtbxFIO.Clear();
                    NavigationService?.GoBack();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
