using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPWhatsNew.Views.InkToolbar
{
    public class UnicornPen : InkToolbarCustomPen
    {
        protected override InkDrawingAttributes
            CreateInkDrawingAttributesCore(Brush brush, double strokeWidth)
        {
            InkDrawingAttributes inkDrawingAttributes = new InkDrawingAttributes();
            inkDrawingAttributes.PenTip = PenTipShape.Circle;
            inkDrawingAttributes.Size = new Windows.Foundation.Size(strokeWidth, strokeWidth * 20);
            SolidColorBrush solidColorBrush = brush as SolidColorBrush;
            inkDrawingAttributes.Color = solidColorBrush != null ? solidColorBrush.Color : Colors.DeepPink;

            Matrix3x2 matrix = Matrix3x2.CreateRotation(45);
            inkDrawingAttributes.PenTipTransform = matrix;

            return inkDrawingAttributes;
        }
    }

}
