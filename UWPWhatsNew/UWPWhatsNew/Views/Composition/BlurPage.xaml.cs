using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPWhatsNew.Views.Composition
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlurPage : Page
    {
        #region Fields
        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private int _currentImageIndex = 1;
        #endregion

        public BlurPage()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IList<ComboBoxItem> blendList = new List<ComboBoxItem>();
            foreach (BlendEffectMode type in Enum.GetValues(typeof(BlendEffectMode)))
            {
                // Exclude unsupported types
                if (type != BlendEffectMode.Dissolve &&
                    type != BlendEffectMode.Saturation &&
                    type != BlendEffectMode.Color &&
                    type != BlendEffectMode.Hue &&
                    type != BlendEffectMode.Luminosity
                    )
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Tag = type;
                    item.Content = type.ToString();
                    blendList.Add(item);
                }
            }

            BlendSelection.ItemsSource = blendList;
            BlendSelection.SelectedIndex = 0;
        }

        private void BlendSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = BlendSelection.SelectedValue as ComboBoxItem;
            BlendEffectMode blendmode = (BlendEffectMode)item.Tag;

            // Create a chained effect graph using a BlendEffect, blending color and blur
            var graphicsEffect = new BlendEffect
            {
                Mode = blendmode,
                Background = new ColorSourceEffect()
                {
                    Name = "Tint",
                    Color = Colors.Pink,
                },
                Foreground = new GaussianBlurEffect()
                {
                    Name = "Blur",
                    Source = new CompositionEffectSourceParameter("Backdrop"),
                    BlurAmount = 0,
                    BorderMode = EffectBorderMode.Hard,
                }
            };

            var blurEffectFactory = _compositor.CreateEffectFactory(graphicsEffect, new[] { "Blur.BlurAmount", "Tint.Color" });

            // Create EffectBrush, BackdropBrush and SpriteVisual
            _brush = blurEffectFactory.CreateBrush();

            // If the animation is running, restart it on the new brush
            var destinationBrush = _compositor.CreateBackdropBrush();
            _brush.SetSourceParameter("Backdrop", destinationBrush);

            var blurSprite = _compositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2((float)BackgroundImage.ActualWidth, (float)BackgroundImage.ActualHeight);
            blurSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(BackgroundImage, blurSprite);
        }

        private void BackgroundImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SpriteVisual blurVisual = (SpriteVisual)ElementCompositionPreview.GetElementChildVisual(BackgroundImage);

            if (blurVisual != null)
            {
                blurVisual.Size = e.NewSize.ToVector2();
            }

        }

        private void StartBlurAnimation()
        {
            ScalarKeyFrameAnimation blurAnimation = _compositor.CreateScalarKeyFrameAnimation();
            blurAnimation.InsertKeyFrame(0.0f, 0.0f);
            blurAnimation.InsertKeyFrame(0.5f, 100.0f);
            blurAnimation.InsertKeyFrame(1.0f, 0.0f);
            blurAnimation.Duration = TimeSpan.FromSeconds(4);
            blurAnimation.IterationBehavior = AnimationIterationBehavior.Count;
            blurAnimation.IterationCount = 1;
            _brush.StartAnimation("Blur.BlurAmount", blurAnimation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartBlurAnimation();
        }

        private void SwitchImage_Click(object sender, RoutedEventArgs e)
        {
            _currentImageIndex++;
            if (_currentImageIndex > 5)
                _currentImageIndex = 1;
            var endImage = string.Format("sw{0}.jpg", _currentImageIndex);
            var image = new BitmapImage(new Uri("ms-appx:///Assets/Images/Composition/Blur/" + endImage));
            BackgroundImage.Source = image;
        }
    }
}
