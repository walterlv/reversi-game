using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

using Reversi;

namespace ReversiDebugUI
{
    /// <summary>
    /// 表示等待状态
    /// </summary>
    enum WaitingState
    {
        NotWaiting = 0,
        WaitingMovePieceConfirmed = 1,
        WaitingMovePieceCompleted = 2,
        StillWaitingMovePieceCompleted = 3,
        WaitingRegretPieceCompleted = 4,
    }
    public delegate void WhenGameOver(ReversiGame game);
    /// <summary>
    /// BoardControl.xaml 的交互逻辑
    /// </summary>
    public partial class BoardControl : UserControl
    {
        public BoardControl()
        {
            InitializeComponent();
        }

        private bool isInitialized = false;
        public bool ShowDebugTile = false;
        public bool IsRegretPieceEnabled = true;
        /// <summary>
        /// 表示当前棋局, 如果游戏需要重新开始, 只需要创建一个新的对象
        /// </summary>
        ReversiGame reversiGame;
        /// <summary>
        /// 人工下棋时的棋子类型 (全人工和全智能时无视此字段)
        /// </summary>
        ReversiPiece HumanPieceType;
        /// <summary>
        /// 人工智能对象, 如果游戏需要重新开始, 一定要在新建棋局对象之后新建此类的一个实例
        /// </summary>
        ReversiAI reversiAI;
        /// <summary>
        /// 用于等待下棋的定时器
        /// </summary>
        DispatcherTimer timer;
        /// <summary>
        /// 下棋线程
        /// </summary>
        Thread movePieceThread;

        WhenGameOver whenGameOver;

        private WaitingState waitingState = WaitingState.NotWaiting;
        private bool IsMovePieceConfirmed = false;
        private bool IsMovePieceCompleted = false;
        //private bool IsRegretPieceCompleted = false;
        private bool isUserTurn = false;
        private bool IsUserTurn
        {
            get
            {
                return isUserTurn;
            }
            set
            {
                isUserTurn = value;
                if (value) boardRectangle.Visibility = Visibility.Collapsed;
                else boardRectangle.Visibility = Visibility.Visible;
            }
        }
        private bool MovePieceNeedStop = false;

        public int PlayerMoveTime = 10;
        public int AIMoveTime = 500;
        public int AIWaitTime = 200;
        public int RegretTime = 200;

        #region 与用户界面的交互
        PieceControl[,] pieceControls = new PieceControl[ReversiGame.BoardSize, ReversiGame.BoardSize];
        PieceControl lastEnablePiece;
        PieceControl lastMovedPiece;
        private void InitialControls()
        {
            for (int i = 0; i < ReversiGame.BoardSize; i++)
            {
                for (int j = 0; j < ReversiGame.BoardSize; j++)
                {
                    foreach (PieceControl pc in pieceGrid.Children)
                    {
                        if (pc.Uid == i.ToString() + j.ToString())
                        {
                            pieceControls[i, j] = pc;
                            break;
                        }
                    }
                }
            }
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
        }
        /// <summary>
        /// 与用户界面的交互 (通过界面上棋子的Uid查找棋盘上棋子位置)
        /// </summary>
        private ReversiPiecePosition UidToPosition(string uid)
        {
            int x = int.Parse(uid.Substring(0, 1));
            int y = int.Parse(uid.Substring(1, 1));
            return new ReversiPiecePosition(x, y);
        }
        #endregion

