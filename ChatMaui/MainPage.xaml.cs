using ChatMaui.ViewModels;

namespace ChatMaui
{
    public partial class MainPage : ContentPage
    {
        
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
