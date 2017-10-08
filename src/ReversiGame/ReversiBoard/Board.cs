using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Reversi;
using ReversiXNAGame.Players;
using ReversiXNAGame.Messages;
using ReversiXNAGame.Settings;
using Walterlv.ReversiGame.FrameworkInterop;

namespace ReversiXNAGame.ReversiBoard
{
    public enum BoardState
    {
        FreshGame,
        InGame,
        GameOver,
    }

    internal class Board : DrawableGameComponent
    {
        ReversiGame reversiGame;
        Player[] player = new Player[2];

        #region 游戏组件属性与方法
        SpriteFont debugFont;
        string DebugText = "";
        // 棋盘纹理
        Texture2D boardTexture;
        // 棋盘位置
        Rectangle boardRectangle;
        // 棋盘状态
        private BoardState boardState;
        public BoardState CurrentBoardState
        {
            get { return boardState; }
            set { boardState = value; }
        }
        public static Board CurrentBoard { get; private set; }
        public static Rectangle BoardRectangle { get; set; }
        // 声明 64 个棋子, 并设置棋子大小
        Piece[,] pieces = new Piece[ReversiGame.BoardSize, ReversiGame.BoardSize];
        // 棋局状态
        public bool IsInitializing = false;

        // 获取鼠标指针所在的棋子位置
        public static ReversiPiecePosition IsMouseInPiece(MouseState currentMouseState)
        {
            if (currentMouseState.X > Board.BoardRectangle.X && currentMouseState.Y > Board.BoardRectangle.Y)
            {
                int x, y;
                x = (currentMouseState.X - Board.BoardRectangle.X) / (Board.BoardRectangle.Width / ReversiGame.BoardSize);
                y = (currentMouseState.Y - Board.BoardRectangle.Y) / (Board.BoardRectangle.Height / ReversiGame.BoardSize);
                return new ReversiPiecePosition(x, y);
            }
            return null;
        }

        public void UpdateAllPieces()
        {
        }

        public Board(Rectangle boardRec)
        {
            CurrentBoard = this;
            BoardRectangle = boardRectangle = boardRec;
            boardTexture = curGame.LoadContent<Texture2D>(@"Images\Board");
            debugFont = curGame.LoadContent<SpriteFont>(@"Fonts\TitleFont");
            int pieceSize = boardRec.Width / ReversiGame.BoardSize;
            for (int i = 0; i < ReversiGame.BoardSize; i++)
            {
                for (int j = 0; j < ReversiGame.BoardSize; j++)
                {
                    pieces[i, j] = CreateChild<Piece, Rectangle>(
                        new Rectangle(
                            boardRec.X + pieceSize * i,
                            boardRec.Y + pieceSize * j, pieceSize, pieceSize));
                }
            }
        }

        public override void Initialize()
        {
            IsInitializing = true;
            // 如果有上一局棋, 则清除上一局棋盘
            if (reversiGame != null) pieces[reversiGame.LastPosition.X, reversiGame.LastPosition.Y].IsLastPiece = false;
            // 创建一个新游戏
            reversiGame = ReversiGame.CreateANewGame();
            // 创建玩家
            for (int i = 0; i < 2; i++)
                if (PlayerSettings.Player[i].Type == PlayerTypes.Human)
                    player[i] = CreateChild<Human, Rectangle, Piece[,], ReversiPiece>(boardRectangle, pieces, i == 0 ? ReversiPiece.Black : ReversiPiece.White);
                else if (PlayerSettings.Player[i].Type == PlayerTypes.AI)
                {
                    player[i] = CreateChild<AI, Rectangle, Piece[,], ReversiPiece>(boardRectangle, pieces, i == 0 ? ReversiPiece.Black : ReversiPiece.White);
                    ((AI)(player[i])).SetAIType(PlayerSettings.Player[i].AIIndex);
                }
            // 请求刷新棋盘
            DebugText = "";
            boardState = BoardState.FreshGame;
            IsInitializing = false;
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            // 绘制棋盘
            if (boardState == BoardState.FreshGame)
            {
                for (int i = 0; i < ReversiGame.BoardSize; i++)
                    for (int j = 0; j < ReversiGame.BoardSize; j++)
                        pieces[i, j].PieceState = (PieceState)(reversiGame.CurrentBoard[i, j]);
                boardState = BoardState.InGame;
            }
            else if (boardState == BoardState.InGame)
            {
                if (!Player.isMovingPiece)
                {
                    if (!reversiGame.IsGameOver)
                    {
                        if (Player.isMovePieceCompleted)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (reversiGame.CurrentPiece == player[i].PieceColor)
                                {
                                    player[i].ToMyTurn();
                                    player[i].Update(gameTime);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (reversiGame.WinSide == player[0].PieceColor) DebugText = player[0].Name + " (黑子) 获胜!";
                        else if (reversiGame.WinSide == player[1].PieceColor) DebugText = player[1].Name + " (白子) 获胜!";
                        else DebugText = "黑白双方战成平局";
                        DebugText += " 黑白比分: " + reversiGame.BlackPieceNumber + ":" + reversiGame.WhitePieceNumber;
                        /*int i;
                        for (i = 0; i < 2; i++)
                        {
                            if (player[i].PlayerType == PlayerTypes.Human)
                            {
                                player[i].ToMyTurn();
                            }
                        }*/
                        boardState = BoardState.GameOver;
                    }
                }
                else
                {
                    Player.CurrentPlayer.Update(gameTime);
                }
                foreach (Piece piece in pieces) piece.Update(gameTime);
            }
            else if (boardState == BoardState.GameOver)
            {
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(boardTexture, boardRectangle, Color.White);
            foreach (Piece piece in pieces)
            {
                piece.Draw(gameTime);
            }
            try
            {
                if (DebugText.Length > 0)
                {
                    spriteBatch.DrawString(debugFont, DebugText, new Vector2(61, 307), Color.Black);
                    spriteBatch.DrawString(debugFont, DebugText, new Vector2(60, 306), Color.White);
                }
            }
            catch
            {
                spriteBatch.DrawString(debugFont, "???? (Font Lost) ???? Score: " + reversiGame.BlackPieceNumber + ":" + reversiGame.WhitePieceNumber, new Vector2(3, 14), Color.Red);
            }
            base.Draw(gameTime);
        }
        #endregion
    }
}
