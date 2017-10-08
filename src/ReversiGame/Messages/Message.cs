using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Walterlv.ReversiGame.FrameworkInterop;


namespace ReversiXNAGame.Messages
{
    public class Message : DrawableGameComponent
    {
        /// <summary>
        /// 画画精灵
        /// </summary>
        SpriteFont messageFont;
        //Rectangle messageRectangle;
        Texture2D messageTexture;
        int showFrames = 0;

        string MessageText = "";

        public void ShowCannotMoveMessage(string cannotMoveName, string canMoveName)
        {
            MessageText = cannotMoveName + "无子可下, " + canMoveName + "继续下棋.";
            if (showFrames <= 0) showFrames = 200;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (showFrames-- <= 0 && MessageText.Length > 0) MessageText = "";

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (MessageText.Length > 0)
                spriteBatch.DrawString(messageFont, MessageText, new Vector2(100, 100), Color.White);
            base.Draw(gameTime);
        }
    }
}
