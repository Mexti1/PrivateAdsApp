using System;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // По старте загружаем страницу авторизации
            MainFrame.Navigate(new Pages.AuthPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (o, t) => DateTimeNow.Text = DateTime.Now.ToString("g");
            timer.Start();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack) MainFrame.GoBack();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}
