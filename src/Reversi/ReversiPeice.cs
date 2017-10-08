using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
    /// <summary>
    /// 表示棋盘上一个位置可能有的三种状态
    /// </summary>
    public enum ReversiPiece
    {
        /// <summary>
        /// 暂未下棋子
        /// </summary>
        Empty = 0x00,
        /// <summary>
        /// 黑子
        /// </summary>
        Black = 0x01,
        /// <summary>
        /// 白子
        /// </summary>
        White = 0x02,
    }
}
