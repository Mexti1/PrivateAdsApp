using System.Windows;
using AdsClientWpf.Data;
using Microsoft.EntityFrameworkCore;

namespace AdsClientWpf
{
    public partial class LoginWindow : Window
    {
        private readonly AdsDbContext _db;

        public LoginWindow()
        {
            InitializeComponent();

            // Настройте connection string здесь
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseSqlServer("Server=.;Database=AdsDB;Trusted_Connection=True;")
                .Options;
            _db = new AdsDbContext(options);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var pwd = pbPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == pwd);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            App.CurrentUser = user;
            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
