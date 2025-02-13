using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EleCho.WpfSuite.Helpers;

namespace LightDarkModeSwitchAnimation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDark;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var center = new Point(0, 0);

            var root = VisualTreeHelper.GetChild(this, 0) as UIElement;
            if (root is null)
            {
                return;
            }

            var dpi = Dpi.GetFromVisual(this);

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)(root.RenderSize.Width * dpi.FactorX), (int)(root.RenderSize.Height * dpi.FactorY), dpi.X, dpi.Y, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(root);

            if (sender is UIElement uiElement)
            {
                center = uiElement.TranslatePoint(new Point(uiElement.RenderSize.Width / 2, uiElement.RenderSize.Height / 2), rootGrid);
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(rootGrid);
            var ripple = new RippleEffect(rootGrid)
            {
                Center = center,
                Speed = 3000,
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut },
                OuterBrush = new ImageBrush(renderTargetBitmap),
            };

            ripple.Completed += (s, e) =>
            {
                adornerLayer.Remove(ripple);
            };

            adornerLayer.Add(ripple);
            ripple.Play();

            if (!_isDark)
            {
                Background = Brushes.Black;
            }
            else
            {
                Background = Brushes.White;
            }

            _isDark ^= true;
        }
    }
}