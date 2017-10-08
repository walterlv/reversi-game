using System;
using System.Numerics;

namespace Walterlv.ReversiGame.FrameworkInterop
{
    public abstract class DrawableGameComponent
    {
        protected IDrawingSession spriteBatch { get; }

        public virtual void Initialize()
        {
        }

        public virtual void Update(GameTime time)
        {
        }

        public virtual void Draw(GameTime time)
        {
        }
    }

    public interface IDrawingSession
    {
        void DrawString(SpriteFont fpsFont, string fps, Vector2 point, Color color);
        void Draw(Texture2D texture, Rectangle area, Color color);
    }

    public class Texture2D
    {
    }

    public struct Color
    {
        public static Color Black { get; set; }
        public static Color Gold { get; set; }
        public static Color White { get; set; }
    }

    public struct Rectangle
    {
    }

    public class SpriteFont
    {
    }

    public class GameTime
    {
        public TimeSpan ElapsedGameTime { get; set; }
    }
}
