using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using Reversi;
using ReversiXNAGame.ReversiBoard;
using ReversiXNAGame.Messages;
using Walterlv.ReversiGame.FrameworkInterop;

namespace ReversiXNAGame.Players
{
    internal enum PlayerTypes
    {
        Human,
        AI,
    }

    internal abstract class Player : DrawableGameComponent
    {
        protected ReversiGame reversiGame;
        // 玩家信息
        protected static Player[] Players = new Player[2];
        public static Player CurrentPlayer;
        // 棋盘矩形
        protected Rectangle boardRectangle;
        // 玩家名字
        public string Name;
        // 玩家类型
        protected PlayerTypes myType;
        public PlayerTypes PlayerType
        {
            get { return myType; }
            protected set { myType = value; }
        }
        // 玩家棋子类型
        protected ReversiPiece myPieceColor;
        public ReversiPiece PieceColor
        {
            get { return myPieceColor; }
            protected set { myPieceColor = value; }
        }
        // 所有棋子
        protected Piece[,] pieces { get; private set; }
        // 当前轮到我下棋
        public bool isMyTurn;
        // 落子完成, 落子数据已经更新
        public static bool isMovePieceCompleted;
        // 正在落子, 计算中
        public static bool isMovingPiece;
        private Message message;
        // 下棋线程
        Thread movePieceThread;

        public void ToMyTurn()
        {
            isMyTurn = true;
            CurrentPlayer = this;
        }

        protected void MovePiece(ReversiPiecePosition position)
        {
            isMovingPiece = true;
            pieces[reversiGame.LastPosition.X, reversiGame.LastPosition.Y].IsLastPiece = false;
            pieces[position.X, position.Y].IsLastPiece = true;
            reversiGame.SetPieceMoveArgs(position, MovePiece_Confirmed, MovePiece_Completed);
            if (movePieceThread != null && movePieceThread.IsAlive) movePieceThread.Abort();
            movePieceThread = new Thread(new ThreadStart(reversiGame.PieceMoves));
            movePieceThread.Start();
        }
        public virtual void MovePiece_Confirmed(bool confirmed, ReversiPiece piece, ReversiPiecePosition position, List<ReversiPiecePosition> positions)
        {
            curGame.board.CurrentBoardState = BoardState.FreshGame;
        }

        protected void MovePiece_Completed(bool isSuccess, bool isGameOver, ReversiPiece nextPiece)
        {
            isMovePieceCompleted = true;
        }

        /// <summary>
        /// 悔棋
        /// </summary>
        protected void RegretMovePiece()
        {
            try
            {
                pieces[reversiGame.LastPosition.X, reversiGame.LastPosition.Y].IsLastPiece = false;
                reversiGame.SetRegretPieceArgs(RegretPiece_Finished);
                bool haveHuman = false, haveAI = false, regretToOwn;
                for (int i = 0; i < 2; i++)
                {
                    if (Players[i].myType == PlayerTypes.Human) haveHuman = true;
                    else haveAI = true;
                }
                if (haveHuman && haveAI) regretToOwn = true;
                else regretToOwn = false;
                reversiGame.RegretPieceMove(regretToOwn);
            }
            finally
            {
                for (int i = 0; i < ReversiGame.BoardSize; i++)
                    for (int j = 0; j < ReversiGame.BoardSize; j++)
                        pieces[i, j].PieceState = (PieceState)(reversiGame.CurrentBoard[i, j]);
            }
            if (reversiGame.BlackPieceNumber + reversiGame.WhitePieceNumber != 4)
                pieces[reversiGame.LastPosition.X, reversiGame.LastPosition.Y].IsLastPiece = true;
        }
        /// <summary>
        /// 此方法只能被回调. 
        /// 当向棋局实例发出悔棋请求后, 悔棋完成时会回调这个方法, 并在参数中告知悔棋完成.
        /// </summary>
        /// <param name="MoveRequired">如果为 true, 则表示悔棋已到最开始的棋局, 需要人工智能下一步棋.</param>
        private void RegretPiece_Finished(bool MoveRequired)
        {
        }

        public Player(Game game,SpriteBatch screenSpriteBatch, Rectangle boardRec, Piece[,] allPieces, ReversiPiece myColor)
            : base(game)
        {
            curGame = (ReversiXNAGame)game;
            reversiGame = ReversiGame.CurrentGame;
            spriteBatch = screenSpriteBatch;
            boardRectangle = boardRec;
            pieces = allPieces;
            myPieceColor = myColor;
            if (myColor == ReversiPiece.Black) Players[0] = this;
            else Players[1] = this;
            CurrentPlayer = Players[0];

            message = new Message(curGame, spriteBatch);
            isMyTurn = false;
            isMovePieceCompleted = true;
            isMovingPiece = false;
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
