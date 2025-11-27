using System.Linq;
using System.Windows;

namespace PrivateAdsApp.Windows
{
    public partial class LoginWindow : Window
    {
        private AdsDBEntities1 db = new AdsDBEntities1();
        public User LoggedUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                LoggedUser = user;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}