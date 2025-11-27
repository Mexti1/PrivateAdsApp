using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfApp3.Data;

namespace WpfApp3.Pages
{
    public partial class AddUserPage : Page
    {
        private Users _editingUser;

        public AddUserPage()
        {
            InitializeComponent();
            comboRole.SelectedIndex = 0;
        }

        // Конструктор для редактирования
        public AddUserPage(Users user) : this()
        {
            _editingUser = user;
            if (user != null)
            {
                TitleText.Text = "Редактирование пользователя";
                txtLogin.Text = user.Login;
                txtFIO.Text = user.FIO;
                comboRole.SelectedItem = (comboRole.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == user.Role));
                // пароль не показываем
            }
        }

        private static string GetHash(string password)
        {
            using (var sha = SHA1.Create())
            {
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new Entities())
                {
                    if (_editingUser == null)
                    {
                        // Добавление
                        if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password) || string.IsNullOrEmpty(txtFIO.Text))
                        {
                            MessageBox.Show("Заполните все поля!");
                            return;
                        }

                        if (db.Users.Any(u => u.Login == txtLogin.Text))
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!");
                            return;
                        }

                        var newUser = new Users
                        {
                            Login = txtLogin.Text,
                            Password = GetHash(txtPassword.Password),
                            Role = (comboRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "User",
                            FIO = txtFIO.Text
                        };
                        db.Users.Add(newUser);
                    }
                    else
                    {
                        // Редактирование
                        var usr = db.Users.Find(_editingUser.ID);
                        if (usr == null)
                        {
                            MessageBox.Show("Пользователь не найден в БД.");
                            return;
                        }

                        usr.Login = txtLogin.Text;
                        if (!string.IsNullOrEmpty(txtPassword.Password))
                            usr.Password = GetHash(txtPassword.Password);
                        usr.Role = (comboRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "User";
                        usr.FIO = txtFIO.Text;
                    }

                    db.SaveChanges();
                    MessageBox.Show("Сохранено!");
                    NavigationService?.Navigate(new AdminPage());
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminPage());
        }
    }
}
