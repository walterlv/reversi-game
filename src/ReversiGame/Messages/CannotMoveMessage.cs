using System;
using System.Collections.Generic;
using System.Linq;
using Walterlv.ReversiGame.FrameworkInterop;


namespace ReversiXNAGame.Messages
{
    public class CannotMoveMessage : DrawableGameComponent
    {
        SpriteFont messageFont;
        int showFrames = 0;

        public CannotMoveMessage()
        {
            messageFont = curGame.Content.Load<SpriteFont>(@"Fonts\TitleFont");
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
