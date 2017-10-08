using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using ReversiXNAGame;
using ReversiXNAGame.ReversiBoard;
using ReversiXNAGame = ReversiXNAGame.ReversiXNAGame;

namespace Walterlv.ReversiGame.FrameworkInterop
{
    public abstract class DrawableGameComponent
    {
        protected IDrawingSession spriteBatch { get; private set; }

        protected Game curGame { get; private set; }

        protected IKeyboard Keyboard { get; private set; }

        protected IMouse Mouse { get; private set; }

        private DrawableGameComponent _parent;

        private readonly List<DrawableGameComponent> _children = new List<DrawableGameComponent>();

        public virtual void Initialize()
        {
            _children.ForEach(x => x.Initialize());
        }

        public virtual void Update(GameTime time)
        {
            _children.ForEach(x => x.Update(time));
        }

        public virtual void Draw(GameTime time)
        {
            _children.ForEach(x => x.Draw(time));
        }

        public static T CreateGame<T>(IDrawingSession dc, IKeyboard keyboard, IMouse mouse)
            where T : Game, new()
        {
            var game = new T
            {
                spriteBatch = dc,
                Keyboard = keyboard,
                Mouse = mouse
            };
            game.curGame = game;
            return game;
        }

        protected T CreateChild<T>()
            where T : DrawableGameComponent, new()
        {
            var child = new T
            {
                _parent = this,
                curGame = curGame,
                spriteBatch = spriteBatch,
                Keyboard = Keyboard,
                Mouse = Mouse,
            };
            _children.Add(child);
            return child;
        }

        protected T CreateChild<T, TParam>(TParam param)
            where T : DrawableGameComponent
        {
            var child = (T) Activator.CreateInstance(typeof(T), param);
            child._parent = this;
            _children.Add(child);
            child.curGame = curGame;
            child.spriteBatch = spriteBatch;
            child.Keyboard = Keyboard;
            child.Mouse = Mouse;
            return child;
        }

        protected T CreateChild<T, TParam0, TParam1>(TParam0 param0, TParam1 param1)
            where T : DrawableGameComponent
        {
            var child = (T) Activator.CreateInstance(typeof(T), param0, param1);
            child._parent = this;
            _children.Add(child);
            child.curGame = curGame;
            child.spriteBatch = spriteBatch;
            child.Keyboard = Keyboard;
            child.Mouse = Mouse;
            return child;
        }

        protected T CreateChild<T, TParam0, TParam1, TParam2>(TParam0 param0, TParam1 param1, TParam2 param2)
            where T : DrawableGameComponent
        {
            var child = (T) Activator.CreateInstance(typeof(T), param0, param1, param2);
            child._parent = this;
            _children.Add(child);
            child.curGame = curGame;
            child.spriteBatch = spriteBatch;
            child.Keyboard = Keyboard;
            child.Mouse = Mouse;
            return child;
        }
    }

    public class Game : DrawableGameComponent
    {
        internal Board board { get; set; }
        public GameState State { get; set; }
        public bool IsActive { get; set; } = true;

        private readonly List<GameContent> _contents = new List<GameContent>();

        public ReadOnlyCollection<GameContent> Contents { get; }

        public Game()
        {
            Contents = new ReadOnlyCollection<GameContent>(_contents);
        }

        protected virtual void LoadContent()
        {
        }

        protected virtual void UnloadContent()
        {
        }

        public void LoadAllContents()
        {
            LoadContent();
        }

        public T LoadContent<T>(string path) where T : GameContent
        {
            var content = (T) Activator.CreateInstance(typeof(T), path);
            _contents.Add(content);
            return content;
        }
    }

    public interface IKeyboard
    {
        IKeyboardState GetState(params Keys[] keys);
    }

    public interface IKeyboardState
    {
        bool IsKeyDown(Keys key);
        bool IsKeyUp(Keys key);
    }

    public interface IMouse
    {
        IMouseState GetState();
    }

    public interface IMouseState
    {
        int X { get; }
        int Y { get; }
        ButtonState LeftButton { get; }
    }

    public enum ButtonState
    {
        Released,
        Pressed,
    }

    public interface IDrawingSession
    {
        void DrawString(SpriteFont fpsFont, string fps, Vector2 point, Color color);
        void Draw(Texture2D texture, Rectangle area, Color color);
    }

    public abstract class GameContent
    {
        public string Path { get; set; }

        protected GameContent(string path)
        {
            Path = path;
        }
    }

    public class Texture2D : GameContent
    {
        public Texture2D(string path) : base(path)
        {
        }
    }

    public class SpriteFont : GameContent
    {
        public SpriteFont(string path) : base(path)
        {
        }
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

        public int Left => X;

        public int Top => Y;

        public int Right => X + Width;

        public int Bottom => Y + Height;

        public int CenterX => X + Width / 2;

        public int CenterY => Y + Height / 2;
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
