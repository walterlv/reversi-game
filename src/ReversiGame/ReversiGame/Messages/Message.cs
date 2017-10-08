using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace ReversiXNAGame.Messages
{
    public class Message : DrawableGameComponent
    {
        /// <summary>
        /// 当前游戏
        /// </summary>
        ReversiXNAGame curGame = null;
        /// <summary>
        /// 画画精灵
        /// </summary>
        SpriteBatch spriteBatch;
        SpriteFont messageFont;
        //Rectangle messageRectangle;
        Texture2D messageTexture;
        int showFrames;

        string MessageText;

        public void ShowCannotMoveMessage(string cannotMoveName, string canMoveName)
        {
            MessageText = cannotMoveName + "无子可下, " + canMoveName + "继续下棋.";
            if (showFrames <= 0) showFrames = 200;
        }

        public Message(Game game, SpriteBatch screenSpriteBatch)
            : base(game)
        {
            curGame = (ReversiXNAGame)game;
            spriteBatch = screenSpriteBatch;
            messageFont = curGame.Content.Load<SpriteFont>(@"Fonts\TitleFont");
            //messageRectangle = messageRec;
            MessageText = "";
            showFrames = 0;
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
