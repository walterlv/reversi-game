using Windows.Foundation;
using Windows.System;
using Walterlv.ReversiGame.FrameworkInterop;
using Color = Windows.UI.Color;

namespace Walterlv.Gaming.Reversi.FrameworkInterop
{
    internal static class InteropExtensions
    {
        internal static Color ToColor(this ReversiGame.FrameworkInterop.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        internal static Rect ToRect(this Rectangle rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        internal static VirtualKey ToVirtualKey(this Keys key)
        {
            return (VirtualKey) key;
        }
    }
}
