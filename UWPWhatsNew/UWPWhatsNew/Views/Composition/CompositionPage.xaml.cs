using System;
using System.Collections.Generic;
using System.Numerics;
using UWPWhatsNew.Composition;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
//Code from sample :https://github.com/Microsoft/WindowsUIDevLabs
namespace UWPWhatsNew.Views.Composition
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompositionPage : Page
    {

        public CompositionPage()
        {
            this.InitializeComponent();
        }

        private void ImplicitAnimations_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImplicitAnimationsPage));
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BlurPage));
        }
    }
}
