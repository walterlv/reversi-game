using System;
using System.Collections.Generic;

namespace Reversi
{
    public delegate void WhenMovePiece_Confirmed(bool confirmed, ReversiPiece piece, ReversiPiecePosition position, List<ReversiPiecePosition> positions);
    public delegate void WhenMovePiece_Completed(bool IsSuccess, bool isGameOver, ReversiPiece nextPiece);
    public delegate void WhenRegretPiece_Finished(bool haveRegretedToTotalStart);

    public class ReversiGame
    {
        public const int BoardSize = 8;
        /// <summary>
        /// 当前棋盘
        /// </summary>
        public ReversiPiece[,] CurrentBoard { get; private set; }
        /// <summary>
        /// 获取当前正准备落子的一方
        /// </summary>
        public ReversiPiece CurrentPiece { get; private set; }
        /// <summary>
        /// 获取上一个落子的棋子类型
        /// </summary>
        public ReversiPiece LastPiece { get; private set; }
        /// <summary>
        /// 获取上一个落子的位置
        /// </summary>
        public ReversiPiecePosition LastPosition { get; private set; }
        /// <summary>
        /// 指示当前游戏是否结束 (是否赢棋)
        /// </summary>
        public bool IsGameOver { get; private set; }
        /// <summary>
        /// 获取当前赢棋的一方 (Empty 表示无人赢棋)
        /// </summary>
        public ReversiPiece WinSide { get; private set; }
        /// <summary>
        /// 黑子数目
        /// </summary>
        public int BlackPieceNumber { get; private set; }
        /// <summary>
        /// 白子数目
        /// </summary>
        public int WhitePieceNumber { get; private set; }
        /// <summary>
        /// 获取已经下棋的总步数
        /// </summary>
        public int TotalSteps { get; private set; }
        /// <summary>
        /// 获取棋局缓存
        /// </summary>
        public ReversiBuffer reversiBuffer { get; private set; }
        /// <summary>
        /// 棋局栈, 记录每一步下棋
        /// </summary>
        private RecersiGameStateStack gameStateStack;
        /// <summary>
        /// 当前是否允许悔棋
        /// </summary>
        public bool isRegretEnabled { get; private set; }
        /// <summary>
        /// 当前允许的最大悔棋步数
        /// </summary>
        public int MaxRegretSteps { get; private set; }
        /// <summary>
        /// 只是当前是否正在进行缓存重建 (或者表示当前是否正在执行下棋操作)
        /// </summary>
        public bool IsBusy { get; private set; }

        // 为使用多线程而储存的落子参数
        private bool MovePieceHandled;
        private ReversiPiecePosition MovePiece_Position;
        private WhenMovePiece_Confirmed MovePiece_Confirmed;
        private WhenMovePiece_Completed MovePiece_Completed;
        private WhenRegretPiece_Finished RegretPiece_Finished;
        
        #region 管理黑白棋棋局实例
        /// <summary>
        /// 获取当前正在进行的棋局
        /// </summary>
        public static ReversiGame CurrentGame { get; private set; }

        /// <summary>
        /// 创建一个黑白棋棋盘, 其大小为 8*8. 中间 4 个初始棋子为白黑白黑.
        /// </summary>
        /// <returns></returns>
        public static ReversiGame CreateANewGame()
        {
            return new ReversiGame();
        }
        public static ReversiGame CreateANewGame(ReversiPiece[,] initialReversiBoard)
        {
            return new ReversiGame(initialReversiBoard);
        }
        #endregion

