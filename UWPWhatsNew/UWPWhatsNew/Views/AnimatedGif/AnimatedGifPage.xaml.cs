using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPWhatsNew.Views.AnimatedGif
{
    public sealed partial class AnimatedGifPage : Page
    {
        public AnimatedGifPage()
        {
            InitializeComponent();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (HaveAnimatedBitmapInAPI())
                this.ImageSource.Play();
        }

        private bool HaveAnimatedBitmapInAPI()
        {
            return ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Imaging.BitmapImage", "IsAnimatedBitmap");
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (HaveAnimatedBitmapInAPI())
                this.ImageSource.Stop();
        }
    }
}
