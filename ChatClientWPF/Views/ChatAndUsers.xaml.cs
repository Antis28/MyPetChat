using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
