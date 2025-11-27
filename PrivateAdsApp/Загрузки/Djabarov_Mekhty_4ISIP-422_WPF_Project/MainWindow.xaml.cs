using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Djabarov_Mekhty_4ISIP_422
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Pages.AuthPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (o, t) => { DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); };
            timer.Start();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack) MainFrame.GoBack();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}
