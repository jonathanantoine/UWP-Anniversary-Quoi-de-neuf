using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPWhatsNew.Views.ConnectedAnimation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectedAnimationDetail : Page
    {
        SystemNavigationManager _systemNavigationManager = SystemNavigationManager.GetForCurrentView();

        public ConnectedAnimationDetail()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            _image.Source = new BitmapImage(new Uri((string)e.Parameter));

            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
            if (animation != null)
            {
                _image.Opacity = 0;
                _image.ImageOpened += (sender_, e_) =>
                {
                    _image.Opacity = 1;
                    animation.TryStart(_image);
                };
            }

            _systemNavigationManager.BackRequested += ConnectedAnimationDetail_BackRequested;
            _systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _systemNavigationManager.BackRequested -= ConnectedAnimationDetail_BackRequested;
            _systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void ConnectedAnimationDetail_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            var img = _image;
            
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", img);

            e.Handled = true;
            Frame.GoBack();
        }
    }
}
