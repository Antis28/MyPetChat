using System.Windows;
using System.Windows.Controls;

namespace Login_UI.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для TextBoxWithPlaceHolder.xaml
    /// </summary>
    public partial class TextBoxWithPlaceHolder : UserControl
    {
        public TextBoxWithPlaceHolder()
        {
            InitializeComponent();
        }


        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }

        // Using a DependencyProperty as the backing store for PlaceHolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(TextBoxWithPlaceHolder));



        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBoxWithPlaceHolder));



        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsPassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPasswordProperty =
            DependencyProperty.Register("IsPassword", typeof(bool), typeof(TextBoxWithPlaceHolder));

        private void passbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            email.Text = passbox.Password;
        }
    }
}
