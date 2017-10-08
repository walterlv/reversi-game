using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
    public class ReversiAIPriority:ReversiAI
    {
        private List<int> priority;
        public override ReversiPiecePosition GetNextpiece()
        {
            enabledPositionList = reversiGame.GetEnabledPositions();
            priority = new List<int>();
            foreach (ReversiPiecePosition p in enabledPositionList)
            {
                priority.Add(Priority(p));
            }
            int maxPriority=0;
            int maxPriorityIndex = -1;
            for (int i = 0; i < priority.Count; i++)
            {
                if (priority[i] > maxPriority)
                {
                    maxPriority = priority[i];
                    maxPriorityIndex = i;
                }
            }
            priority = null;
            return enabledPositionList[maxPriorityIndex];
        }


        private int Priority(ReversiPiecePosition position)
        {
            return 0;
        }
    }



}
