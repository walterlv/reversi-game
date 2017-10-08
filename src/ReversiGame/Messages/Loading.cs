using System;
using System.Collections.Generic;
using System.Linq;
using Walterlv.ReversiGame.FrameworkInterop;


namespace ReversiXNAGame.Messages
{
    public class Loading : DrawableGameComponent
    {
        // 画图范围
        Rectangle screenRectangle;
        // 加载纹理
        Texture2D board;
        Texture2D background;
        Texture2D loadingTexture;
        // 显示时间
        int showTime;
        long passedTime;

        public Loading(Rectangle screenRec)
        {
            screenRectangle = screenRec;
        }

        public override void Initialize()
        {
            showTime = 1000;
            passedTime = 0;

            loadingTexture = curGame.LoadContent<Texture2D>(@"Images\Loading");
            background = curGame.LoadContent<Texture2D>(@"Images\LoadingBackground");
            board = curGame.LoadContent<Texture2D>(@"Images\Board");

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            passedTime += (long) gameTime.ElapsedGameTime.TotalMilliseconds;
            if (passedTime > showTime)
                curGame.State = GameState.CheckLoaded;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (passedTime <= showTime)
            {
                spriteBatch.Draw(board, screenRectangle, Color.White);
                spriteBatch.Draw(background, screenRectangle, Color.White);
                spriteBatch.Draw(loadingTexture, screenRectangle, Color.White);
            }
            base.Draw(gameTime);
        }
    }
}
