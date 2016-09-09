using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPWhatsNew
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (HaveAnimatedBitmapInAPI())
                this.imageSource.Play();
        }

        private bool HaveAnimatedBitmapInAPI()
        {
            return ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Imaging.BitmapImage", "IsAnimatedBitmap");
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (HaveAnimatedBitmapInAPI())
                this.imageSource.Stop();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (HaveAnimatedBitmapInAPI())
                this.imageSource.AutoPlay = true;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (HaveAnimatedBitmapInAPI())
                this.imageSource.AutoPlay = false;
        }
    }
}
