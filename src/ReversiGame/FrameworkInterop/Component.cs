using System;
using System.Numerics;
using ReversiXNAGame;
using ReversiXNAGame.ReversiBoard;

namespace Walterlv.ReversiGame.FrameworkInterop
{
    public abstract class DrawableGameComponent
    {
        protected IDrawingSession spriteBatch { get; private set; }

        protected Game curGame { get; private set; }

        public virtual void Initialize()
        {
        }

        public virtual void Update(GameTime time)
        {
        }

        public virtual void Draw(GameTime time)
        {
        }

        public static T CreateGame<T>(IDrawingSession dc)
            where T : Game, new()
        {
            var game = new T
            {
                spriteBatch = dc,
            };
            game.curGame = game;
            return game;
        }

        protected T CreateChild<T>()
            where T : DrawableGameComponent, new()
        {
            var child = new T
            {
                curGame = curGame,
                spriteBatch = spriteBatch
            };
            return child;
        }

        protected T CreateChild<T, TParam>(TParam param)
            where T : DrawableGameComponent
        {
            var child = (T) Activator.CreateInstance(typeof(T), param);
            child.curGame = curGame;
            child.spriteBatch = spriteBatch;
            return child;
        }

        protected T CreateChild<T, TParam0, TParam1>(TParam0 param0, TParam1 param1)
            where T : DrawableGameComponent
        {
            var child = (T) Activator.CreateInstance(typeof(T), param0, param1);
            child.curGame = curGame;
            child.spriteBatch = spriteBatch;
            return child;
        }

        protected T CreateChild<T, TParam0, TParam1, TParam2>(TParam0 param0, TParam1 param1, TParam2 param2)
            where T : DrawableGameComponent
        {
            var child = (T) Activator.CreateInstance(typeof(T), param0, param1, param2);
            child.curGame = curGame;
            child.spriteBatch = spriteBatch;
            return child;
        }
    }

    public class Game : DrawableGameComponent
    {
        internal Board board { get; set; }
        public GameState State { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
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
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static Color Black { get; set; }
        public static Color Gold { get; set; }
        public static Color White { get; set; }
        public static Color Red { get; set; }
    }

    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int sizeX, int sizeY)
        {
            this.X = x;
            this.Y = y;
            this.Width = sizeX;
            this.Height = sizeY;
        }

        public int CenterX => X + Width / 2;

        public int CenterY => Y + Height / 2;
    }

    public class SpriteFont
    {
    }

    public struct GameTime
    {
        public TimeSpan ElapsedGameTime { get; set; }

        public static explicit operator GameTime(TimeSpan elapsedTime)
        {
            return new GameTime {ElapsedGameTime = elapsedTime};
        }
    }
}
