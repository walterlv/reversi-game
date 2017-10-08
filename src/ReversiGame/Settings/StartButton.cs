using System;
using System.Collections.Generic;
using System.Linq;

using Reversi;
using ReversiXNAGame.ReversiBoard;
using Walterlv.ReversiGame.FrameworkInterop;

namespace ReversiXNAGame.Settings
{
    internal class StartButton : DrawableGameComponent
    {
        // 按钮位置
        Rectangle buttonRectangle;
        // 按钮样式
        SettingType type = SettingType.Start;
        bool startButtonClicked = false;
        public bool StartButtonClicked
        {
            get { return startButtonClicked; }
        }
        bool resumeButtonClicked = false;
        public bool ResumeButtonClicked
        {
            get { return resumeButtonClicked; }
        }
        Texture2D buttonTexture;
        Texture2D buttonStartNormalTexture;
        Texture2D buttonStartRollTexture;
        Texture2D buttonResumeNormalTexture;
        Texture2D buttonResumeRollTexture;
        Texture2D buttonRestartRollTexture;
        // 鼠标状态
        IMouseState currentMouseState;
        IMouseState lastMouseState;
        // 键盘状态
        IKeyboardState currentKeyboardState;
        IKeyboardState lastKeyboardState;

        public StartButton(Rectangle buttonRec)
        {
            buttonRectangle = buttonRec;

            buttonTexture = null;
            buttonStartNormalTexture = curGame.LoadContent<Texture2D>(@"Images\StartButton0");
            buttonStartRollTexture = curGame.LoadContent<Texture2D>(@"Images\StartButton1");
            buttonResumeNormalTexture = curGame.LoadContent<Texture2D>(@"Images\ResumeButton0");
            buttonResumeRollTexture = curGame.LoadContent<Texture2D>(@"Images\ResumeButton1");
            buttonRestartRollTexture = curGame.LoadContent<Texture2D>(@"Images\ResumeButton2");
        }


        public override void Initialize()
        {
            currentMouseState = Mouse.GetState();
            lastMouseState = Mouse.GetState();
            currentKeyboardState = Keyboard.GetState(Keys.Enter);
            lastKeyboardState = Keyboard.GetState(Keys.Enter);
        }

        public void Initialize(SettingType setType)
        {
            startButtonClicked = false;
            resumeButtonClicked = false;
            type = setType;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (curGame.IsActive)
            {
                // 获取鼠标状态
                lastMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();
                bool isMouseInUp = false, isMouseInDown = false;
                if (currentMouseState.X >= buttonRectangle.Left && currentMouseState.Y >= buttonRectangle.Top
                    && currentMouseState.X <= buttonRectangle.Right && currentMouseState.Y <= buttonRectangle.Bottom)
                {
                    if (currentMouseState.Y <= buttonRectangle.CenterY) isMouseInUp = true;
                    else isMouseInDown = true;
                }
                if (isMouseInUp || isMouseInDown)
                {
                    if (type == SettingType.Start) buttonTexture = buttonStartRollTexture;
                    else if (type == SettingType.Pause)
                    {
                        if (isMouseInUp) buttonTexture = buttonResumeRollTexture;
                        else if (isMouseInDown) buttonTexture = buttonRestartRollTexture;
                    }
                    // 如果单击鼠标左键
                    if ((currentMouseState.LeftButton == ButtonState.Released) && (lastMouseState.LeftButton == ButtonState.Pressed))
                    {
                        if (type == SettingType.Start || isMouseInDown) startButtonClicked = true;
                        else resumeButtonClicked = true;
                    }
                }
                // 当按下 Enter 键时
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState(Keys.Enter);
                if (currentKeyboardState.IsKeyUp(Keys.Enter) && lastKeyboardState.IsKeyDown(Keys.Enter))
                {
                    if (type == SettingType.Start) startButtonClicked = true;
                    else resumeButtonClicked = true;
                }
                if (buttonTexture == null)
                {
                    if (type == SettingType.Start) buttonTexture = buttonStartNormalTexture;
                    else if (type == SettingType.Pause) buttonTexture = buttonResumeNormalTexture;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (buttonTexture != null)
            {
                spriteBatch.Draw(buttonTexture, buttonRectangle, Color.White);
                buttonTexture = null;
            }
            base.Draw(gameTime);
        }
    }
}
