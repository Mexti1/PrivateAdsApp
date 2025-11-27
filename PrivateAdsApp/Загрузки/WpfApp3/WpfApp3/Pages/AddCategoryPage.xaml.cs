using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp3.Data;

namespace WpfApp3.Pages
{
    public partial class AddCategoryPage : Page
    {
        private Category _editingCategory;

        public AddCategoryPage()
        {
            InitializeComponent();
        }

        public AddCategoryPage(Category category) : this()
        {
            _editingCategory = category;
            if (category != null)
            {
                TitleText.Text = "Редактирование категории";
                txtName.Text = category.Name;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new Entities())
                {
                    if (string.IsNullOrEmpty(txtName.Text))
                    {
                        MessageBox.Show("Введите название категории!");
                        return;
                    }

                    if (_editingCategory == null)
                    {
                        if (db.Category.Any(c => c.Name == txtName.Text))
                        {
                            MessageBox.Show("Такая категория уже есть!");
                            return;
                        }
                        var cat = new Category { Name = txtName.Text };
                        db.Category.Add(cat);
                    }
                    else
                    {
                        var c = db.Category.Find(_editingCategory.ID);
                        if (c == null) { MessageBox.Show("Не найдено"); return; }
                        c.Name = txtName.Text;
                    }

                    db.SaveChanges();
                    MessageBox.Show("Сохранено!");
                    NavigationService?.Navigate(new AdminPage());
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminPage());
        }
    }
}
