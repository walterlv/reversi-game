using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
    public class ReversiDirection
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ReversiDirection(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public static class ReversiDirections
    {
        private static ReversiDirection[] directions = new ReversiDirection[8];
        private static bool isInitialized = false;

        private static void Initialize()
        {
            directions[0] = new ReversiDirection(0, -1);
            directions[1] = new ReversiDirection(1, -1);
            directions[2] = new ReversiDirection(1, 0);
            directions[3] = new ReversiDirection(1, 1);
            directions[4] = new ReversiDirection(0, 1);
            directions[5] = new ReversiDirection(-1, 1);
            directions[6] = new ReversiDirection(-1, 0);
            directions[7] = new ReversiDirection(-1, -1);
        }

        public static ReversiDirection Num(int d)
        {
            if (!isInitialized) Initialize();
            return directions[d];
        }
    }
}
