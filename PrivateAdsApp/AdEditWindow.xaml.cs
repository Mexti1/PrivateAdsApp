using System;
using System.Linq;
using System.Windows;
using Microsoft.VisualBasic;

namespace PrivateAdsApp.Windows
{
    public partial class AdEditWindow : Window
    {
        private AdsDBEntities1 db = new AdsDBEntities1();
        private User currentUser;
        private Ad currentAd; // null — если новое, иначе — редактируемое

        public AdEditWindow(User user, Ad ad = null)
        {
            InitializeComponent();
            currentUser = user;
            currentAd = ad ?? new Ad();

            LoadCombos();
            LoadData();
        }

        private void LoadCombos()
        {
            CmbCity.ItemsSource = db.Cities.ToList();
            CmbCategory.ItemsSource = db.Categories.ToList();
            CmbType.ItemsSource = db.AdTypes.ToList();
            CmbStatus.ItemsSource = db.AdStatuses.ToList();
        }

        private void LoadData()
        {
            if (currentAd.AdId != 0)
            {
                // Редактирование — подгружаем данные
                TxtTitle.Text = currentAd.Title;
                TxtDescription.Text = currentAd.Description;
                TxtPrice.Text = currentAd.Price.ToString();
                CmbCity.SelectedItem = currentAd.City;
                CmbCategory.SelectedItem = currentAd.Category;
                CmbType.SelectedItem = currentAd.AdType;
                CmbStatus.SelectedItem = currentAd.AdStatus;
            }
            else
            {
                // Новое объявление
                CmbStatus.SelectedIndex = 0; // Активно
                currentAd.PostDate = DateTime.Today;
                currentAd.UserId = currentUser.UserId;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Введите заголовок!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbCity.SelectedItem == null || CmbCategory.SelectedItem == null ||
                CmbType.SelectedItem == null || CmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите все значения из списков!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Заполняем объект
            currentAd.Title = TxtTitle.Text.Trim();
            currentAd.Description = TxtDescription.Text.Trim();
            currentAd.Price = price;
            currentAd.CityId = (CmbCity.SelectedItem as City).CityId;
            currentAd.CategoryId = (CmbCategory.SelectedItem as Category).CategoryId;
            currentAd.AdTypeId = (CmbType.SelectedItem as AdType).AdTypeId;
            currentAd.StatusId = (CmbStatus.SelectedItem as AdStatus).StatusId;

            bool wasActive = currentAd.AdStatus?.StatusName == "Активно";
            bool becomingCompleted = (CmbStatus.SelectedItem as AdStatus).StatusName == "Завершено";

            // === КЛЮЧЕВОЙ МОМЕНТ: ВВОД ПРИБЫЛИ ПРИ ЗАВЕРШЕНИИ ===
            if (becomingCompleted && wasActive)
            {
                string input = Interaction.InputBox(
                    "Объявление завершается.\nВведите полученную сумму (руб.):",
                    "Фиксация прибыли",
                    "0");

                if (!decimal.TryParse(input, out decimal profit) || profit < 0)
                {
                    MessageBox.Show("Сумма должна быть неотрицательным числом!", "Ошибка ввода", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Сохраняем прибыль
                var profitRecord = new CompletedAdProfit
                {
                    AdId = currentAd.AdId == 0 ? 0 : currentAd.AdId, // временно
                    ProfitAmount = profit
                };

                // Если новое объявление — сначала сохраним его, потом привяжем прибыль
                if (currentAd.AdId == 0)
                {
                    db.Ads.Add(currentAd);
                    db.SaveChanges(); // получаем AdId
                    profitRecord.AdId = currentAd.AdId;
                    db.CompletedAdProfits.Add(profitRecord);
                }
                else
                {
                    profitRecord.AdId = currentAd.AdId;
                    db.CompletedAdProfits.Add(profitRecord);
                }
            }

            // Сохранение объявления
            if (currentAd.AdId == 0)
                db.Ads.Add(currentAd);

            try
            {
                db.SaveChanges();
                MessageBox.Show("Объявление успешно сохранено!", "Успех", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}