        #region 创建棋局实例
        /// <summary>
        /// 初始化一个黑白棋棋盘, 大小为 8*8. 中间 4 个初始棋子为白黑白黑.
        /// </summary>
        private ReversiGame()
        {
            CurrentBoard = new ReversiPiece[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    CurrentBoard[i, j] = ReversiPiece.Empty;
            CurrentBoard[BoardSize / 2 - 1, BoardSize / 2 - 1] = ReversiPiece.White;
            CurrentBoard[BoardSize / 2, BoardSize / 2 - 1] = ReversiPiece.Black;
            CurrentBoard[BoardSize / 2 - 1, BoardSize / 2] = ReversiPiece.Black;
            CurrentBoard[BoardSize / 2, BoardSize / 2] = ReversiPiece.White;
            // TODO: 删除用于调试的初始棋局
            /*CurrentBoard[0, 0] = ReversiPiece.Black;
            CurrentBoard[0, 1] = ReversiPiece.Black;
            CurrentBoard[1, 0] = ReversiPiece.White;
            CurrentBoard[1, 1] = ReversiPiece.White;*/
            CurrentGame = this;
            CurrentPiece = ReversiPiece.Black;
            LastPiece = ReversiPiece.White;
            LastPosition = new ReversiPiecePosition(4, 4);
            TotalSteps = 0;
            reversiBuffer = new ReversiBuffer();
            IsBusy = false;
            isRegretEnabled = false;
            MaxRegretSteps = 0;
            gameStateStack = new RecersiGameStateStack();
            UpdateState();
            ResetBuffer();
        }
        /// <summary>
        /// 初始化一个有特定棋局的黑白棋棋盘, 其棋局由参数确定.
        /// </summary>
        /// <param name="initialReversiBoard">是一个二维数组, 指定黑白棋棋局.</param>
        private ReversiGame(ReversiPiece[,] initialReversiBoard)
        {
            CurrentBoard = initialReversiBoard;
            CurrentGame = this;
            CurrentPiece = ReversiPiece.Black;
            LastPiece = ReversiPiece.White;
            LastPosition = new ReversiPiecePosition(4, 4);
            reversiBuffer = new ReversiBuffer();
            IsBusy = false;
            UpdateState();
            ResetBuffer();
        }
        #endregion

        #region 实现落子逻辑
        /// <summary>
        /// 检查落子的绝对合法性
        /// </summary>
        /// <param position="x">落子的坐标</param>
        private void CheckpieceValid(ReversiPiecePosition position)
        {
            if ((position.X < 0 || position.X >= BoardSize) || (position.Y < 0 || position.Y >= BoardSize))
                throw new Exception("棋子已超出棋盘外! (" + position.X + ", " + position.Y + ")");
        }
        /// <summary>
        /// 检查落子的绝对合法性
        /// </summary>
        /// <param name="x">落子的 x 坐标</param>
        /// <param name="y">落子的 y 坐标</param>
        private void CheckpieceValid(int x, int y)
        {
            CheckpieceValid(new ReversiPiecePosition(x, y));
        }

        /// <summary>
        /// 判断是否可以在指定的点落子
        /// </summary>
        /// <param name="x">落子的 x 坐标</param>
        /// <param name="y">落子的 y 坐标</param>
        /// <returns>如果可以在此处落子, 则返回 true.</returns>
        public bool IspieceEnabled(int x, int y)
        {
            return IspieceEnabled(new ReversiPiecePosition(x, y));
        }
        /// <summary>
        /// 判断是否可以在指定的点落子
        /// </summary>
        /// <param name="location">落子的坐标</param>
        /// <returns>如果可以在此处落子, 则返回 true.</returns>
        public bool IspieceEnabled(ReversiPiecePosition position)
        {
            try
            {
                foreach (ReversiBufferPosition rbp in reversiBuffer.AllBufferPositions)
                {
                    if (position.Equals(rbp.Position)) return true;
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// 获取所有能落子的位置
        /// </summary>
        /// <param name="piece">黑方或白方</param>
        /// <returns>所有位置构成的列表</returns>
        public List<ReversiPiecePosition> GetEnabledPositions()
        {
            if (reversiBuffer != null)
                return reversiBuffer.AllEnabledPositions;
            return new List<ReversiPiecePosition>();
        }

        /// <summary>
        /// 获取落子后所有该翻的棋子
        /// </summary>
        /// <param name="location">落子的坐标</param>
        /// <returns>如果可以在此处落子, 则返回 true.</returns>
        public List<ReversiPiecePosition> GetReversePositions(ReversiPiecePosition position)
        {
            return reversiBuffer.GetReversePositions(position);
        }

        /// <summary>
        /// 重建缓存
        /// </summary>
        private void ResetBuffer()
        {
            reversiBuffer = null;
            if (!IsGameOver)
            {
                reversiBuffer = new ReversiBuffer();

                ReversiPiecePosition tempPosition;
                List<ReversiPiecePosition> tempReverseList;
                int x, y;
                bool isThisPositionEnabled;
                bool haveDiffPiece;
                for (int i = 0; i < ReversiGame.BoardSize; i++)
                {
                    for (int j = 0; j < ReversiGame.BoardSize; j++)
                    {
                        if (GetpieceInBoard(j, i) != ReversiPiece.Empty) continue;
                        tempPosition = new ReversiPiecePosition(j, i);
                        isThisPositionEnabled = false;
                        for (int k = 0; k < 8; k++)
                        {
                            x = j;
                            y = i;
                            haveDiffPiece = false;
                            tempReverseList = new List<ReversiPiecePosition>();
                            while (true)
                            {
                                x += ReversiDirections.Num(k).X;
                                y += ReversiDirections.Num(k).Y;
                                if (GetpieceInBoardWithoutException(x, y) == ReversiPiece.Empty)
                                    break;
                                else if (GetpieceInBoardWithoutException(x, y) == GetReversepiece(CurrentPiece))
                                {
                                    tempReverseList.Add(new ReversiPiecePosition(x, y));
                                    haveDiffPiece = true;
                                }
                                else
                                {
                                    if (!haveDiffPiece) break;
                                    if (!isThisPositionEnabled) reversiBuffer.AddNewEnabledPosition(tempPosition);
                                    isThisPositionEnabled = true;
                                    foreach (ReversiPiecePosition rpp in tempReverseList)
                                    {
                                        reversiBuffer.AllBufferPositions[reversiBuffer.AllBufferPositions.Count - 1].AllReversePositions.Add(rpp);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 设置异步执行落子操作的参数
        /// </summary>
        /// <param name="position"></param>
        /// <param name="movePiece_Confirmed"></param>
        /// <param name="movePiece_Completed"></param>
        public void SetPieceMoveArgs(ReversiPiecePosition position, WhenMovePiece_Confirmed movePiece_Confirmed, WhenMovePiece_Completed movePiece_Completed)
        {
            MovePiece_Position = position;
            MovePiece_Confirmed = movePiece_Confirmed;
            MovePiece_Completed = movePiece_Completed;
            MovePieceHandled = false;
        }
        /// <summary>
        /// 开始执行落子操作
        /// </summary>
        public void PieceMoves()
        {
            if (MovePieceHandled || !IspieceEnabled(MovePiece_Position))
            {
                MovePiece_Confirmed(false, CurrentPiece, null, null);
                MovePiece_Completed(false, false, CurrentPiece);
                return;
            }
            // 保存缓冲区当中的当前落子信息
            ReversiPiece thisPiece = CurrentPiece;
            ReversiPiecePosition thisMovePosition = MovePiece_Position;
            List<ReversiPiecePosition> thisReversePosition = reversiBuffer.GetReversePositions(thisMovePosition);
            if (IsBusy || thisReversePosition == null)
            {
                MovePiece_Confirmed(false, CurrentPiece, null, null);
                MovePiece_Completed(false, false, CurrentPiece);
                return;
            }
            IsBusy = true;
            // 开始落子, 更新棋盘.
            PushState();
            CurrentBoard[thisMovePosition.X, thisMovePosition.Y] = CurrentPiece;
            foreach (ReversiPiecePosition rpp in thisReversePosition)
            {
                CurrentBoard[rpp.X, rpp.Y] = CurrentPiece;
            }
            UpdateState();
            LastPiece = thisPiece;
            LastPosition = thisMovePosition;
            MovePiece_Confirmed(true, thisPiece, thisMovePosition, thisReversePosition);
            // 重建缓冲区
            CurrentPiece = GetReversepiece(CurrentPiece);
            ResetBuffer();
            if (GetEnabledPositions().Count <= 0)
            {
                CurrentPiece = GetReversepiece(CurrentPiece);
                ResetBuffer();
                if (GetEnabledPositions().Count <= 0)
                    IsGameOver = true;
            }
            // 落子完成, 对外宣布并重置落子参数
            IsBusy = false;
            MovePiece_Completed(true, IsGameOver, CurrentPiece);
            MovePiece_Position = null;
            MovePiece_Confirmed = null;
            MovePiece_Completed = null;
            MovePieceHandled = true;
        }
        /// <summary>
        /// 设置悔棋参数
        /// </summary>
        /// <param name="regretPiece_Finished">悔棋失败时的委托, 通常会要求对方执行一次下棋操作.</param>
        public void SetRegretPieceArgs(WhenRegretPiece_Finished regretPiece_Finished)
        {
            RegretPiece_Finished = regretPiece_Finished;
        }
        /// <summary>
        /// 执行悔棋操作 (悔棋1步)
        /// </summary>
        public void RegretPieceMove()
        {
            RegretPieceMove(1);
        }
        /// <summary>
        /// 执行悔棋操作 (悔棋 regretCount 步)
        /// </summary>
        /// <param name="regretCount">悔棋步数</param>
        public void RegretPieceMove(int regretCount)
        {
            if (regretCount > MaxRegretSteps) throw new Exception("需求的悔棋步数大于实际能悔棋的步数!");
            PopState(regretCount);
            ResetBuffer();
            RegretPiece_Finished = null;
        }
        /// <summary>
        /// 执行悔棋操作 (悔棋到自己的上一步棋子)
        /// </summary>
        /// <param name="toOwnPiece">如果为 true, 则悔棋到自己的上一步棋子; 如果为 false, 则悔棋一步.</param>
        public void RegretPieceMove(bool toOwnPiece)
        {
            if (toOwnPiece)
            {
                ReversiPiece ownPiece = CurrentPiece;
                try
                {
                    while (true)
                    {
                        PopState();
                        if (ownPiece == CurrentPiece) break;
                    }
                    ResetBuffer();
                }
                catch
                {
                    ResetBuffer();
                    RegretPiece_Finished(true);
                }
                RegretPiece_Finished = null;
            }
            else
            {
                RegretPieceMove();
            }
        }
        #endregion

        #region 管理棋子
        /// <summary>
        /// 获取棋盘上某点的棋子
        /// </summary>
        /// <param name="x">该点的 x 坐标</param>
        /// <param name="y">该点的 y 坐标</param>
        /// <returns>棋子类型</returns>
        public ReversiPiece GetpieceInBoard(int x, int y)
        {
            CheckpieceValid(x, y);
            return CurrentBoard[x, y];
        }
        /// <summary>
        /// 获取棋盘上某点的棋子
        /// </summary>
        /// <param name="posion">该点的坐标</param>
        /// <returns>棋子类型</returns>
        public ReversiPiece GetpieceInBoard(ReversiPiecePosition position)
        {
            return GetpieceInBoard(position.X, position.Y);
        }
        /// <summary>
        /// 获取棋盘上某点的棋子, 如果该点不在棋盘内, 则返回 Empty.
        /// </summary>
        /// <param name="x">该点的 x 坐标</param>
        /// <param name="y">该点的 y 坐标</param>
        /// <returns>棋子类型</returns>
        public ReversiPiece GetpieceInBoardWithoutException(int x, int y)
        {
            try
            {
                CheckpieceValid(x, y);
            }
            catch
            {
                return ReversiPiece.Empty;
            }
            return CurrentBoard[x, y];
        }
        /// <summary>
        /// 获取棋盘上某点的棋子, 如果该点不在棋盘内, 则返回 Empty.
        /// </summary>
        /// <param name="posion">该点的坐标</param>
        /// <returns>棋子类型</returns>
        public ReversiPiece GetpieceInBoardWithoutException(ReversiPiecePosition position)
        {
            return GetpieceInBoardWithoutException(position.X, position.Y);
        }

        /// <summary>
        /// 获取一个棋子的反棋子, 如果不存在棋子, 则抛出异常.
        /// </summary>
        /// <param name="piece">黑棋或白棋</param>
        /// <returns>白棋或黑棋</returns>
        public static ReversiPiece GetReversepiece(ReversiPiece piece)
        {
            if (piece == ReversiPiece.Black) return ReversiPiece.White;
            else if (piece == ReversiPiece.White) return ReversiPiece.Black;
            else throw new Exception("无法转化一个不存在的棋子");
        }
        /// <summary>
        /// 获取一个棋子的反棋子, 如果不存在棋子, 则返回空棋子.
        /// </summary>
        /// <param name="piece">黑棋或白棋或空</param>
        /// <returns>白棋或黑棋或空</returns>
        public static ReversiPiece TryGetReversepiece(ReversiPiece piece)
        {
            try
            {
                return GetReversepiece(piece);
            }
            catch
            {
                return ReversiPiece.Empty;
            }
        }
        #endregion

        #region 棋局状态
        /// <summary>
        /// 把状态压入棋局栈中记录下来
        /// </summary>
        private void PushState()
        {
            gameStateStack.Push(CurrentBoard, CurrentPiece, LastPiece, LastPosition);
            isRegretEnabled = true;
            MaxRegretSteps++;
        }
        /// <summary>
        /// 从棋局栈中恢复旧的状态
        /// </summary>
        private void PopState()
        {
            PopState(1);
        }
        /// <summary>
        /// 从棋局栈中恢复旧的状态
        /// </summary>
        /// <param name="popCount">恢复到最近的第 popCount 个状态</param>
        private void PopState(int popCount)
        {
            ReversiGameState tempState = null;
            if (popCount < 1)
            {
                throw new Exception("悔棋步数不可以小于 1!");
            }
            for (int t = 0; t < popCount; t++)
            {
                tempState = null;
                if (gameStateStack.IsEmpty) throw new Exception("已经达到悔棋步数极限!");
                tempState = gameStateStack.Pop();
            }
            MaxRegretSteps -= popCount;
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    CurrentBoard[i, j] = tempState.CurrentBoard[i, j];
                }
            }
            CurrentPiece = tempState.CurrentPiece;
            LastPiece = tempState.LastPiece;
            LastPosition = tempState.LastPosition;
            UpdateState();
            if (MaxRegretSteps <= 0) isRegretEnabled = false;
        }
        /// <summary>
        /// 更新棋局状态
        /// </summary>
        private void UpdateState()
        {
            int blackNumber = 0;
            int whiteNumber = 0;
            for (int i = 0; i < ReversiGame.BoardSize; i++)
            {
                for (int j = 0; j < ReversiGame.BoardSize; j++)
                {
                    if (GetpieceInBoard(i, j) == ReversiPiece.Black) blackNumber++;
                    else if (GetpieceInBoard(i, j) == ReversiPiece.White) whiteNumber++;
                }
            }
            BlackPieceNumber = blackNumber;
            WhitePieceNumber = whiteNumber;
            if (blackNumber == 0 || whiteNumber == 0 || blackNumber + whiteNumber == ReversiGame.BoardSize * ReversiGame.BoardSize) IsGameOver = true;
            else { WinSide = ReversiPiece.Empty; IsGameOver = false; }
            if (IsGameOver)
            {
                if (blackNumber < whiteNumber) WinSide = ReversiPiece.White;
                else if (blackNumber > whiteNumber) WinSide = ReversiPiece.Black;
                else WinSide = ReversiPiece.Empty;
            }
        }

        /// <summary>
        /// 判断哪一方赢了
        /// </summary>
        /// <returns>是否赢棋</returns>
        public ReversiPiece isWin()
        {
            return WinSide;
        }
        /// <summary>
        /// 判断某一方是否赢了
        /// </summary>
        /// <param name="piece">需要判断的某一方</param>
        /// <returns>是否赢棋</returns>
        public bool isWin(ReversiPiece piece)
        {
            if (piece == WinSide) return true;
            else return false;
        }
        #endregion
    }
}
