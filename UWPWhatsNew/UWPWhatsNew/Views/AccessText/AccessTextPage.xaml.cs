using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPWhatsNew.Views.AccessText
{

    //Code inspiré des exemples de : https://github.com/Microsoft/WindowsUIDevLabs

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccessTextPage : Page
    {

        #region Private member variables
        ColorBloomTransitionHelper _transition;
        List<Color> _colors;
        int _index;
        readonly Queue<int> _queue = new Queue<int>();
        #endregion

        #region Ctor
        public AccessTextPage()
        {
            InitializeComponent();

            InitializeColors();

            InitializeTransitionHelper();

            Unloaded += ColorBloomTransition_OnUnloaded;
        }

        #endregion


        #region Initializers
        private void InitializeColors()
        {
            _colors = new List<Color>
            {
                Colors.Orange,
                Colors.Lavender,
                Colors.GreenYellow,
                Colors.DeepSkyBlue
            };
        }


        /// <summary>
        /// All of the Color Bloom transition functionality is encapsulated in this handy helper
        /// which we will init once
        /// </summary>
        private void InitializeTransitionHelper()
        {
            // we pass in the UIElement that will host our Visuals
            _transition = new ColorBloomTransitionHelper(hostForVisual);

            // when the transition completes, we need to know so we can update other property values
            _transition.ColorBloomTransitionCompleted += ColorBloomTransitionCompleted;
        }


        #endregion


        #region Event handlers

        /// <summary>
        /// Event handler for the Click event on the header. 
        /// In response this function will trigger a Color Bloom transition animation.
        /// This is achieved by creating a circular solid colored visual directly underneath the
        /// Pivot header which was clicked, and animating its scale so that it floods a designated bounding box. 
        /// </summary>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _index++;
            var currentIndex = _index;

            var targetButton = (Button)sender;

            var headerPosition = targetButton.TransformToVisual(UICanvas).TransformPoint(new Point(0d, 0d));

            var initialBounds = new Rect  // maps to a rectangle the size of the header
            {
                Width = targetButton.RenderSize.Width,
                Height = targetButton.RenderSize.Height,
                X = headerPosition.X + targetButton.RenderSize.Width / 2,
                Y = headerPosition.Y + targetButton.RenderSize.Height / 2
            };

            _queue.Enqueue(currentIndex);
            var finalBounds = Window.Current.Bounds;  // maps to the bounds of the current window
            _transition.Start(_colors[currentIndex % 4], initialBounds, finalBounds);
        }


        /// <summary>
        /// Updates the background of the layout panel to the same color whose transition animation just completed.
        /// </summary>
        private void ColorBloomTransitionCompleted(object sender, EventArgs e)
        {
            UICanvas.Background = new SolidColorBrush(_colors[_queue.Dequeue() % 4]);
        }


        /// <summary>
        /// In response to a XAML layout event on the Grid (named UICanvas) we will apply a clip
        /// to ensure all Visual animations stay within the bounds of the Grid, and doesn't bleed into
        /// the top level Frame belonging to the Sample Gallery. Probably not a factor in most other cases.
        /// </summary>
        private void UICanvas_OnzeChanged(object sender, SizeChangedEventArgs e)
        {
            var uiCanvasLocation = UICanvas.TransformToVisual(UICanvas).TransformPoint(new Point(0d, 0d));
            var clip = new RectangleGeometry
            {
                Rect = new Rect(uiCanvasLocation, e.NewSize)
            };
            UICanvas.Clip = clip;
        }

        /// <summary>
        /// Cleans up remaining surfaces when the page is unloaded.
        /// </summary>
        private void ColorBloomTransition_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _transition.DisposeSurfaces();
        }

        #endregion

    }
}
