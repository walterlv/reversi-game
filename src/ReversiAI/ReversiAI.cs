using System;
using System.Collections.Generic;
namespace Reversi
{
    /// <summary>
    /// 表示黑白棋 AI.
    /// </summary>
    public abstract class ReversiAI
    {
        protected ReversiGame reversiGame = ReversiGame.CurrentGame;
        protected ReversiPiece AIColor;
        protected ReversiPiecePosition LastOpponentpiecePosition;
        protected List<ReversiPiecePosition> enabledPositionList;

        /// <summary>
        /// 初始化 AI
        /// </summary>
        /// <param name="reversiBoard">初始棋盘</param>
        /// <param name="aIColor">AI 子颜色</param>
        public void AIInitialize(ReversiPiece aIColor)
        {
            AIColor = aIColor;
        }

        /// <summary>
        /// 设置对手的上一步棋子的位置
        /// </summary>
        /// <param name="position">位置</param>
        public void SetLastOpponentpiece(ReversiPiecePosition position)
        {
            LastOpponentpiecePosition = position;
        }

        public abstract ReversiPiecePosition GetNextpiece();
    }
}
