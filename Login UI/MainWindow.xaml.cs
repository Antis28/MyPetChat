using System;
using System.Windows;

namespace Login_UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new Uri("/pages/loginpage.xaml", UriKind.RelativeOrAbsolute));
        }

        private MainWindow mainWindow => Application.Current.MainWindow as MainWindow;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainWindow.Title = $"Размер окна: {mainWindow.Height}x{mainWindow.Width}";
        }
    }
}
