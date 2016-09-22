using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace UWPWhatsNew.Views.ConnectedAnimation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectedAnimationDetail : Page
    {
        private Thumbnail _thumbnail;
        private SystemNavigationManager _systemNavigationManager = SystemNavigationManager.GetForCurrentView();

        public ConnectedAnimationDetail()
        {
            InitializeComponent();
            _image.ImageOpened += _image_ImageOpened;
            _systemNavigationManager.BackRequested += ConnectedAnimationDetail_BackRequested;
            _systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void _image_ImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ConnectedAnimationData.AnimationIsEnabled)
            {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
                if (animation != null)
                {
                    _image.Opacity = 1;
                    animation.TryStart(_image);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _image.Opacity = 0;
            _thumbnail = e.Parameter as Thumbnail;
            if (_thumbnail != null)
            {
                _image.Source = new BitmapImage(new Uri((string)_thumbnail.ImageUrl));
            }
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
            if (_image.Parent != null && ConnectedAnimationData.AnimationIsEnabled)
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", _image);
            }

            e.Handled = true;
            Frame.GoBack();
        }
    }
}
