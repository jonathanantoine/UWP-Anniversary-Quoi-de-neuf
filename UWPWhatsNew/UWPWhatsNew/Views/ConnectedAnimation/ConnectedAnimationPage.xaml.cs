using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

namespace UWPWhatsNew.Views.ConnectedAnimation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectedAnimationPage : Page
    {
        private static string _navigatedUri;

        public ConnectedAnimationViewModel ViewModel { get; set; }
        public ConnectedAnimationPage()
        {
            this.InitializeComponent();
            enableAnimation.IsChecked = ConnectedAnimationData.AnimationIsEnabled;
            ViewModel = DataContext as ConnectedAnimationViewModel;

        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Don't use vertical entrance animation with connected animation
            if (e.NavigationMode == NavigationMode.Back)
            {
                EntranceTransition.FromVerticalOffset = 0;
            }

            //Hide the back button on the list page as there is no where to go back to. 
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void ItemsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ConnectedAnimationData.AnimationIsEnabled = enableAnimation.IsChecked.GetValueOrDefault();
            // On vérifie si on utilise la connnected animation
            if (ConnectedAnimationData.AnimationIsEnabled)
            {
                var container = ItemsGridView.ContainerFromItem(e.ClickedItem) as GridViewItem;
                if (container != null)
                {
                    var root = (FrameworkElement)container.ContentTemplateRoot;
                    var image = (UIElement)root.FindName("ImageItem");

                    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", image);
                }
            }

            var item = (Thumbnail)e.ClickedItem;

            // Add a fade out effect
            Transitions = new TransitionCollection();
            Transitions.Add(new ContentThemeTransition());
            _navigatedUri = item.ImageUrl;
            Frame.Navigate(typeof(ConnectedAnimationDetail), _navigatedUri);
        }

        private void ItemsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConnectedAnimationData.AnimationIsEnabled)
            {
                if (_navigatedUri != null)
                {
                    //récupération de l'animation
                    var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
                    if (animation != null)
                    {
                        var item = ViewModel.Items.Where(compare => compare.ImageUrl == _navigatedUri).First();

                        //on scroll vers l'item
                        ItemsGridView.ScrollIntoView(item, ScrollIntoViewAlignment.Default);
                        ItemsGridView.UpdateLayout();

                        var container = ItemsGridView.ContainerFromItem(item) as GridViewItem;
                        if (container != null)
                        {
                            var root = (FrameworkElement)container.ContentTemplateRoot;
                            var image = (Image)root.FindName("ImageItem");
                            image.Opacity = 1;
                            //on lance l'animation
                            animation.TryStart(image);
                        }
                        else
                        {
                            animation.Cancel();
                        }
                    }

                    _navigatedUri = null;
                }
            }
        }

    }
}
