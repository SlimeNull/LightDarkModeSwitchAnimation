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

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        public RippleEffect(UIElement adornedElement) : base(adornedElement)
        {
            this._adornedElement = adornedElement;
        }

        public void Play(double speed, IEasingFunction? easingFunction)
        {
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

            double fromDiameter = Diameter;
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
                EasingFunction = easingFunction,
            };

            doubleAnimation.Completed += (s, e) =>
            {
                BeginAnimation(DiameterProperty, null);
                Diameter = maxDiameter;

                Completed?.Invoke(this, EventArgs.Empty);
            };

            BeginAnimation(DiameterProperty, doubleAnimation);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            var center = Center;
            var radius = Diameter / 2;

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
            DependencyProperty.Register(nameof(Center), typeof(Point), typeof(RippleEffect), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(RippleEffect), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty InnerBrushProperty =
            DependencyProperty.Register(nameof(InnerBrush), typeof(Brush), typeof(RippleEffect), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty OuterBrushProperty =
            DependencyProperty.Register(nameof(OuterBrush), typeof(Brush), typeof(RippleEffect), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public event EventHandler? Completed;
    }
}
