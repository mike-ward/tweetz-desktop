using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

// ReSharper disable PossibleNullReferenceException

namespace tweetz5.Utilities.System
{
    public static class Screen
    {
        private static Matrix GetSizeFactors(Visual element)
        {
            Matrix transformToDevice;
            var source = PresentationSource.FromVisual(element);
            if (source != null)
            {
                transformToDevice = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var source2 = new HwndSource(new HwndSourceParameters()))
                {
                    transformToDevice = source2.CompositionTarget.TransformToDevice;
                }
            }
            return transformToDevice;
        }

        public static double HorizontalDpiToPixel(UIElement element, double x)
        {
            return x * GetSizeFactors(element).M11;
        }

        public static double VerticalDpiToPixel(UIElement element, double y)
        {
            return y * GetSizeFactors(element).M22;
        }

        public static double HorizontalPixelToDpi(UIElement element, double x)
        {
            return x / GetSizeFactors(element).M11;
        }

        public static double VerticalPixelToDpi(UIElement element, double y)
        {
            return y / GetSizeFactors(element).M22;
        }
    }

    public enum MeasureDirection
    {
        Horizontal,
        Vertical
    }
}