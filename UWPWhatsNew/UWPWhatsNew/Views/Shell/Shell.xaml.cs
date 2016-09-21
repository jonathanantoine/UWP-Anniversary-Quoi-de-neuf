using System;
using UWPWhatsNew.Views;
using UWPWhatsNew.Views.Home;
using UWPWhatsNew.Views.Shell;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace UWPWhatsNew
{
    public sealed partial class Shell : Page
    {
        public Shell()
        {
            this.InitializeComponent();
            ViewModel = DataContext as ShellViewModel;
            NavigationFrame.Navigate(typeof(HomePage));
        }

        public ShellViewModel ViewModel { get; private set; }

        private void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = (e.ClickedItem as AvailableItem<System.Type>);
            //Title.Text = "FOCUS SUR : " + clicked.Label;
            NavigationFrame.Navigate(clicked.Value);
        }
    }
}
