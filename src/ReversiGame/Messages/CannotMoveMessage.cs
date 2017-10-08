using System;
using System.Collections.Generic;
using System.Linq;
using Walterlv.ReversiGame.FrameworkInterop;


namespace ReversiXNAGame.Messages
{
    public class CannotMoveMessage : DrawableGameComponent
    {
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
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
        }
    }
}
