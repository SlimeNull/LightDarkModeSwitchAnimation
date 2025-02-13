using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LightDarkModeSwitchAnimation
{
    public class RippleEffect : Adorner
    {
        private readonly UIElement _adornedElement;
        private RectangleGeometry? _cachedRectangleGeometry;
        private EllipseGeometry? _cachedEllipseGeometry;
        private CombinedGeometry? _cachedOuterGeometry;

        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public Brush InnerBrush
        {
            get { return (Brush)GetValue(InnerBrushProperty); }
            set { SetValue(InnerBrushProperty, value); }
        }

        public Brush OuterBrush
        {
            get { return (Brush)GetValue(OuterBrushProperty); }
            set { SetValue(OuterBrushProperty, value); }
        }

        public double CurrentDiameter
        {
            get { return (double)GetValue(CurrentDiameterProperty); }
            set { SetValue(CurrentDiameterProperty, value); }
        }

        public RippleEffect(UIElement adornedElement) : base(adornedElement)
        {
            this._adornedElement = adornedElement;
        }

        private static double GetDistance(Point point1, Point point2)
        {
            return new Vector(point2.X - point1.X, point2.Y - point1.Y).Length;
        }

        public void Play()
        {
            var speed = Speed;
            var center = Center;
            var renderSize = _adornedElement.RenderSize;

            var d1 = new Vector(center.X, center.Y);
            var d2 = new Vector(renderSize.Width - center.X, center.Y);
            var d3 = new Vector(center.X, renderSize.Height - center.Y);
            var d4 = new Vector(renderSize.Width - center.X, renderSize.Height - center.Y);
            var maxRadiusSquared = Math.Max(
                Math.Max(d1.LengthSquared, d2.LengthSquared),
                Math.Max(d3.LengthSquared, d4.LengthSquared));

            double maxDiameter = Math.Sqrt(maxRadiusSquared) * 2;

            double fromDiameter = CurrentDiameter;
            if (fromDiameter > maxDiameter)
            {
                fromDiameter = 0;
            }

            double timeSeconds = (maxDiameter - fromDiameter) / speed;

            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                From = fromDiameter,
                To = maxDiameter,
                Duration = new Duration(TimeSpan.FromSeconds(timeSeconds)),
            };

            doubleAnimation.Completed += (s, e) =>
            {
                BeginAnimation(CurrentDiameterProperty, null);
                CurrentDiameter = maxDiameter;

                Completed?.Invoke(this, EventArgs.Empty);
            };

            BeginAnimation(CurrentDiameterProperty, doubleAnimation);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            var center = Center;
            var radius = CurrentDiameter / 2;

            var innerBrush = InnerBrush;
            var outerBrush = OuterBrush;

            var wholeRect = new Rect(0, 0, actualWidth, actualHeight);
            var rectangleGeometry = _cachedRectangleGeometry ??= new RectangleGeometry();
            var ellipseGeometry = _cachedEllipseGeometry ??= new EllipseGeometry();
            var outerGeometry = _cachedOuterGeometry ??= new CombinedGeometry();

            rectangleGeometry.Rect = wholeRect;
            ellipseGeometry.Center = center;
            ellipseGeometry.RadiusX = radius;
            ellipseGeometry.RadiusY = radius;
            outerGeometry.Geometry1 = rectangleGeometry;
            outerGeometry.Geometry2 = ellipseGeometry;
            outerGeometry.GeometryCombineMode = GeometryCombineMode.Exclude;

            if (innerBrush is not null)
            {
                drawingContext.PushClip(ellipseGeometry);
                drawingContext.DrawRectangle(innerBrush, null, wholeRect);
                drawingContext.Pop();
            }

            if (outerBrush is not null)
            {
                drawingContext.PushClip(outerGeometry);
                drawingContext.DrawRectangle(outerBrush, null, wholeRect);
                drawingContext.Pop();
            }

        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(RippleEffect), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(double), typeof(RippleEffect), new FrameworkPropertyMetadata(300.0));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(RippleEffect), new PropertyMetadata(null));

        public static readonly DependencyProperty OuterBrushProperty =
            DependencyProperty.Register("OuterBrush", typeof(Brush), typeof(RippleEffect), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty InnerBrushProperty =
            DependencyProperty.Register("InnerBrush", typeof(Brush), typeof(RippleEffect), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CurrentDiameterProperty =
            DependencyProperty.Register("CurrentDiameter", typeof(double), typeof(RippleEffect), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public event EventHandler? Completed;
    }
}
