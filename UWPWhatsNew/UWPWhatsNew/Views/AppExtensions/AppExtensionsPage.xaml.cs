using Windows.ApplicationModel.AppExtensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPWhatsNew.Views.AppExtensions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppExtensionsPage : Page
    {
        public AppExtensionsViewModel ViewModel { get; set; }

        public AppExtensionsPage()
        {
            InitializeComponent();
            ViewModel = DataContext as AppExtensionsViewModel;
        }

        private void OnAppExtension(object sender, ItemClickEventArgs e)
        {
            ViewModel.LaunchExtensionAsync(e.ClickedItem as AppExtension);
        }

        private void OnRemoveExtensionClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            ViewModel.AskRemoveExtensionAsync(((FrameworkElement) sender).DataContext as AppExtension);
        }
    }
}