        #region 初始化或者更新操作
        /// <summary>
        /// 初始化用户界面
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// 更新用户界面. 如果需要在事件执行过程中就更新界面, 需要执行此方法.
        /// </summary>
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            new DispatcherOperationCallback(delegate(object f)
            {
                ((DispatcherFrame)f).Continue = false;
                return null;
            }
                ), frame);
            Dispatcher.PushFrame(frame);
        }
        /// <summary>
        /// 执行棋局初始化操作 (包括新建棋局实例, 新建人工智能实例. 必要时执行自动下棋操作.)
        /// </summary>
        public void InitializeGame(ReversiPiece userPiece, int AIIndex, WhenGameOver gameOver)
        {
            InitialControls();
            whenGameOver = gameOver;
            reversiGame = ReversiGame.CreateANewGame();
            HumanPieceType = userPiece;
            UpdateWindow();
            reversiAI = ReversiAIType.GetAI(AIIndex);
            changeWaitingState(WaitingState.NotWaiting);
            if (HumanPieceType == ReversiPiece.White)
            {
                reversiAI.AIInitialize(ReversiPiece.Black);
                MovePiece();
                DoEvents();
            }
            else
            {
                reversiAI.AIInitialize(ReversiPiece.White);
            }
            ShowNextEnabledPieces();
            isInitialized = true;
            IsUserTurn = true;
        }
        /// <summary>
        /// 更新窗口, 把棋局上的棋盘复制到用户界面的棋盘上.
        /// </summary>
        private void UpdateWindow()
        {
            for (int i = 0; i < ReversiGame.BoardSize; i++)
            {
                for (int j = 0; j < ReversiGame.BoardSize; j++)
                {
                    pieceControls[i, j].CurrentPiece = reversiGame.CurrentBoard[i, j];
                }
            }
            if (lastMovedPiece != null) lastMovedPiece.IsLastPiece = false;
            DoEvents();
        }
        /// <summary>
        /// 结束游戏
        /// </summary>
        /// <returns>如果游戏本身已结束, 则返回 true; 如果中途中断结束游戏, 则返回 false.</returns>
        public bool EndGame()
        {
            if (lastMovedPiece != null) lastMovedPiece.IsLastPiece = false;
            if (reversiGame.IsGameOver)
            {
                whenGameOver(reversiGame);
                return true;
            }
            else return false;
        }
        #endregion

        #region 异步执行下棋操作
        /// <summary>
        /// 执行下棋操作. 没有参数, 交给 AI 完成.
        /// </summary>
        private void MovePiece()
        {
            changeWaitingState(WaitingState.WaitingMovePieceConfirmed);
            reversiGame.SetPieceMoveArgs(reversiAI.GetNextpiece(), MovePiece_Confirmed, MovePiece_Completed);
            if (movePieceThread != null && movePieceThread.IsAlive) movePieceThread.Abort();
            movePieceThread = new Thread(new ThreadStart(reversiGame.PieceMoves));
            movePieceThread.Start();
        }
        /// <summary>
        /// 执行下棋操作. 有位置参数, 通常由用户落子确定位置.
        /// </summary>
        /// <param name="position">用户落子的位置</param>
        private void MovePiece(ReversiPiecePosition position)
        {
            IsUserTurn = false;
            changeWaitingState(WaitingState.WaitingMovePieceConfirmed);
            reversiGame.SetPieceMoveArgs(position, MovePiece_Confirmed, MovePiece_Completed);
            if (movePieceThread != null && movePieceThread.IsAlive) movePieceThread.Abort();
            movePieceThread = new Thread(new ThreadStart(reversiGame.PieceMoves));
            movePieceThread.Start();
        }
        /// <summary>
        /// 此方法只能被回调. 当向棋局实例发出下棋请求后, 如果此请求被允许, 则会回调这个方法, 并传回下棋参数.
        /// 此方法只为更快地响应用户的下棋操作.
        /// </summary>
        /// <param name="piece">应下的棋子颜色</param>
        /// <param name="position">应下的棋子位置</param>
        /// <param name="positions">应该翻转的棋子</param>
        private void MovePiece_Confirmed(bool confirmed, ReversiPiece piece, ReversiPiecePosition position, List<ReversiPiecePosition> positions)
        {
            if (confirmed) IsMovePieceConfirmed = true;
            else MovePieceNeedStop = true;
        }
        /// <summary>
        /// 此方法只能被回调. 
        /// 当向棋局实例发出下棋请求后, 如果此请求被拒绝, 则会回调这个方法, 并在参数中告知下棋失败.
        /// 当整个下棋操作执行完成后, 会回调此方法. 在收到此方法之前, 应该停止新的下棋操作.
        /// </summary>
        /// <param name="isSuccess">指示落子是否成功</param>
        /// <param name="isGameOver">指示落子成功后游戏是否因此而结束</param>
        /// <param name="nextPiece">当落子成功并且游戏未结束, 则告知下一步应该下的棋子是哪种颜色的.</param>
        private void MovePiece_Completed(bool isSuccess, bool isGameOver, ReversiPiece nextPiece)
        {
            if (isSuccess) IsMovePieceCompleted = true;
            else MovePieceNeedStop = true;
        }
        /// <summary>
        /// 悔棋
        /// </summary>
        private void RegretMovePiece()
        {
            try
            {
                changeWaitingState(WaitingState.WaitingRegretPieceCompleted);
                reversiGame.SetRegretPieceArgs(RegretPiece_Finished);
                reversiGame.RegretPieceMove(true);
                UpdateWindow();
                ShowNextEnabledPieces();
                lastMovedPiece.IsLastPiece = false;
                lastMovedPiece = pieceControls[reversiGame.LastPosition.X, reversiGame.LastPosition.Y];
                lastMovedPiece.IsLastPiece = true;
            }
            catch
            {
                UpdateWindow();
            }
        }
        /// <summary>
        /// 此方法只能被回调. 
        /// 当向棋局实例发出悔棋请求后, 悔棋完成时会回调这个方法, 并在参数中告知悔棋完成.
        /// </summary>
        /// <param name="MoveRequired">如果为 true, 则表示悔棋已到最开始的棋局, 需要人工智能下一步棋.</param>
        private void RegretPiece_Finished(bool MoveRequired)
        {
            //IsRegretPieceCompleted = true;
            if (MoveRequired && reversiGame.CurrentPiece != HumanPieceType) MovePiece();
        }
        /// <summary>
        /// 清除所有显示的下一步棋子
        /// </summary>
        private void ClearAllNextPieces()
        {
            if (ShowDebugTile)
            {
                for (int i = 0; i < ReversiGame.BoardSize; i++)
                    for (int j = 0; j < ReversiGame.BoardSize; j++)
                        pieceControls[i, j].DebugLabel = false;
            }
        }
        /// <summary>
        /// 显示下一步可下棋子
        /// </summary>
        private void ShowNextEnabledPieces()
        {
            if (ShowDebugTile)
            {
                ClearAllNextPieces();
                foreach (ReversiPiecePosition rpp in reversiGame.GetEnabledPositions())
                    pieceControls[rpp.X, rpp.Y].DebugLabel = true;
            }
        }
        #endregion

        #region 动态调整当前棋局状态
        /// <summary>
        /// 更改当前的等待状态
        /// </summary>
        /// <param name="newState">新的状态</param>
        private void changeWaitingState(WaitingState newState)
        {
            if (newState == WaitingState.NotWaiting)
            {
                string s;
                if (reversiGame.CurrentPiece == ReversiPiece.Black) s = "请下黑子";
                else if (reversiGame.CurrentPiece == ReversiPiece.White) s = "请下白子";
                else s = "请下棋";
                stateControl.CurrentStateText = s;
                IsUserTurn = true;
            }
            else if (newState == WaitingState.WaitingMovePieceConfirmed)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, PlayerMoveTime);
                timer.Start();
            }
            else if (newState == WaitingState.WaitingMovePieceCompleted)
            {
                if (!stateControl.IsVeryBusy) stateControl.CurrentStateText = "计算机思考中...";
                timer.Interval = new TimeSpan(0, 0, 0, 0, AIMoveTime / 2);
                timer.Start();
            }
            else if (newState == WaitingState.StillWaitingMovePieceCompleted)
            {
                stateControl.IsVeryBusy = true;
                DoEvents();
                timer.Interval = new TimeSpan(0, 0, 0, 0, AIWaitTime);
                timer.Start();
            }
            else if (newState == WaitingState.WaitingRegretPieceCompleted)
            {
                stateControl.CurrentStateText = "悔棋中, 请稍等...";
                DoEvents();
                timer.Interval = new TimeSpan(0, 0, 0, 0, RegretTime);
                timer.Start();
            }
            waitingState = newState;
        }
        /// <summary>
        /// 当时钟到达时执行
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            // 如果需要停止下棋过程 (通常是因为下棋失败)
            if (MovePieceNeedStop)
            {
                MovePieceNeedStop = false;
                timer.Stop();
                changeWaitingState(WaitingState.NotWaiting);
                return;
            }
            // 当当前未等待任何事时
            if (waitingState == WaitingState.NotWaiting)
            {
                changeWaitingState(WaitingState.NotWaiting);
            }
            // 当当前正在等待下棋确认时
            else if (waitingState == WaitingState.WaitingMovePieceConfirmed)
            {
                // 如果下棋已被确认
                if (IsMovePieceConfirmed)
                {
                    timer.Stop();
                    // 确保只有到玩家下棋时才会显示下一步可下的棋子
                    if (reversiGame.CurrentPiece != HumanPieceType)
                    {
                        UpdateWindow();
                        ClearAllNextPieces();
                        if (lastMovedPiece != null) lastMovedPiece.IsLastPiece = false;
                        lastMovedPiece = pieceControls[reversiGame.LastPosition.X, reversiGame.LastPosition.Y];
                        lastMovedPiece.IsLastPiece = true;
                    }
                    IsMovePieceConfirmed = false;
                    changeWaitingState(WaitingState.WaitingMovePieceCompleted);
                }
            }
            // 当当前正在等待下棋完成时
            else if (waitingState == WaitingState.WaitingMovePieceCompleted || waitingState == WaitingState.StillWaitingMovePieceCompleted)
            {
                if (!reversiGame.IsBusy && IsMovePieceCompleted)
                {
                    timer.Stop();
                    // 当游戏结束时, 通知父容器
                    if (reversiGame.IsGameOver)
                    {
                        changeWaitingState(WaitingState.NotWaiting);
                        UpdateWindow();
                        EndGame();
                    }
                    else
                    {
                        // 确保当玩家下棋时能显示上一步已下棋子和下一步可下棋子
                        if (reversiGame.CurrentPiece == HumanPieceType)
                        {
                            UpdateWindow();
                            if (lastMovedPiece != null) lastMovedPiece.IsLastPiece = false;
                            lastMovedPiece = pieceControls[reversiGame.LastPosition.X, reversiGame.LastPosition.Y];
                            lastMovedPiece.IsLastPiece = true;
                        }
                        // 如果下完棋后不是玩家下棋, 则自动调用AI再下一步
                        if (reversiGame.CurrentPiece == ReversiGame.GetReversepiece(HumanPieceType))
                        {
                            UpdateWindow();
                            MovePiece();
                            changeWaitingState(WaitingState.WaitingMovePieceConfirmed);
                        }
                        // 如果下完棋后轮到玩家下棋, 则显示下一步棋子
                        else
                        {
                            ShowNextEnabledPieces();
                            IsMovePieceCompleted = false;
                            changeWaitingState(WaitingState.NotWaiting);
                        }
                    }
                }
                // 如果规定时间内未完成下棋, 则使界面转到长时间等待状态
                else
                {
                    if (waitingState != WaitingState.StillWaitingMovePieceCompleted) changeWaitingState(WaitingState.StillWaitingMovePieceCompleted);
                }
            }
            // 当当前正在等待悔棋完成时
            else if (waitingState == WaitingState.WaitingRegretPieceCompleted)
            {
                changeWaitingState(WaitingState.NotWaiting);
            }
        }
        #endregion

        #region 用户执行下棋操作
        /// <summary>
        /// 当用户按下鼠标按钮时执行人工下棋操作
        /// </summary>
        private void PieceControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!reversiGame.IsBusy)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    PieceControl pc = (PieceControl)sender;
                    MovePiece(UidToPosition(pc.Uid));
                }
                else if (IsRegretPieceEnabled && e.RightButton == MouseButtonState.Pressed)
                {
                    RegretMovePiece();
                }
            }
        }
        private void PieceControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isInitialized && !reversiGame.IsBusy)
            {
                foreach (ReversiPiecePosition rpp in reversiGame.GetEnabledPositions())
                {
                    pieceControls[rpp.X, rpp.Y].SetPreview = false;
                }
                PieceControl pc = (PieceControl)sender;
                if (reversiGame.IspieceEnabled(UidToPosition(pc.Uid)))
                {
                    pc.SetPreview = true;
                    lastEnablePiece = pc;
                }
            }
        }
        #endregion
    }
}
