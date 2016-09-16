using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPWhatsNew.Views.Partials
{
    public sealed partial class SnowFallUserControl : UserControl
    {
        public SnowFallUserControl()
        {
            this.InitializeComponent();
            Loaded += OnSnowFallUserControlLoaded;
        }

        public SnowFallUserControl(BitmapImage changeImage) : this()
        {
            if (changeImage != null)
            {
                image.Source = changeImage;
            }
        }

        private static readonly Random _Random = new Random((int)DateTime.UtcNow.Ticks);
        private void OnSnowFallUserControlLoaded(object sender, RoutedEventArgs e)
        {
            var left = _Random.Next(-50, (int)(ActualWidth - 100));
            image.Margin = new Thickness(left, 0, 0, 0);
            LetItSnowStoryboard.Completed += OnLetItSnowStoryboardCompleted;
            LetItSnowStoryboard.Begin();
        }

        private void OnLetItSnowStoryboardCompleted(object sender, object e)
        {
            LetItSnowStoryboard.Completed -= OnLetItSnowStoryboardCompleted;
            ((Panel)Parent).Children.Remove(this);
        }
    }
}
