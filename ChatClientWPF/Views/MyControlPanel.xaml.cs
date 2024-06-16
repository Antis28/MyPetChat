using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для MyControlPanel.xaml
    /// </summary>
    public partial class MyControlPanel : UserControl
    {
        public MyControlPanel()
        {
            InitializeComponent();
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if the input is a numeric value (you can add support for decimal separator if needed)
            if (!IsTextNumeric(e.Text))
            {
                e.Handled = true; // Mark the event as handled to prevent the character from being entered
            }
        }

        private bool IsTextNumeric(string text)
        {
            // Modify the regex pattern to support decimal separator or negative numbers if needed
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextNumeric(text))
                {
                    e.CancelCommand(); // Cancel the paste command if the text is not numeric
                }
            }
            else
            {
                e.CancelCommand(); // Cancel the paste command if the data is not a string
            }
        }
    }
}
