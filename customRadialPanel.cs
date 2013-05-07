using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoftwareEng
{
    /// <summary>
    /// 
    /// Taken from http://alexpkent.wordpress.com/2012/10/04/wpf-radial-context-menu-pie-menu/ with permission
    /// 
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SoftwareEng"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SoftwareEng;assembly=SoftwareEng"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.   customRadialPanel
    ///
    ///     <MyNamespace:customRadialPanel/>
    ///
    /// </summary>
    public class customRadialPanel : Panel
    {
        #region Dependency Properties


        //the radius of the outer circle defining the panel as a double
        public double OuterRadius
        {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }


        //the actual outer radius property that can be tied to other things (data bound etc.)
        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register("OuterRadius", typeof(double), typeof(customRadialPanel),
            new UIPropertyMetadata(0.0, OuterRadiusChanged));

        //event handler that is called the outer radius property changes
        private static void OuterRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var arcPanel = obj as customRadialPanel;
            if (arcPanel != null)
            {
                arcPanel.Width = (double)e.NewValue * 2;
                arcPanel.Height = (double)e.NewValue * 2;
            }
        }

        //the radius of the inner circle defining the panel as a double
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        //the actual inner radius property that can be tied to other things (data bound etc.)
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(customRadialPanel), new UIPropertyMetadata(0.0));


        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        //the actual angle property that can be tied to other things (data bound etc.)
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(customRadialPanel), new UIPropertyMetadata(360.0));

        //determines whether items on panel will be arranged clockwise or counter-clockwise
        public bool IsClockwise
        {
            get { return (bool)GetValue(IsClockwiseProperty); }
            set { SetValue(IsClockwiseProperty, value); }
        }

        //the actual IsClockwise property that can be tied to other things (data bound etc.)
        public static readonly DependencyProperty IsClockwiseProperty =
            DependencyProperty.Register("IsClockwise", typeof(bool), typeof(customRadialPanel), new UIPropertyMetadata(true));

        // the angle (in degrees) from the top position
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }


        // angle property that can be tied to other things (data bound etc.)
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(customRadialPanel), new UIPropertyMetadata(0.0));

        public bool ShowBorder
        {
            get { return (bool)GetValue(ShowBorderProperty); }
            set { SetValue(ShowBorderProperty, value); }
        }

        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.Register("ShowBorder", typeof(bool), typeof(customRadialPanel), new UIPropertyMetadata(true));

        public bool ShowPieLines
        {
            get { return (bool)GetValue(ShowPieLinesProperty); }
            set { SetValue(ShowPieLinesProperty, value); }
        }

        public static readonly DependencyProperty ShowPieLinesProperty =
            DependencyProperty.Register("ShowPieLines", typeof(bool), typeof(customRadialPanel), new UIPropertyMetadata(false));

        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Brush), typeof(customRadialPanel),
                                        new UIPropertyMetadata(Brushes.Gray));

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(customRadialPanel),
                                        new UIPropertyMetadata(Colors.LightGray));

        public double BackgroundOpacity
        {
            get { return (double)GetValue(BackgroundOpacityProperty); }
            set { SetValue(BackgroundOpacityProperty, value); }
        }

        public static readonly DependencyProperty BackgroundOpacityProperty =
            DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(customRadialPanel),
                                        new UIPropertyMetadata(0.4));

        #endregion


        private double _angleEach;


        /****************************************************************************************************
         * One of several functions that must be overridden when inhereting from the Panel class
         * 
         ***************************************************************************************************/
        protected override Size MeasureOverride(Size availableSize)
        {
            CalculateAnglePerSection();

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
            }

            return new Size(2 * OuterRadius, 2 * OuterRadius);
        }


        /****************************************************************************************************
         *
         * 
         ***************************************************************************************************/
        protected override Size ArrangeOverride(Size finalSize)
        {
            CalculateAnglePerSection();

            var startAngle = AngleToRadian(StartAngle);
            var startPointX = OuterRadius + (IsClockwise ? 1 : -1) * Math.Sin(startAngle) * (OuterRadius + InnerRadius) / 2;
            var startPointY = (OuterRadius - InnerRadius) / 2 + (1 - Math.Cos(startAngle)) * (OuterRadius + InnerRadius) / 2;
            var currentPosition = new Point(startPointX, startPointY);

            int childCount = Children.Count;
            double perAngle = AngleToRadian(Angle) / childCount;

            for (int i = 0; i < childCount; i++)
            {
                UIElement child = Children[i];

                var angle = (i + 1) * perAngle + startAngle;
                var offsetX = Math.Sin(angle) * (OuterRadius + InnerRadius) / 2;
                var offsetY = (1 - Math.Cos(angle)) * (OuterRadius + InnerRadius) / 2;

                var childRect = new Rect(new Point(currentPosition.X - child.DesiredSize.Width / 2,
                                                currentPosition.Y - child.DesiredSize.Height / 2),
                                        new Point(currentPosition.X + child.DesiredSize.Width / 2,
                                                currentPosition.Y + child.DesiredSize.Height / 2));
                child.Arrange(childRect);
                currentPosition.X = (IsClockwise ? 1 : -1) * offsetX + OuterRadius;
                currentPosition.Y = offsetY + (OuterRadius - InnerRadius) / 2;
            }

            return new Size(2 * OuterRadius, 2 * OuterRadius);
        }


        /****************************************************************************************************
         *
         * 
         ***************************************************************************************************/
        private void CalculateAnglePerSection()
        {
            _angleEach = Angle / InternalChildren.Count;
        }


        /****************************************************************************************************
         *
         * 
         ***************************************************************************************************/
        private static double AngleToRadian(double angle)
        {
            return angle * Math.PI / 180;
        }


        /****************************************************************************************************
         *
         * 
         ***************************************************************************************************/
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var centerPoint = new Point(RenderSize.Width / 2.0, RenderSize.Height / 2.0);
            var pen = new Pen(BorderColor, 1.0) { DashStyle = DashStyles.Solid };

            // Display inner & outer circles.
            if (ShowBorder)
            {
                var renderBrush = new SolidColorBrush(BackgroundColor) { Opacity = BackgroundOpacity };

                var geometryGroup = new GeometryGroup { FillRule = FillRule.EvenOdd };
                var outerBorder = OuterRadius - 1;
                geometryGroup.Children.Add(new EllipseGeometry { RadiusX = outerBorder, RadiusY = outerBorder, Center = centerPoint });
                geometryGroup.Children.Add(new EllipseGeometry { RadiusX = InnerRadius, RadiusY = InnerRadius, Center = centerPoint });

                dc.DrawGeometry(renderBrush, pen, geometryGroup);
            }

            if (ShowPieLines)
            {
                if (InternalChildren.Count == 1)
                    return;

                // Initialize angle.
                var angleChild = -(_angleEach / 2.0) - 90.0;

                //Take into account the requested start angle
                angleChild += StartAngle;

                // Loop through each child to draw radial lines from center.
                #pragma warning disable 168
                foreach (var child in InternalChildren)
                #pragma warning restore 168
                {
                    var angleChildInRadian = 2.0 * Math.PI * angleChild / 360;
                    var innerPoint = new Point(centerPoint.X + (InnerRadius * Math.Cos(angleChildInRadian)),
                                               centerPoint.Y + (InnerRadius * Math.Sin(angleChildInRadian)));
                    var outerPoint = new Point(centerPoint.X + (OuterRadius * Math.Cos(angleChildInRadian)),
                                               centerPoint.Y + (OuterRadius * Math.Sin(angleChildInRadian)));
                    dc.DrawLine(pen, innerPoint, outerPoint);
                    angleChild += _angleEach;
                }
            }
        }

        /*
        static customRadialPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(customRadialPanel), new FrameworkPropertyMetadata(typeof(customRadialPanel)));
        }
        */
    }
}
