using AdsClientWpf.Data;
using AdsClientWpf.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;

namespace AdsClientWpf
{
    public partial class EditAdWindow : Window
    {
        private readonly AdsDbContext _db;
        private Ad? _ad;
        private int? _adId;

        public string WindowTitle { get; set; } = "Новое объявление";

        public EditAdWindow(int? adId)
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseSqlServer("Server=.;Database=AdsDB;Trusted_Connection=True;")
                .Options;
            _db = new AdsDbContext(options);

            _adId = adId;
            if (adId.HasValue)
            {
                _ad = _db.Ads.Include(a => a.Photos).FirstOrDefault(a => a.AdId == adId.Value);
                WindowTitle = "Редактирование объявления";
            }

            LoadLookups();
            FillForm();
            DataContext = this;
        }

        private void LoadLookups()
        {
            cbCategory.ItemsSource = _db.Categories.OrderBy(c => c.Name).ToList();
            cbCity.ItemsSource = _db.Cities.OrderBy(c => c.Name).ToList();
            cbType.ItemsSource = _db.AdTypes.OrderBy(t => t.Name).ToList();
            cbStatus.ItemsSource = _db.AdStatuses.OrderBy(s => s.Name).ToList();
        }

        private void FillForm()
        {
            if (_ad == null) return;
            txtTitle.Text = _ad.Title;
            txtDescription.Text = _ad.Description;
            txtPrice.Text = _ad.Price?.ToString();
            cbCategory.SelectedValue = _ad.CategoryId;
            cbCity.SelectedValue = _ad.CityId;
            cbType.SelectedValue = _ad.AdTypeId;
            cbStatus.SelectedValue = _ad.AdStatusId;
            lbPhotos.ItemsSource = _ad.Photos?.ToList() ?? new List<Photo>();
        }

        private void BtnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() != true) return;

            var bytes = File.ReadAllBytes(ofd.FileName);
            var photo = new Photo
            {
                FileName = Path.GetFileName(ofd.FileName),
                ImageData = bytes,
                IsMain = false
            };

            if (_ad == null)
            {
                // временно создаём объект для отображения до сохранения
                _ad = new Ad { Photos = new List<Photo>() };
            }
            _ad.Photos ??= new List<Photo>();
            _ad.Photos.Add(photo);
            lbPhotos.ItemsSource = _ad.Photos.ToList();
        }

        private void BtnSetMain_Click(object sender, RoutedEventArgs e)
        {
            if (lbPhotos.SelectedItem is Photo sel)
            {
                if (_ad?.Photos == null) return;
                foreach (var p in _ad.Photos) p.IsMain = false;
                sel.IsMain = true;
                lbPhotos.ItemsSource = _ad.Photos.ToList();
            }
        }

        private void BtnRemovePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (lbPhotos.SelectedItem is Photo sel)
            {
                if (_ad?.Photos == null) return;
                _ad.Photos.Remove(sel);
                lbPhotos.ItemsSource = _ad.Photos.ToList();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Заголовок обязателен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_ad == null)
            {
                _ad = new Ad();
                // пользователь, который сейчас вошёл
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Требуется авторизация.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _ad.UserId = App.CurrentUser.UserId;
                _ad.CreatedAt = DateTime.UtcNow;
            }

            _ad.Title = txtTitle.Text.Trim();
            _ad.Description = txtDescription.Text.Trim();
            if (int.TryParse(txtPrice.Text, out int p) && p >= 0) _ad.Price = p;
            else _ad.Price = null;
            _ad.CategoryId = (int)(cbCategory.SelectedValue ?? 0);
            _ad.CityId = (int?)(cbCity.SelectedValue);
            _ad.AdTypeId = (int)(cbType.SelectedValue ?? 0);
            _ad.AdStatusId = (int)(cbStatus.SelectedValue ?? 0);
            _ad.UpdatedAt = DateTime.UtcNow;

            if (_ad.AdId == 0)
            {
                // новый
                // если есть фото с Photo.AdId == 0, EF сам привяжет после добавления
                _db.Ads.Add(_ad);
            }
            else
            {
                // редактирование
                var tracked = _db.Ads.Include(a => a.Photos).FirstOrDefault(a => a.AdId == _ad.AdId);
                if (tracked != null)
                {
                    // обновляем поля
                    tracked.Title = _ad.Title;
                    tracked.Description = _ad.Description;
                    tracked.Price = _ad.Price;
                    tracked.CategoryId = _ad.CategoryId;
                    tracked.CityId = _ad.CityId;
                    tracked.AdTypeId = _ad.AdTypeId;
                    tracked.AdStatusId = _ad.AdStatusId;
                    tracked.UpdatedAt = _ad.UpdatedAt;
                    // синхронизируем фото: удаляем те, что были удалены
                    // простая реализация: удаляем все и вставляем заново
                    _db.Photos.RemoveRange(tracked.Photos ?? Enumerable.Empty<Photo>());
                    if (_ad.Photos != null)
                    {
                        foreach (var ph in _ad.Photos)
                        {
                            ph.AdId = tracked.AdId;
                            _db.Photos.Add(ph);
                        }
                    }
                }
            }

            _db.SaveChanges();
            MessageBox.Show("Сохранено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
