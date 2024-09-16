using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatUI.Custom_Controls
{
    /// <summary>
    /// Interaction logic for RoundProfileButton.xaml
    /// </summary>
    public partial class RoundProfileButton : UserControl
    {
        public RoundProfileButton()
        {
            InitializeComponent();
        }

        public SolidColorBrush StrokeBrush
        {
            get => (SolidColorBrush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeBrushProperty =
            DependencyProperty.Register("StrokeBrush", typeof(SolidColorBrush), typeof(RoundProfileButton));



        public bool IsOnline
        {
            get => (bool)GetValue(IsOnlineProperty);
            set => SetValue(IsOnlineProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register("IsOnline", typeof(bool), typeof(RoundProfileButton));

        public ImageSource ProfileImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ProfileImageSource", typeof(ImageSource), typeof(RoundProfileButton));
    }
}
