using System;
using System.Collections.Generic;
using System.Numerics;
using UWPWhatsNew.Composition;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
//Code from sample :https://github.com/Microsoft/WindowsUIDevLabs
namespace UWPWhatsNew.Views.Composition
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompositionPage : Page
    {
        #region Fields
        private Compositor _compositor;
        private ContainerVisual _root;
        //Helper
        private readonly Random randomBrush = new Random();
        #endregion

        #region Constantes
        // Constants
        private const float _posX = 600;
        private const float _posY = 400;
        private const float _circleRadius = 300;
        private const float _ellipseRadiusX = 400;
        private const float _ellipseRadiusY = 200;
        private const double _spiralOrientation = 5;
        private const double _spiralTightness = 0.8;
        private const int _distance = 60;
        private const int _rowCount = 13;
        private const int _columnCount = 13;
        #endregion

        public CompositionPage()
        {
            this.InitializeComponent();
            InitializeComposition();
        }


        /// <summary>
        /// Initialize Composition
        /// </summary>
        private void InitializeComposition()
        {

            // Retrieve an instance of the Compositor from the backing Visual of the Page
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            // Create a root visual from the Compositor
            _root = _compositor.CreateContainerVisual();

            // Set the root ContainerVisual as the XAML Page Visual           
            ElementCompositionPreview.SetElementChildVisual(this, _root);

            // Assign initial values to variables used to store updated offsets for the visuals          
            float posXUpdated = _posX;
            float posYUpdated = _posY;


            //Create a list of image brushes that can be applied to a visual
            string[] imageNames = {
                "01.png",
                "02.png",
                "03.png",
                "04.png",
                "05.png",
                "06.png",
                "07.png",
                "08.png",
                "60Banana.png",
                "60Lemon.png",
                "60Vanilla.png",
                "60Mint.png",
                "60Orange.png",
                "110Strawberry.png",
                "60SprinklesRainbow.png"
             };
            List<CompositionSurfaceBrush> imageBrushList = new List<CompositionSurfaceBrush>();
            IImageLoader imageFactory = ImageLoaderFactory.CreateImageLoader(_compositor);
            for (int k = 0; k < imageNames.Length; k++)
            {
                var surface = imageFactory.LoadImageFromUri(new Uri("ms-appx:///Assets/Images/Composition/ImplicitAnimation/" + imageNames[k]));
                imageBrushList.Add(_compositor.CreateSurfaceBrush(surface));
            }

            // Create nxn matrix of visuals where n=row/ColumnCount-1 and passes random image brush to the function
            // that creates a visual
            for (int i = 1; i < _rowCount; i++)
            {
                posXUpdated = i * _distance;
                for (int j = 1; j < _columnCount; j++)
                {
                    CompositionSurfaceBrush brush = imageBrushList[randomBrush.Next(imageBrushList.Count)];

                    posYUpdated = j * _distance;
                    _root.Children.InsertAtTop(CreateChildElement(brush, posXUpdated, posYUpdated));
                }
            }
            EnableAnimationOnChildren(EnableAnimations.IsChecked.GetValueOrDefault());
        }

        #region Activation des animatiions
        private void EnableAnimations_Checked(object sender, RoutedEventArgs e)
        {
            if (_compositor != null)
            {
                EnableAnimationOnChildren(true);
            }
        }

        private void EnableAnimations_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_compositor != null)
            {
                EnableAnimationOnChildren(false);
            }
        }
        #endregion

        /// <summary>
        /// Creates a visible element in our application
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <returns> </returns>
        Visual CreateChildElement(CompositionSurfaceBrush brush, float positionX, float positionY)
        {

            // Each element consists of a single Sprite visual
            SpriteVisual visual = _compositor.CreateSpriteVisual();

            // Create a SpriteVisual with size, offset and center point
            visual.Size = new Vector2(50, 50);
            visual.Offset = new Vector3(positionX, positionY, 0);
            visual.CenterPoint = new Vector3(visual.Size.X / 2.0f, visual.Size.Y / 2.0f, 0.0f);

            //apply the random image brush to a visual
            visual.Brush = brush;

            return visual;
        }


        private void EnableAnimationOnChildren(bool animationIsEnable)
        {
            ImplicitAnimationCollection implicitAnimationCollection = null;
            if (animationIsEnable)
            {
                implicitAnimationCollection = _compositor.CreateImplicitAnimationCollection();
                implicitAnimationCollection["Offset"] = CreateOffsetAnimation();

            }
            foreach (var child in _root.Children)
            {
                child.ImplicitAnimations = implicitAnimationCollection;
            }
        }

        /// <summary>
        /// Creates offset animation that can be applied to a visual
        /// </summary>
        Vector3KeyFrameAnimation CreateOffsetAnimation()
        {
            var _offsetKeyFrameAnimation = _compositor.CreateVector3KeyFrameAnimation();
            _offsetKeyFrameAnimation.Target = "Offset";

            // Final Value signifies the target value to which the visual will animate
            // in this case it will be defined by new offset
            _offsetKeyFrameAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            _offsetKeyFrameAnimation.Duration = TimeSpan.FromSeconds(3);

            return _offsetKeyFrameAnimation;
        }

        /// <summary>
        ///  This method implicitly animates the visual elements into a Grid layout
        ///  on offset change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridLayout(object sender, RoutedEventArgs e)
        {
            //get the position of the initial grid element
            float posXgrid = _posX;
            float posYgrid = _posY;

            List<Vector3> newOffset = new List<Vector3>();

            // Calculate the position for each visual in the grid layout
            for (int i = 1; i < _rowCount; i++)
            {
                posXgrid = i * _distance;
                for (int j = 1; j < _columnCount; j++)
                {
                    posYgrid = j * _distance;
                    newOffset.Add(new Vector3(posXgrid, posYgrid, 0));
                }
            }
            //counter for adding elements to the grid
            int k = 0;

            foreach (var child in _root.Children)
            {
                child.Offset = newOffset[k];
                k++;

            }
        }

        /// <summary>
        ///  This method implicitly animates the visual elements into a circle layout
        ///  on offset change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CircleLayout(object sender, RoutedEventArgs e)
        {
            //
            // Define initial angle of each element on the spiral
            //
            double theta = 0;
            double thetaRadians = 0;

            foreach (var child in _root.Children)
            {
                // Change the Offset property of the visual. This will trigger the implicit animation that is associated with the Offset change.
                // The position of the element on the circle is defined using parametric equation:
                child.Offset = new Vector3((float)(_circleRadius * Math.Cos(thetaRadians)) + _posX, (float)(_circleRadius * Math.Sin(thetaRadians) + _posY), 0);
                // Update the angle to be used for the next visual element
                theta += 2.5;
                thetaRadians = theta * Math.PI / 180F;
            }
        }


        /// <summary>
        /// This method implicitly animates the visual elements into a spiral layout
        /// on offset change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpiralLayout(object sender, RoutedEventArgs e)
        {

            // Define initial angle of each element on the spiral
            double theta = 0;
            double thetaOrientationRadians = _spiralOrientation * Math.PI / 180F;

            foreach (var child in _root.Children)
            {
                // Change the Offset property of the visual. This will trigger the implicit animation that is associated with the Offset change.
                // Define the position of the visual on the spiral using parametric equation:
                // x = beta*cos(theta + alpha); y = beta*sin(theta + alpha ) 
                child.Offset = new Vector3((float)(_spiralTightness * theta * (Math.Cos(thetaOrientationRadians))) + _posX, (float)(_spiralTightness * theta * (Math.Sin(thetaOrientationRadians))) + _posY, 0);

                // Update the angle to be used for the next visual element
                theta += 4;
                thetaOrientationRadians = (theta + _spiralOrientation) * Math.PI / 180F;
            }
        }

        /// <summary>
        ///This method implicitly animates and rotates the visual elements into an ellipse layout
        /// on offset change  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EllipseLayout(object sender, RoutedEventArgs e)
        {
            // Define initial angle of each element on the ellipse
            double theta = 0;
            double thetaRadians = 0;

            foreach (var child in _root.Children)
            {
                // Change the Offset property of the visual. This will trigger the implicit animation that is associated with the Offset change.
                // The position of the element on the ellipse is defined using parametric equation:
                // x = alpha * cos(theta) ; y = beta*sin(theta)
                child.Offset = new Vector3((float)(_ellipseRadiusX * Math.Cos(thetaRadians)) + _posX, (float)(_ellipseRadiusY * Math.Sin(thetaRadians)) + _posY, 0);

                // Update the angle to be used for the next visual element
                theta += 2.5;
                thetaRadians = theta * Math.PI / 180F;
            }
        }
    }
}
