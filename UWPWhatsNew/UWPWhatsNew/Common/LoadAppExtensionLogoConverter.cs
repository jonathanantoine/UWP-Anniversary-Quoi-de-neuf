using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppExtensions;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPWhatsNew.Common
{
    public class LoadAppExtensionLogoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var logoStreamRef = (value as AppExtension)?.AppInfo.DisplayInfo.GetLogo(new Size(50, 50));
            if (logoStreamRef == null)
            {
                return null;

            }

            var bitmapImage = new BitmapImage();
            LoadLogoAsync(bitmapImage, logoStreamRef);

            return bitmapImage;
        }

        private async Task LoadLogoAsync(BitmapImage bitmapImage, RandomAccessStreamReference logoStreamRef)
        {
            var stream = await logoStreamRef.OpenReadAsync();
            bitmapImage.SetSource(stream);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
