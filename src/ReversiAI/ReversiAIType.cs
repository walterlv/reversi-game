using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
    public static class ReversiAIType
    {
        public static string[] GetAINames()
        {
            string[] names = {"最多子", "区域","区域2"};
            return names;
        }
        public static ReversiAI GetAI(int level)
        {
            
            if (level == 0) return new ReversiAIMost();
            else if (level == 1) return new ReversiAIArea();
            else if (level == 2) return new ReversiAIArea2();
            else return null;
        }
    }
}
