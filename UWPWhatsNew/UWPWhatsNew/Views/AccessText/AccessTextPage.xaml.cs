using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

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
        List<Windows.UI.Color> _colors;
        int _index;
        Queue<int> _queue = new Queue<int>();
        #endregion

        #region Ctor
        public AccessTextPage()
        {
            this.InitializeComponent();

            this.InitializeColors();

            this.InitializeTransitionHelper();

            this.Unloaded += ColorBloomTransition_Unloaded;
        }

        #endregion


        #region Initializers
        private void InitializeColors()
        {
            _colors = new List<Windows.UI.Color>();
            _colors.Add(Windows.UI.Colors.Orange);
            _colors.Add(Windows.UI.Colors.Lavender);
            _colors.Add(Windows.UI.Colors.GreenYellow);
            _colors.Add(Windows.UI.Colors.DeepSkyBlue);
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
        private void Header_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _index++;
            var currentIndex = _index;

            var header = sender as Button;

            var headerPosition = header.TransformToVisual(UICanvas).TransformPoint(new Windows.Foundation.Point(0d, 0d));

            var initialBounds = new Windows.Foundation.Rect()  // maps to a rectangle the size of the header
            {
                Width = header.RenderSize.Width,
                Height = header.RenderSize.Height,
                X = headerPosition.X,
                Y = headerPosition.Y
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
        private void UICanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var uiCanvasLocation = UICanvas.TransformToVisual(UICanvas).TransformPoint(new Windows.Foundation.Point(0d, 0d));
            var clip = new RectangleGeometry()
            {
                Rect = new Windows.Foundation.Rect(uiCanvasLocation, e.NewSize)
            };
            UICanvas.Clip = clip;
        }

        /// <summary>
        /// Cleans up remaining surfaces when the page is unloaded.
        /// </summary>
        private void ColorBloomTransition_Unloaded(object sender, RoutedEventArgs e)
        {
            _transition.DisposeSurfaces();
        }

        #endregion

    }
}
