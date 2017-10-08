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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

using Reversi;

namespace ReversiDebugUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Storyboard ShowStartPageStory;
        Storyboard HideStartPageStory;

        #region 初始化或者更新操作
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
        /// 初始化用户界面
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowStartPageStory = (Storyboard)FindResource("ShowStartPage");
            HideStartPageStory = (Storyboard)FindResource("HideStartPage");
            foreach (string aiName in ReversiAIType.GetAINames())
            {
                AIType.Items.Add(aiName);
            }
            // TODO 修改此处值可以修改默认使用的 AI
            AIType.SelectedIndex = 1;
        }
        #endregion

        #region 控制游戏流程
        /// <summary>
        /// 点击开始按钮开始游戏
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AIType.SelectedIndex < 0) errorText.Text = "请选择您想要挑战的人工智能";
            else
            {
                try
                {
                    boardControl.PlayerMoveTime = int.Parse(playerTime.Text);
                    boardControl.AIMoveTime = int.Parse(aiMoveTime.Text);
                    boardControl.AIWaitTime = int.Parse(aiWaitTime.Text);
                    boardControl.RegretTime = int.Parse(regretTime.Text);
                }
                catch
                {
                    errorTextDebugTime.Visibility = Visibility.Visible;
                    return;
                }
                errorTextDebugTime.Visibility = Visibility.Collapsed;
                errorText.Text = "";
                CreateANewGame();
            }
        }
        /// <summary>
        /// 创建一个新游戏
        /// </summary>
        private void CreateANewGame()
        {
            boardControl.ShowDebugTile = (bool)showDebugOn.IsChecked;
            boardControl.IsRegretPieceEnabled = (bool)regretEnabled.IsChecked;

            ReversiPiece playPiece;
            if (playBlack.IsChecked == true) playPiece = ReversiPiece.Black;
            else playPiece = ReversiPiece.White;

            boardControl.InitializeGame(playPiece, AIType.SelectedIndex, GameOver);

            HideStartPageStory.Begin();
        }
        /// <summary>
        /// 当动画效果播放完毕时, 隐藏首页
        /// </summary>
        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            startPage.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 结束一个游戏
        /// </summary>
        private void EndGame()
        {
            startPage.Visibility = Visibility.Visible;
            ShowStartPageStory.Begin();
        }
        /// <summary>
        /// 当手动按下结束按钮时
        /// </summary>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要退出游戏吗？棋局将不会被保存。" + Environment.NewLine + "当前黑白比分：" + ReversiGame.CurrentGame.BlackPieceNumber + ":" + ReversiGame.CurrentGame.WhitePieceNumber, "退出黑白棋", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                boardControl.EndGame();
                EndGame();
            }
        }
        /// <summary>
        /// 此函数被回调, 当游戏结束时被回调
        /// </summary>
        /// <param name="reversiGame">游戏结束时的棋局</param>
        private void GameOver(ReversiGame reversiGame)
        {
            DoEvents();
            MessageBox.Show("游戏结束! 黑白比分: " + reversiGame.BlackPieceNumber + ":" + reversiGame.WhitePieceNumber + Environment.NewLine + "点击\"确定\"按钮重新开始...", "游戏结束");
            reversiGame = null;
            EndGame();
        }
        #endregion


    }
}
