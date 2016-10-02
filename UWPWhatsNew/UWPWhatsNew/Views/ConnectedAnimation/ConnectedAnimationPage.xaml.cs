using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using System.Collections.Generic;

namespace UWPWhatsNew.Views.ConnectedAnimation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectedAnimationPage : Page
    {
        private static string _navigatedUri;
        Compositor _compositor;
        ImplicitAnimationCollection _elementImplicitAnimation;

        public ConnectedAnimationViewModel ViewModel { get; set; }
        public ConnectedAnimationPage()
        {
            this.InitializeComponent();
            enableAnimation.IsChecked = ConnectedAnimationData.AnimationIsEnabled;
            ViewModel = DataContext as ConnectedAnimationViewModel;

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            // Create ImplicitAnimations Collection. 
            _elementImplicitAnimation = _compositor.CreateImplicitAnimationCollection();

            //Define trigger and animation that should play when the trigger is triggered. 
            _elementImplicitAnimation["Offset"] = CreateOffsetAnimation();
        }

        private void gridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            ////var elementVisual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
            ////if (args.InRecycleQueue)
            ////{
            ////    elementVisual.ImplicitAnimations = null;
            ////}
            ////else
            ////{
            ////    //Add implicit animation to each visual 
            ////    elementVisual.ImplicitAnimations = _elementImplicitAnimation;
            ////}
        }

        private CompositionAnimationGroup CreateOffsetAnimation()
        {

            //Define Offset Animation for the Animation group
            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            offsetAnimation.Duration = TimeSpan.FromSeconds(.4);

            //Define Animation Target for this animation to animate using definition. 
            offsetAnimation.Target = "Offset";

            //Define Rotation Animation for Animation Group. 
            ScalarKeyFrameAnimation rotationAnimation = _compositor.CreateScalarKeyFrameAnimation();
            rotationAnimation.InsertKeyFrame(.5f, 0.160f);
            rotationAnimation.InsertKeyFrame(1f, 0f);
            rotationAnimation.Duration = TimeSpan.FromSeconds(.4);

            //Define Animation Target for this animation to animate using definition. 
            rotationAnimation.Target = "RotationAngle";

            //Add Animations to Animation group. 
            CompositionAnimationGroup animationGroup = _compositor.CreateAnimationGroup();
            animationGroup.Add(offsetAnimation);
            animationGroup.Add(rotationAnimation);

            return animationGroup;
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
            Transitions = new TransitionCollection
            {
                new ContentThemeTransition()
            };
            _navigatedUri = item.ImageUrl;
            //ConnectedAnimationData.CurrentThumbnail = item;
            Frame.Navigate(typeof(ConnectedAnimationDetail), item);
        }

        private async void ItemsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConnectedAnimationData.AnimationIsEnabled)
            {
                if (_navigatedUri != null)
                {
                    //récupération de l'animation
                    var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
                    if (animation != null)
                    {
                        var item = ViewModel.Items.First(compare => compare.ImageUrl == _navigatedUri);

                        //on scroll vers l'item
                        ItemsGridView.ScrollIntoView(item, ScrollIntoViewAlignment.Default);

                        await Task.Yield();

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
