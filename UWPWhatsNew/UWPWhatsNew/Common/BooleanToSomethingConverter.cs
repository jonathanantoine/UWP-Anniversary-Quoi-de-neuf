using System;
using Windows.UI.Xaml.Data;

namespace UWPWhatsNew.Common
{
    public class BooleanToSomethingConverter : IValueConverter
    {
        public object IfTrue { get; set; }
        public object IfFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var casted = System.Convert.ToBoolean(value);
            var inveted = System.Convert.ToBoolean(parameter);
            if (inveted) casted = !casted;

            return casted ? IfTrue : IfFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
