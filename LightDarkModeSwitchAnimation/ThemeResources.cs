using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LightDarkModeSwitchAnimation
{
    public class ThemeResources : ResourceDictionary
    {
        private bool _isDark;
        private readonly ResourceDictionary _dark = new ResourceDictionary() {Source = new Uri("pack://application:,,,/LightDarkModeSwitchAnimation;component/Themes/Dark.xaml") };
        private readonly ResourceDictionary _light = new ResourceDictionary() {Source = new Uri("pack://application:,,,/LightDarkModeSwitchAnimation;component/Themes/Light.xaml") };

        public ThemeResources()
        {
            MergedDictionaries.Add(_light);
        }

        public bool IsDark
        {
            get => _isDark;
            set
            {
                if (_isDark == value)
                {
                    return;
                }

                if (value)
                {
                    MergedDictionaries.Remove(_light);
                    MergedDictionaries.Add(_dark);
                }
                else
                {
                    MergedDictionaries.Remove(_dark);
                    MergedDictionaries.Add(_light);
                }

                _isDark = value;
            }
        }
    }
}
