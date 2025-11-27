using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp3.Data;

namespace WpfApp3.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            Load();
            dateFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dateTo.SelectedDate = DateTime.Now;
        }

        private void Load()
        {
            using (var db = new Entities())
            {
                comboCategoryFilter.ItemsSource = db.Category.ToList();
                comboCategoryFilter.DisplayMemberPath = "Name";
                PaymentGrid.ItemsSource = db.Payment.ToList();
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new Entities())
                {
                    var q = db.Payment.AsQueryable();
                    if (comboCategoryFilter.SelectedItem is Category cat)
                        q = q.Where(p => p.CategoryID == cat.ID);
                    if (dateFrom.SelectedDate.HasValue)
                        q = q.Where(p => p.Date >= dateFrom.SelectedDate.Value);
                    if (dateTo.SelectedDate.HasValue)
                        q = q.Where(p => p.Date <= dateTo.SelectedDate.Value);
                    PaymentGrid.ItemsSource = q.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка фильтра: " + ex.Message);
            }
        }
    }
}
