using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PrivateAdsApp.Windows
{
    public partial class MyAdsWindow : Window
    {
        private AdsDBEntities1 db = new AdsDBEntities1();
        private User currentUser;

        public MyAdsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadMyAds();
        }

        private void LoadMyAds()
        {
            var myAds = db.Ads
                .Where(a => a.UserId == currentUser.UserId)
                .OrderByDescending(a => a.PostDate)
                .ToList();

            LvMyAds.ItemsSource = myAds;
        }

        private void LvMyAds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = LvMyAds.SelectedItem != null;
            BtnEdit.IsEnabled = hasSelection;
            BtnDelete.IsEnabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWin = new AdEditWindow(currentUser);
            if (editWin.ShowDialog() == true)
            {
                LoadMyAds();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LvMyAds.SelectedItem is Ad selectedAd)
            {
                var editWin = new AdEditWindow(currentUser, selectedAd);
                if (editWin.ShowDialog() == true)
                {
                    LoadMyAds();
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LvMyAds.SelectedItem is Ad selectedAd)
            {
                var result = MessageBox.Show(
                    $"Удалить объявление «{selectedAd.Title}»?\nЭто действие нельзя отменить.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    db.Ads.Remove(selectedAd);
                    db.SaveChanges();
                    LoadMyAds();
                    MessageBox.Show("Объявление удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnCompleted_Click(object sender, RoutedEventArgs e)
        {
            var completedWin = new CompletedAdsWindow(currentUser);
            completedWin.ShowDialog();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}