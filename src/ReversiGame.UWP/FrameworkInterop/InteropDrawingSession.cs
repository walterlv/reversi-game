using System.Numerics;
using Microsoft.Graphics.Canvas;
using Walterlv.ReversiGame.FrameworkInterop;
using Color = Windows.UI.Color;
using InteropColor = Walterlv.ReversiGame.FrameworkInterop.Color;
using IDrawingSession = Walterlv.ReversiGame.FrameworkInterop.IDrawingSession;
using SpriteFont = Walterlv.ReversiGame.FrameworkInterop.SpriteFont;

namespace Walterlv.Gaming.Reversi.FrameworkInterop
{
    public class InteropDrawingSession : IDrawingSession
    {
        public CanvasDrawingSession Session { get; set; }

        public void DrawString(SpriteFont fpsFont, string fps, Vector2 point, InteropColor color)
        {
            Session?.DrawText(fps, point, color.ToColor());
        }

        public void Draw(Texture2D texture, Rectangle area, InteropColor color)
        {
        }
    }

    internal static class InteropExtensions
    {
        internal static Color ToColor(this InteropColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
