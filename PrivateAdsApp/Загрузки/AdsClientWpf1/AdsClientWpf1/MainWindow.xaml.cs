using AdsClientWpf.Data;
using AdsClientWpf.Helpers;
using AdsClientWpf.Models;
using AdsClientWpf1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;

namespace AdsClientWpf
{
    public partial class MainWindow : Window
    {
        private readonly AdsDbContext _db;
        private List<dynamic> _items = new();

        public string CurrentUserText => App.CurrentUser != null ? $"Пользователь: {App.CurrentUser.FullName ?? App.CurrentUser.Login}" : "Гость";

        public MainWindow()
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseSqlServer("Server=.;Database=AdsDB;Trusted_Connection=True;")
                .Options;
            _db = new AdsDbContext(options);

            LoadFilters();
            LoadAds();
            DataContext = this;
        }

        private void LoadFilters()
        {
            cbCity.ItemsSource = _db.Cities.OrderBy(c => c.Name).ToList();
            cbCategory.ItemsSource = _db.Categories.OrderBy(c => c.Name).ToList();
            cbType.ItemsSource = _db.AdTypes.OrderBy(t => t.Name).ToList();
            cbStatus.ItemsSource = _db.AdStatuses.OrderBy(s => s.Name).ToList();
        }

        private void LoadAds(string keyword = null, int? cityId = null, int? catId = null, int? typeId = null, int? statusId = null)
        {
            var q = _db.Ads.Include(a => a.Photos)
                            .Include(a => a.Category)
                            .Include(a => a.City)
                            .Include(a => a.AdType)
                            .Include(a => a.AdStatus)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                q = q.Where(a => a.Title.Contains(keyword) || (a.Description ?? "").Contains(keyword));
            }
            if (cityId.HasValue) q = q.Where(a => a.CityId == cityId.Value);
            if (catId.HasValue) q = q.Where(a => a.CategoryId == catId.Value);
            if (typeId.HasValue) q = q.Where(a => a.AdTypeId == typeId.Value);
            if (statusId.HasValue) q = q.Where(a => a.AdStatusId == statusId.Value);

            var list = q.OrderByDescending(a => a.CreatedAt).ToList();
            _items = list.Select(a => new
            {
                a.AdId,
                a.Title,
                ShortDescription = a.Description?.Length > 250 ? a.Description.Substring(0, 250) + "..." : a.Description,
                PriceText = a.Price.HasValue ? $"Цена: {a.Price} ₽" : "Цена: не указана",
                CategoryName = a.Category?.Name,
                CityName = a.City?.Name,
                MainImage = ImageHelper.BytesToImageSource(a.MainImageBytes)
            }).ToList();
            lvAds.ItemsSource = _items;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadAds(txtSearch.Text,
                    (cbCity.SelectedItem as City)?.CityId,
                    (cbCategory.SelectedItem as Category)?.CategoryId,
                    (cbType.SelectedItem as AdType)?.AdTypeId,
                    (cbStatus.SelectedItem as AdStatus)?.AdStatusId);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new EditAdWindow(null);
            wnd.ShowDialog();
            LoadAds();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int adId)
            {
                var wnd = new EditAdWindow(adId);
                wnd.ShowDialog();
                LoadAds();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int adId)
            {
                var res = MessageBox.Show("Удалить объявление?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes) return;
                var ad = _db.Ads.Include(a => a.Photos).FirstOrDefault(a => a.AdId == adId);
                if (ad != null)
                {
                    _db.Ads.Remove(ad);
                    _db.SaveChanges();
                    LoadAds();
                }
            }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int adId)
            {
                var ad = _db.Ads.FirstOrDefault(a => a.AdId == adId);
                if (ad == null) return;

                // Диалог ввода суммы
                var input = Microsoft.VisualBasic.Interaction.InputBox("Введите сумму продажи (целое >=0):", "Завершение объявления", ad.CompletedAmount?.ToString() ?? "0");
                if (!int.TryParse(input, out int sum) || sum < 0)
                {
                    MessageBox.Show("Неверная сумма.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ad.CompletedAmount = sum;
                var completedStatus = _db.AdStatuses.FirstOrDefault(s => s.Name == "Завершено");
                if (completedStatus == null)
                {
                    MessageBox.Show("Статус 'Завершено' не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ad.AdStatusId = completedStatus.AdStatusId;
                ad.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges(); // триггер создаст запись в Sales
                MessageBox.Show("Объявление завершено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAds();
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int adId)
            {
                var ad = _db.Ads.Include(a => a.Photos).Include(a => a.Category).Include(a => a.City).Include(a => a.AdStatus).FirstOrDefault(a => a.AdId == adId);
                if (ad == null) return;
                var text = $"Заголовок: {ad.Title}\nКатегория: {ad.Category?.Name}\nГород: {ad.City?.Name}\nЦена: {(ad.Price.HasValue ? ad.Price.Value.ToString() : "не указана")}\n\n{ad.Description}";
                MessageBox.Show(text, "Просмотр объявления", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCompleted_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new CompletedAdsWindow();
            wnd.ShowDialog();
        }
    }
}
