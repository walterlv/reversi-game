using System;
using System.Collections.Generic;
using System.Linq;

using ReversiXNAGame.Settings;
using Walterlv.ReversiGame.FrameworkInterop;

namespace ReversiXNAGame.ReversiBoard
{
    internal enum PieceState
    {
        Empty,
        Black,
        White,
        Reversing,
    }
    internal enum DisplayState
    {
        Normal,
        CanMove,
        WillMove,
        WillReverse,
    }

    internal class Piece : DrawableGameComponent
    {
        Texture2D pieceTexture;
        Texture2D pieceTextureBlack;
        Texture2D pieceTextureWhite;
        Texture2D displayTexture;
        Texture2D isEnabledTexture;
        Texture2D willMoveTexture;
        Texture2D willReverseTexture;
        Texture2D isLastPieceTexture;
        Texture2D lastPieceTexture;
        Rectangle pieceRectangle;
        PieceState currentState;
        PieceState nextState;
        public PieceState PieceState
        {
            get { return currentState; }
            set { nextState = value; }
        }
        DisplayState currentDisplay;
        public DisplayState CurrentDisplay
        {
            get { return currentDisplay; }
            set { currentDisplay = value; }
        }
        public bool IsLastPiece { get; set; }
        // 强制显示所有状态 (用于非棋盘如菜单等处)
        public bool ForceShow { get; set; }

        public Piece(Rectangle pieceRec)
        {
            pieceRectangle = pieceRec;
            pieceTextureBlack = curGame.LoadContent<Texture2D>(@"Images\BlackPiece");
            pieceTextureWhite = curGame.LoadContent<Texture2D>(@"Images\WhitePiece");
            isEnabledTexture = curGame.LoadContent<Texture2D>(@"Images\Enabled");
            willMoveTexture = curGame.LoadContent<Texture2D>(@"Images\WillMove");
            willReverseTexture = curGame.LoadContent<Texture2D>(@"Images\WillReverse");
            lastPieceTexture = curGame.LoadContent<Texture2D>(@"Images\PieceFlag");
            currentState = PieceState.Empty;
            nextState = PieceState.Empty;
            currentDisplay = DisplayState.Normal;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (nextState != currentState)
            {
                if (nextState == PieceState.Black) pieceTexture = pieceTextureBlack;
                else if (nextState == PieceState.White) pieceTexture = pieceTextureWhite;
                else pieceTexture = null;
                currentState = nextState;
            }
            if (currentDisplay == DisplayState.WillMove)
                displayTexture = willMoveTexture;
            else if (currentDisplay == DisplayState.WillReverse && (ForceShow || GameSettings.ShowReversePieces))
                displayTexture = willReverseTexture;
            else if (currentDisplay == DisplayState.CanMove && (ForceShow || GameSettings.ShowEnabledPieces))
                displayTexture = isEnabledTexture;
            else displayTexture = null;
            if (IsLastPiece) isLastPieceTexture = lastPieceTexture;
            else isLastPieceTexture = null;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (displayTexture != null)
            {
                spriteBatch.Draw(displayTexture, pieceRectangle, Color.White);
                currentDisplay = DisplayState.Normal;
            }
            if (pieceTexture != null) spriteBatch.Draw(pieceTexture, pieceRectangle, Color.White);
            if (isLastPieceTexture != null && pieceTexture != null) spriteBatch.Draw(isLastPieceTexture, pieceRectangle, Color.White);
            
            base.Draw(gameTime);
        }
    }
}
