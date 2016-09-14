using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.RemoteSystems;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWPWhatsNew.Common
{
    public class RemoteSystemStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            RemoteSystemStatus status;
            if (Enum.TryParse(value.ToString(), true, out status))
            {
                switch (status)
                {
                    case RemoteSystemStatus.Unavailable:
                        return new SolidColorBrush(Colors.Red);
                        break;
                    case RemoteSystemStatus.DiscoveringAvailability:
                        return new SolidColorBrush(Colors.Orange);

                        break;
                    case RemoteSystemStatus.Available:
                        return new SolidColorBrush(Colors.ForestGreen);

                        break;
                    case RemoteSystemStatus.Unknown:
                    default:
                        return new SolidColorBrush(Colors.Gray);

                }
            }

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
