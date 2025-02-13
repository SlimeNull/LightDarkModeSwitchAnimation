using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LightDarkModeSwitchAnimation.Utilities
{
    public static class ResourceDictionaryUtils
    {
        public static T? FindResourceDictionary<T>(ResourceDictionary? root)
            where T : ResourceDictionary
        {
            if (root is null)
            {
                return default;
            }

            if (root is T result)
            {
                return result;
            }

            foreach (var child in root.MergedDictionaries)
            {
                if (FindResourceDictionary<T>(child) is T resultInChild)
                {
                    return resultInChild;
                }
            }

            return default;
        }
    }
}
