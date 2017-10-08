using System;

namespace Reversi
{
    /// <summary>
    /// 表示棋盘上的一个位置
    /// </summary>
    public class ReversiPiecePosition
    {
        private int x = 0;
        private int y = 0;

        /// <summary>
        /// 横坐标, 通常指棋盘上的第几列.
        /// </summary>
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if (x >= 0 || x < ReversiGame.BoardSize) x = value;
                else throw new Exception("棋子的 x 坐标出现在棋盘外!");
            }
        }
        /// <summary>
        /// 纵坐标, 通常指棋盘上的第几行.
        /// </summary>
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (y >= 0 || y < ReversiGame.BoardSize) y = value;
                else throw new Exception("棋子的 y 坐标出现在棋盘外!");
            }
        }

        /// <summary>
        /// 创建棋盘上的一个位置, 值为 (0, 0).
        /// </summary>
        public ReversiPiecePosition()
        {
            x = 0;
            y = 0;
        }

        /// <summary>
        /// 创建棋盘上的一个位置, 值为 (x, y).
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public ReversiPiecePosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 判断两个位置是否相等
        /// </summary>
        /// <param name="position">另一个位置</param>
        /// <returns>如果相等, 则返回 true.</returns>
        public bool Equals(ReversiPiecePosition position)
        {
            if (x == position.X && y == position.Y) return true;
            else return false;
        }
    }
}
