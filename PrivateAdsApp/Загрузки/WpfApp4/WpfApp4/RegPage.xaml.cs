using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp4.Pages;

namespace WpfApp4.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0; // По умолчанию User
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) || string.IsNullOrEmpty(txtbxFIO.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) || string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            // Проверка пароля: 6+ символов, только англ.
            if (passBxFrst.Password.Length < 6 || !Regex.IsMatch(passBxFrst.Password, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов и только английские буквы/цифры!");
                return;
            }

            using (var db = new Djabarov_DBEntities1())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == txtbxLog.Text);
                if (user != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                string hashedPassword = AuthPage.GetHash(passBxFrst.Password);
                db.Users.Add(new User
                {
                    Login = txtbxLog.Text,
                    Password = hashedPassword,
                    Role = (comboBxRole.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    FIO = txtbxFIO.Text
                });
                db.SaveChanges();
                MessageBox.Show("Регистрация успешна!");
                NavigationService.Navigate(new AuthPage());
            }
        }

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLogHitn.Visibility = txtbxLog.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblFioHitn.Visibility = txtbxFIO.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassHitn.Visibility = passBxFrst.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassSecHitn.Visibility = passBxScnd.Password.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void lblLogHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxLog.Focus();
        }

        private void lblPassHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxFrst.Focus();
        }

        private void lblPassSecHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxScnd.Focus();
        }

        private void lblFioHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxFIO.Focus();
        }
    }
}