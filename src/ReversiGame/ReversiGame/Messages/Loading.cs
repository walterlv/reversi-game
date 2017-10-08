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
    public class Loading : DrawableGameComponent
    {
        // 当前游戏
        ReversiXNAGame curGame = null;
        // 画图精灵
        SpriteBatch spriteBatch;
        // 画图范围
        Rectangle screenRectangle;
        // 加载纹理
        Texture2D board;
        Texture2D background;
        Texture2D loadingTexture;
        // 显示时间
        int showTime;
        int passedTime;

        public Loading(Game game, SpriteBatch screenSpriteBatch, Rectangle screenRec)
            : base(game)
        {
            curGame = (ReversiXNAGame)game;
            spriteBatch = screenSpriteBatch;
            screenRectangle = screenRec;
            loadingTexture = curGame.Content.Load<Texture2D>(@"Images\Loading");
            background = curGame.Content.Load<Texture2D>(@"Images\LoadingBackground");
            board = curGame.Content.Load<Texture2D>(@"Images\Board");
        }

        public override void Initialize()
        {
            showTime = 1000;
            passedTime = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            passedTime += gameTime.ElapsedGameTime.Milliseconds;
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
