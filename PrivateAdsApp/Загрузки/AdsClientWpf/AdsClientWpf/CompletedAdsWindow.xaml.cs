using System.Windows;
using AdsClientWpf.Data;
using Microsoft.EntityFrameworkCore;

namespace AdsClientWpf
{
    public partial class CompletedAdsWindow : Window
    {
        private readonly AdsDbContext _db;
        public CompletedAdsWindow()
        {
            InitializeComponent();
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseSqlServer("Server=.;Database=AdsDB;Trusted_Connection=True;")
                .Options;
            _db = new AdsDbContext(options);
            LoadCompleted();
        }

        private void LoadCompleted()
        {
            var q = _db.Sales.Include(s => s.Ad).OrderByDescending(s => s.SaleDate).ToList();
            lvCompleted.ItemsSource = q.Select(s => new { s.AdId, Title = s.Ad?.Title, s.SaleAmount, SaleDate = s.SaleDate.ToLocalTime().ToString("g") }).ToList();
            tbTotal.Text = q.Sum(s => s.SaleAmount).ToString() + " ₽";
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCompleted();
        }
    }
}
