using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp4.Pages;

namespace WpfApp4.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new Djabarov_DBEntities1())
            {
                PaymentsGrid.ItemsSource = db.Payments.Where(p => p.UserID == AuthPage.currentUser.ID).ToList();
            }
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            // Откройте модальное окно или страницу для добавления платежа
            // Пример: NavigationService.Navigate(new AddPaymentPage());
        }
    }
}