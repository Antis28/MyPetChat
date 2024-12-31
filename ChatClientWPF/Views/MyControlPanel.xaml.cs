using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatClientWPF.CustomControls;
using Microsoft.Win32;

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

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = System.IO.Path.GetFileName(files[0]);
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };
            bool? responce = openFileDialog.ShowDialog();
            if (responce == false) return;

            //Get selected files
            string[] files = openFileDialog.FileNames;

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = System.IO.Path.GetFileName(files[i]);
                FileInfo fileInfo = new FileInfo(files[i]);

            }
        }
    }
}
