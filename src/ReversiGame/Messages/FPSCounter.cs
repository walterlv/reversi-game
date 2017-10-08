using System;
using System.Numerics;
using Walterlv.ReversiGame.FrameworkInterop;

namespace ReversiXNAGame.Messages
{
    public class FPSCounter : DrawableGameComponent
    {
        /// <summary>
        /// 画画精灵
        /// </summary>
        SpriteFont fpsFont;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

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
