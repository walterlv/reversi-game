using System;
using System.Collections.Generic;
using System.Linq;

using ReversiXNAGame.ReversiBoard;
using ReversiXNAGame.Messages;
using ReversiXNAGame.Settings;

namespace ReversiXNAGame
{
    public enum GameState
    {
        Menu,
        StartLoad,
        Loading,
        CheckLoaded,
        InGame,
        Pause,
        Over,
    }

    internal class ReversiXNAGame : Microsoft.Xna.Framework.Game
    {
        const int ScreenWidth = 640;
        const int ScreenHeight = 640;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont debugFont;

        public GameState State;
        PlayerSettingBoard playerSettings;
        Loading loading;
        public Board board;
        FPSCounter fpsCounter;

        // 键盘状态
        KeyboardState currentKeyboardState = new KeyboardState();
        KeyboardState lastKeyboardState = new KeyboardState();

        public ReversiXNAGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: 添加初始化逻辑
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            State = GameState.Menu;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent 在一次游戏中只调用一次, 这里是加载内容的地方.
        /// </summary>
        protected override void LoadContent()
        {
            // 创建一个新的 SpriteBatch, 可以用来绘制纹理.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: 使用 this.Content 来加载游戏内容.
            debugFont = Content.Load<SpriteFont>(@"Fonts\TitleFont");
            Rectangle boardRectangle = new Rectangle((ScreenWidth - ScreenHeight) / 2, 0, ScreenHeight, ScreenHeight);
            playerSettings = new PlayerSettingBoard(this, spriteBatch, boardRectangle);
            playerSettings.Show(SettingType.Start);
            loading = new Loading(this, spriteBatch, boardRectangle);
            board = new Board(this, spriteBatch, boardRectangle);
            fpsCounter = new FPSCounter(this, spriteBatch);
        }

        /// <summary>
        /// UnloadContent 在一次游戏中只调用一次, 这是卸载内容的地方.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: 卸载任何非 ContentManager 管理的内容.
        }

        /// <summary>
        /// 允许游戏允许逻辑例如更新地图, 检查碰撞, 收集输入, 或者播放音效.
        /// </summary>
        /// <param name="gameTime">提供一个游戏时间快照.</param>
        protected override void Update(GameTime gameTime)
        {
            // 允许游戏退出
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // 获取键盘状态
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            // 当按下 F 键时
            if (currentKeyboardState.IsKeyDown(Keys.F) && lastKeyboardState.IsKeyUp(Keys.F))
            {
                GameSettings.ShowFPS = !(GameSettings.ShowFPS);
            }
            // 开始状态循环
            if (State == GameState.Menu)
            {
                playerSettings.Update(gameTime);
            }
            else if (State == GameState.StartLoad)
            {
                loading.Initialize();
                loading.Update(gameTime);
                board.Initialize();
                State = GameState.Loading;
            }
            else if (State == GameState.Loading)
            {
                loading.Update(gameTime);
            }
            else if (State == GameState.CheckLoaded)
            {
                if (board.IsInitializing)
                {
                    loading.Initialize();
                    loading.Update(gameTime);
                    State = GameState.Loading;
                }
                else
                {
                    board.Update(gameTime);
                    State = GameState.InGame;
                }
            }
            else if (State == GameState.InGame)
            {
                // 当按下 Escape 键时
                if (currentKeyboardState.IsKeyUp(Keys.Escape) && lastKeyboardState.IsKeyDown(Keys.Escape))
                {
                    if (board.CurrentBoardState == BoardState.GameOver)
                    {
                        State = GameState.Menu;
                        playerSettings.Show(SettingType.Start);
                    }
                    else
                    {
                        State = GameState.Pause;
                        playerSettings.Show(SettingType.Pause);
                    }
                    playerSettings.Update(gameTime);
                }
                board.Update(gameTime);
            }
            else if (State == GameState.Pause)
            {
                playerSettings.Update(gameTime);
            }
            else if (State == GameState.Over)
            {
            }
            if (GameSettings.ShowFPS) fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// 当游戏需要绘制时会被调用
        /// </summary>
        /// <param name="gameTime">提供一个游戏时间快照.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: 添加绘制代码
            spriteBatch.Begin();
            if (State == GameState.Menu)
            {
                playerSettings.Draw(gameTime);
            }
            else if (State == GameState.StartLoad || State == GameState.Loading)
            {
                loading.Draw(gameTime);
            }
            else if (State == GameState.CheckLoaded)
            {
                loading.Draw(gameTime);
            }
            else if (State == GameState.InGame)
            {
                board.Draw(gameTime);
            }
            else if (State == GameState.Pause)
            {
                playerSettings.Draw(gameTime);
            }
            else if (State == GameState.Over)
            {
            }
            if (GameSettings.ShowFPS) fpsCounter.Draw(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /*// 切换全屏显示
        private void ToggleFullScreen()
        {
            PresentationParameters presentation =
                graphics.GraphicsDevice.PresentationParameters;
            if (presentation.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 600;
                Window.AllowUserResizing = false;
            }
            else
            {
                GraphicsAdapter adapter = graphics.GraphicsDevice.CreationParameters.Adapter;
                graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
            }
            graphics.ToggleFullScreen();
        }*/
    }
}
