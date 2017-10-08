using System;
using System.Collections.Generic;
using System.Linq;

using Reversi;
using ReversiXNAGame.ReversiBoard;
using ReversiXNAGame.Settings;
using Walterlv.ReversiGame.FrameworkInterop;

namespace ReversiXNAGame.Players
{
    internal class Human : Player
    {
        // 表示已经下棋完成
        bool haveCompleted;
        // 鼠标状态
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        // 键盘状态
        KeyboardState currentKeyboardState = new KeyboardState();
        KeyboardState lastKeyboardState = new KeyboardState();
        bool isCtrlZPressed = false;

        public Human(Game game, SpriteBatch screenSpriteBatch, Rectangle boardRec, Piece[,] allPieces, ReversiPiece myColor)
            : base(game, screenSpriteBatch, boardRec, allPieces, myColor)
        {
            myType = PlayerTypes.Human;
            Name = "您";
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (isMyTurn && !isMovingPiece && curGame.IsActive)
            {
                // 获取键盘状态
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
                // 当按下 Ctrl+Z 时
                if ((currentKeyboardState.IsKeyDown(Keys.LeftControl) || currentKeyboardState.IsKeyDown(Keys.RightControl))
                    && currentKeyboardState.IsKeyDown(Keys.Z))
                {
                    isCtrlZPressed = true;
                }
                else if (currentKeyboardState.IsKeyUp(Keys.Z))
                {
                    if (isCtrlZPressed) { isCtrlZPressed = false; if (GameSettings.IsRegretEnabled) RegretMovePiece(); }
                }
                foreach (ReversiPiecePosition rpp in reversiGame.GetEnabledPositions())
                {
                    pieces[rpp.X, rpp.Y].CurrentDisplay = DisplayState.CanMove;
                }
                lastMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();
                ReversiPiecePosition tempPosition = Board.IsMouseInPiece(currentMouseState);
                // 如果移动鼠标
                if (tempPosition != null && reversiGame.IspieceEnabled(tempPosition))
                {
                    pieces[tempPosition.X, tempPosition.Y].CurrentDisplay = DisplayState.WillMove;
                    foreach (ReversiPiecePosition rpp in reversiGame.GetReversePositions(tempPosition))
                    {
                        pieces[rpp.X, rpp.Y].CurrentDisplay = DisplayState.WillReverse;
                    }
                }
                // 如果单击鼠标左键
                if ((currentMouseState.LeftButton == ButtonState.Released) && (lastMouseState.LeftButton == ButtonState.Pressed))
                {
                    if (tempPosition != null && reversiGame.IspieceEnabled(tempPosition))
                    {
                        isMovePieceCompleted = false;
                        /*pieces[tempPosition.X, tempPosition.Y].PieceState = (PieceState)myPieceColor;
                        foreach (ReversiPiecePosition rpp in reversiGame.GetReversePositions(tempPosition))
                            pieces[rpp.X, rpp.Y].PieceState = (PieceState)myPieceColor;*/
                        MovePiece(tempPosition);
                    }
                }
            }
            if (isMovePieceCompleted && isMyTurn)
            {
                if (!reversiGame.IsGameOver && reversiGame.CurrentPiece == reversiGame.LastPiece)
                {
                    // TODO: 添加无子可下的显示
                }
                isMyTurn = false;
                isMovingPiece = false;
            }

            base.Update(gameTime);
        }
    }
}
