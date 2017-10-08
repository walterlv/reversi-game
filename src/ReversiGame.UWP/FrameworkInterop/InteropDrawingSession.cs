using System.Numerics;
using Microsoft.Graphics.Canvas;
using Walterlv.ReversiGame.FrameworkInterop;

namespace Walterlv.Gaming.Reversi.FrameworkInterop
{
    public class InteropDrawingSession : IDrawingSession
    {
        public CanvasDrawingSession Session { get; set; }

        public void DrawString(SpriteFont fpsFont, string fps, Vector2 point, Color color)
        {
        }

        public void Draw(Texture2D texture, Rectangle area, Color color)
        {
        }
    }
}
