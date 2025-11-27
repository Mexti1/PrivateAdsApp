using System.Linq;
using System.Windows;

namespace PrivateAdsApp.Windows
{
    public partial class CompletedAdsWindow : Window
    {
        private readonly AdsDBEntities1 db = new AdsDBEntities1();
        private readonly User currentUser;

        public CompletedAdsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            var completed = db.CompletedAdProfits
                .Where(p => p.Ad.UserId == currentUser.UserId)
                .OrderByDescending(p => p.Ad.PostDate)
                .ToList();

            LvCompleted.ItemsSource = completed;

            decimal total = completed.Sum(p => p.ProfitAmount);
            TxtTotalProfit.Text = total > 0
                ? $"Общая прибыль: {total:N0} руб."
                : "Прибыль ещё не зафиксирована";

            if (completed.Count == 0)
                MessageBox.Show("У вас пока нет завершённых объявлений.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}