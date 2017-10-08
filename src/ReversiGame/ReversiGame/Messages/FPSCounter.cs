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
    public class FPSCounter : DrawableGameComponent
    {
        /// <summary>
        /// 当前游戏
        /// </summary>
        ReversiXNAGame curGame = null;
        /// <summary>
        /// 画画精灵
        /// </summary>
        SpriteBatch spriteBatch;
        SpriteFont fpsFont;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public FPSCounter(Game game, SpriteBatch screenSpriteBatch)
            : base(game)
        {
            curGame = (ReversiXNAGame)game;
            spriteBatch = screenSpriteBatch;
            fpsFont = curGame.Content.Load<SpriteFont>(@"Fonts\NormalFont");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            string fps = string.Format("fps: {0}", frameRate);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(2, 0), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(3, 0), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(4, 0), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(2, 1), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(4, 1), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(2, 2), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(3, 2), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(4, 2), Color.Black);
            spriteBatch.DrawString(fpsFont, fps, new Vector2(3, 1), Color.Gold);

            base.Draw(gameTime);
        }
    }
}
