﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private MainWindow mainWindow => Application.Current.MainWindow as MainWindow;
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mainWindow.DragMove();

            }
        }




        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for BackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(TopWindowPanel));





        public string TitleWindow
        {
            get => (string)GetValue(TitleWindowProperty);
            set => SetValue(TitleWindowProperty, value);
        }

        // Using a DependencyProperty as the backing store for TitleWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleWindowProperty =
            DependencyProperty.Register("TitleWindow", typeof(string), typeof(TopWindowPanel));



        public Brush TitleColor
        {
            get => (Brush)GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for TitleColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleColorProperty =
            DependencyProperty.Register("TitleColor", typeof(Brush), typeof(TopWindowPanel));

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            MaximizeOrMinimize();
        }

        private void MaximizeOrMinimize()
        {
            mainWindow.WindowState = mainWindow.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void _this_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaximizeOrMinimize();
        }
    }
}
