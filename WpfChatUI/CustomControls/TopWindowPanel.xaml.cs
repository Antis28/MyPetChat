using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WpfChatUI.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для TopWindowPanel.xaml
    /// </summary>
    public partial class TopWindowPanel : UserControl
    {
        public TopWindowPanel()
        {
            InitializeComponent();
        }
        MainWindow mainWindow { get => Application.Current.MainWindow as MainWindow; }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mainWindow.DragMove();
            }
        }
    }
}
