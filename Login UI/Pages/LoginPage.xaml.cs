using System;
using System.Windows;
using System.Windows.Controls;

namespace Login_UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        //Show another page on login on button click
        private MainWindow mainWindow => Application.Current.MainWindow as MainWindow;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainFrame.Navigate(new Uri("/pages/dashboard.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
