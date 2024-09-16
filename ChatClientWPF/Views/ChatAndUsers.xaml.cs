using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChatClientWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для ChatAndUsers.xaml
    /// </summary>
    public partial class ChatAndUsers : UserControl
    {
        public ChatAndUsers()
        {
            InitializeComponent();
        }

        private void lbUsers_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //lbUsers.SelectedIndex = 1;
        }

        private void lbUsers_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (lbUsers.Items.Count <= 0) { return; }

            lbUsers.ScrollIntoView(lbUsers.Items[lbUsers.Items.Count - 1]);

        }


    }
}
