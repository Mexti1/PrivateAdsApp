using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp4.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new Djabarov_DBEntities1())
            {
                UsersGrid.ItemsSource = db.Users.ToList();
                CategoriesGrid.ItemsSource = db.Categories.ToList();
                PaymentsGrid.ItemsSource = db.Payments.ToList();
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления категории
            MessageBox.Show("Добавление категории (реализуйте логику)");
            // Пример: NavigationService.Navigate(new AddCategoryPage());
        }

        private void Diagram_Click(object sender, RoutedEventArgs e)
        {
            // Логика перехода к диаграмме
            MessageBox.Show("Переход к диаграмме (реализуйте логику)");
            // Пример: NavigationService.Navigate(new DiagrammPage());
        }

        private void ExportWord_Click(object sender, RoutedEventArgs e)
        {
            // Логика экспорта в Word
            MessageBox.Show("Экспорт в Word (реализуйте логику)");
            // Пример: ExportToWord();
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            // Логика экспорта в Excel
            MessageBox.Show("Экспорт в Excel (реализуйте логику)");
            // Пример: ExportToExcel();
        }

        // (Опционально) Методы экспорта, если они уже реализованы
        private void ExportToWord()
        {
            // Реализация экспорта в Word (из предыдущих ответов)
            MessageBox.Show("Функция экспорта в Word не реализована полностью.");
        }

        private void ExportToExcel()
        {
            // Реализация экспорта в Excel (из предыдущих ответов)
            MessageBox.Show("Функция экспорта в Excel не реализована полностью.");
        }
    }
}