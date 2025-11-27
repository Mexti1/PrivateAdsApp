using PrivateAdsApp.Windows;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;

namespace PrivateAdsApp
{
    public partial class MainWindow : Window
    {
        private AdsDBEntities1 db = new AdsDBEntities1();
        private User currentUser = null;

        public MainWindow()
        {
            InitializeComponent();
            LoadFilters();
            LoadAds();
            UpdateLoginButton();
        }

        private void LoadFilters()
        {
            CmbCity.ItemsSource = db.Cities.ToList();
            CmbCategory.ItemsSource = db.Categories.ToList();
            CmbType.ItemsSource = db.AdTypes.ToList();
            CmbStatus.ItemsSource = db.AdStatuses.ToList();

            CmbCity.SelectedIndex = -1;
            CmbCategory.SelectedIndex = -1;
            CmbType.SelectedIndex = -1;
            CmbStatus.SelectedIndex = -1;
        }

        private void LoadAds()
        {
            var query = db.Ads.AsQueryable();

            // Поиск по ключевым словам
            if (!string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                string search = TxtSearch.Text.ToLower();
                query = query.Where(a => a.Title.Contains(search) ||
           (a.Description != null && a.Description.Contains(search)));
            }

            // Фильтры
            if (CmbCity.SelectedItem is City city) query = query.Where(a => a.CityId == city.CityId);
            if (CmbCategory.SelectedItem is Category cat) query = query.Where(a => a.CategoryId == cat.CategoryId);
            if (CmbType.SelectedItem is AdType type) query = query.Where(a => a.AdTypeId == type.AdTypeId);
            if (CmbStatus.SelectedItem is AdStatus status) query = query.Where(a => a.StatusId == status.StatusId);

            LvAds.ItemsSource = query.OrderByDescending(a => a.PostDate).ToList();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadAds();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWin = new LoginWindow();
            if (loginWin.ShowDialog() == true)
            {
                currentUser = loginWin.LoggedUser;
                UpdateLoginButton();
                MessageBox.Show($"Добро пожаловать, {currentUser.Login}!", "Успешный вход",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateLoginButton()
        {
            if (currentUser != null)
            {
                BtnLogin.Content = "Мои объявления";
                BtnLogin.Click -= BtnLogin_Click;
                BtnLogin.Click += (s, e) => new MyAdsWindow(currentUser).ShowDialog();
            }
            else
            {
                BtnLogin.Content = "Войти";
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadAds();
        }
    }
}