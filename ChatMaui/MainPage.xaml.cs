using ChatMaui.ViewModels;

namespace ChatMaui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
