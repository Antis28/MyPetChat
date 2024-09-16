using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClientWPF.Handlers
{
    internal  class FileDialogs
    {
        public static string Open()
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = ""; // Default file name
            dialog.DefaultExt = "*.*"; // Default file extension
            dialog.Filter = "Все файлы|*.*"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                return dialog.FileName;
            }
            return "";
        }

        public static string Save()
        {
            // Configure save file dialog box
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = ""; // Default file name
            //dialog.DefaultExt = "*.*"; // Default file extension
            dialog.Filter = "Все файлы|*.*"; // Filter files by extension
            //dialog.AddExtension = false;
            //dialog.CheckPathExists = false;
            // Show save file dialog box
            bool? result = dialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                return dialog.FileName;
            }
            return "";
        }

        public static void msg()
        {
            string messageBoxText = "Хотите передать файл?";//"Хотите сохранить файл?";
            string caption = "Чат";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            switch (result)
            {
                case MessageBoxResult.Cancel:
                    // User pressed Cancel
                    break;
                case MessageBoxResult.Yes:
                    Open();
                    // User pressed Yes
                    break;
                case MessageBoxResult.No:
                    // User pressed No
                    break;
                default: throw new ArgumentException("msg не опознан switch");
            }
        }
    }
}
