using System;
using System.Collections.Generic;
using System.IO;
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

namespace ChatClientWPF.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для FileDetail.xaml
    /// </summary>
    public partial class FileDetail : UserControl
    {
        // To convert bytes to Mb => bytes /  1.049e+6
        
        public FileDetail()
        {
            double RatioBytesOnMb = 1.049e+6;
            InitializeComponent();
        }
        
        #region FileName : string - File name
        ///<summary>File name</summary>
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register(nameof(FileName), typeof(string), typeof(FileDetail));
        #endregion
        
        #region FileSize : string - File size
        ///<summary>File size</summary>
        public string FileSize
        {
            get => (string)GetValue(FileSizeProperty);
            set => SetValue(FileSizeProperty, value);
        }
        // Using a DependencyProperty as the backing store for FileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileSizeProperty =
            DependencyProperty.Register(nameof(FileSize), typeof(string), typeof(FileDetail));
        #endregion
        
        #region UploadProgress : int - Upload Progress
        ///<summary>Upload Progress</summary>
        public int UploadProgress
        {
            get => (int)GetValue(UploadProgressProperty);
            set => SetValue(UploadProgressProperty, value);
        }
        // Using a DependencyProperty as the backing store for UploadProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UploadProgressProperty =
            DependencyProperty.Register(nameof(UploadProgress), typeof(int), typeof(FileDetail));
        #endregion
        
        #region UploadSpeed : int - Upload speed
        ///<summary>Upload speed</summary>
        public int UploadSpeed
        {
            get => (int)GetValue(UploadSpeedProperty);
            set => SetValue(UploadSpeedProperty, value);
        }
        // Using a DependencyProperty as the backing store for UploadSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UploadSpeedProperty =
            DependencyProperty.Register(nameof(UploadSpeed), typeof(int), typeof(FileDetail));
        #endregion








    }
}
