using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Reversi
{
    public class ReversiAIMost:ReversiAI
    {
        public override ReversiPiecePosition GetNextpiece()
        {
            enabledPositionList = reversiGame.GetEnabledPositions();
            int maxPiecesChanged = 0;
            int flag = 0;
            int tmp=0;
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                if (IsCorner(enabledPositionList[i]))
                    return enabledPositionList[i];
                tmp = this.NumberOfChangedPieces(enabledPositionList[i]);
                if (tmp > maxPiecesChanged)
                {
                    maxPiecesChanged = tmp;
                    flag = i;
                }
            }
            return enabledPositionList[flag];
        }

        private int NumberOfChangedPieces(ReversiPiecePosition location)
        {
            int number=0;
            number += MostRemoteChangedpiece(location, 1, 0).X - location.X;
            number += MostRemoteChangedpiece(location, 1, -1).X - location.X;
            number += location.Y - MostRemoteChangedpiece(location, 0, -1).Y;
            number += location.Y - MostRemoteChangedpiece(location, -1, -1).Y;
            number += location.X - MostRemoteChangedpiece(location, -1, 0).X;
            number += MostRemoteChangedpiece(location, -1, 1).Y - location.Y;
            number += MostRemoteChangedpiece(location, 0, 1).Y - location.Y;
            number += MostRemoteChangedpiece(location, 1, 1).Y - location.Y;
            return number;
        }

        private ReversiPiecePosition MostRemoteChangedpiece(ReversiPiecePosition location, int directionX, int directionY)
        {
            
            while (location.X + directionX < ReversiGame.BoardSize && location.X + directionX>-1
                && location.Y + directionY < ReversiGame.BoardSize && location.Y + directionY > -1
                && reversiGame.CurrentBoard[location.X + directionX, location.Y + directionY] != ReversiPiece.Empty
                && reversiGame.CurrentBoard[location.X + directionX, location.Y + directionY] != this.AIColor)
            {
                if (directionX > 0)
                    directionX = directionX + 1;
                else if (directionX < 0)
                    directionX = directionX - 1;
                if (directionY > 0)
                    directionY = directionY + 1;
                else if (directionY < 0)
                    directionY = directionY - 1;
            }
            if (location.X + directionX == ReversiGame.BoardSize || location.X + directionX == -1
                || location.Y + directionY == ReversiGame.BoardSize || location.Y + directionY == -1)
                return new ReversiPiecePosition(location.X, location.Y);
            else if (reversiGame.CurrentBoard[location.X + directionX, location.Y + directionY] == ReversiPiece.Empty)
                return new ReversiPiecePosition(location.X, location.Y);
            else
            {
                if (directionX > 0) directionX = directionX - 1;
                else if (directionX < 0) directionX = directionX + 1;
                if (directionY > 0) directionY = directionY - 1;
                else if (directionY < 0) directionY = directionY + 1;
                return new ReversiPiecePosition(location.X + directionX, location.Y + directionY);
            }
        }

        private bool IsCorner(ReversiPiecePosition pos)
        {
            if ((pos.X == 0 && pos.Y == 0) || (pos.X == 7 && pos.Y == 7)
                || (pos.X == 0 && pos.Y == 7) || (pos.X == 7 && pos.Y == 0))
                return true;
            else return false;
        }
    }
}
