using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using WpfApp3.Data;

namespace WpfApp3.Pages
{
    public partial class AddPaymentPage : Page
    {
        private Payment _editingPayment;

        public AddPaymentPage()
        {
            InitializeComponent();
            LoadLookups();
            datePicker.SelectedDate = DateTime.Now;
        }

        public AddPaymentPage(Payment payment) : this()
        {
            _editingPayment = payment;
            if (payment != null)
            {
                TitleText.Text = "Редактирование платежа";
                comboCategory.SelectedItem = comboCategory.Items.Cast<Category>().FirstOrDefault(c => c.ID == payment.CategoryID);
                comboUser.SelectedItem = comboUser.Items.Cast<Users>().FirstOrDefault(u => u.ID == payment.UserID);
                datePicker.SelectedDate = payment.Date;
                txtName.Text = payment.Name;
                txtNum.Text = payment.Num.ToString();
                txtPrice.Text = payment.Price.ToString();
            }
        }

        private void LoadLookups()
        {
            using (var db = new Entities())
            {
                comboCategory.ItemsSource = db.Category.ToList();
                comboUser.ItemsSource = db.Users.ToList();
                comboCategory.DisplayMemberPath = "Name";
                comboUser.DisplayMemberPath = "FIO";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new Entities())
                {
                    if (comboCategory.SelectedItem == null || comboUser.SelectedItem == null || string.IsNullOrEmpty(txtName.Text))
                    {
                        MessageBox.Show("Заполните обязательные поля!");
                        return;
                    }

                    int num = 0;
                    decimal price = 0;
                    if (!int.TryParse(txtNum.Text, out num)) { MessageBox.Show("Неверный формат количества"); return; }
                    if (!decimal.TryParse(txtPrice.Text, out price)) { MessageBox.Show("Неверный формат цены"); return; }

                    if (_editingPayment == null)
                    {
                        var pay = new Payment
                        {
                            CategoryID = ((Category)comboCategory.SelectedItem).ID,
                            UserID = ((Users)comboUser.SelectedItem).ID,
                            Date = datePicker.SelectedDate ?? DateTime.Now,
                            Name = txtName.Text,
                            Num = num,
                            Price = price
                        };
                        db.Payment.Add(pay);
                    }
                    else
                    {
                        var p = db.Payment.Find(_editingPayment.ID);
                        if (p == null) { MessageBox.Show("Не найден платеж в БД"); return; }
                        p.CategoryID = ((Category)comboCategory.SelectedItem).ID;
                        p.UserID = ((Users)comboUser.SelectedItem).ID;
                        p.Date = datePicker.SelectedDate ?? DateTime.Now;
                        p.Name = txtName.Text;
                        p.Num = num;
                        p.Price = price;
                    }

                    db.SaveChanges();
                    MessageBox.Show("Сохранено!");
                    NavigationService?.Navigate(new AdminPage());
                }
            }
            catch (Exception ex)
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
