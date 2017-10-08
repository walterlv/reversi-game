using Windows.UI.Xaml.Controls;

namespace Walterlv.Gaming.Reversi
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            ContentFrame.Navigate(typeof(MenuPage));
        }
    }
}
