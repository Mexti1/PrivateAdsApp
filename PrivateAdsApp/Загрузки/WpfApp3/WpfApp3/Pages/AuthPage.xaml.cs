using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WpfApp3.Data;

namespace WpfApp3.Pages
{
    public partial class AuthPage : Page
    {
        private string _captcha;
        public AuthPage()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            var rnd = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            _captcha = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
            CaptchaText.Text = _captcha;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaInput.Text != _captcha)
            {
                MessageBox.Show("Капча введена неверно!");
                GenerateCaptcha();
                return;
            }

            using (var db = new Entities())
            {
                string passHash = GetHash(txtPass.Password);
                var user = db.Users.FirstOrDefault(u => u.Login == txtLogin.Text && u.Password == passHash);
                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль!");
                    return;
                }

                if (user.Role == "Admin")
                    NavigationService.Navigate(new AdminPage());
                else
                    NavigationService.Navigate(new UserPage());
            }
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        public static string GetHash(string text)
        {
            using (SHA1 sha = SHA1.Create())
            {
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("X2")));
            }
        }
    }
}
