using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using WpfApp4.Pages;

namespace WpfApp4.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(oldPassBx.Password) || string.IsNullOrEmpty(newPassBx.Password) || string.IsNullOrEmpty(confirmNewPassBx.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (newPassBx.Password != confirmNewPassBx.Password)
            {
                MessageBox.Show("Новые пароли не совпадают!");
                return;
            }

            if (newPassBx.Password.Length < 6 || !Regex.IsMatch(newPassBx.Password, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Новый пароль должен содержать минимум 6 символов и только английские буквы/цифры!");
                return;
            }

            using (var db = new Djabarov_DBEntities1())
            {
                var user = db.Users.FirstOrDefault(u => u.ID == AuthPage.currentUser.ID);
                if (user == null || user.Password != AuthPage.GetHash(oldPassBx.Password))
                {
                    MessageBox.Show("Старый пароль неверный!");
                    return;
                }

                user.Password = AuthPage.GetHash(newPassBx.Password);
                db.SaveChanges();
                MessageBox.Show("Пароль успешно изменен!");
                NavigationService.Navigate(new AuthPage());
            }
        }
    }
}