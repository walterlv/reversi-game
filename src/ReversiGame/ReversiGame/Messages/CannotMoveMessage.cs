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
    public class CannotMoveMessage : DrawableGameComponent
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
        Texture2D messageTexture;
        int showFrames;

        public CannotMoveMessage(Game game, SpriteBatch screenSpriteBatch)
            : base(game)
        {
            curGame = (ReversiXNAGame)game;
            spriteBatch = screenSpriteBatch;
            messageFont = curGame.Content.Load<SpriteFont>(@"Fonts\TitleFont");
            showFrames = 0;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
