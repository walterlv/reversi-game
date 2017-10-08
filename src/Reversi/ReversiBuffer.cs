using System;
using System.Collections.Generic;

namespace Reversi
{
    public class ReversiBufferPosition
    {
        public bool IsInitialized { get; private set; }
        public ReversiPiecePosition Position;
        public List<ReversiPiecePosition> AllReversePositions;

        public ReversiBufferPosition(ReversiPiecePosition position)
        {
            SetPosition(position);
        }

        public void SetPosition(ReversiPiecePosition position)
        {
            IsInitialized = false;
            Position = position;
            AllReversePositions = new List<ReversiPiecePosition>();
        }

        public void SetAllReversePositions(List<ReversiPiecePosition> positions)
        {
            AllReversePositions = positions;
            IsInitialized = true;
        }
    }

    public class ReversiBuffer
    {
        public List<ReversiBufferPosition> AllBufferPositions { get; private set; }
        public List<ReversiPiecePosition> AllEnabledPositions { get; private set; }

        public ReversiBuffer()
        {
            AllBufferPositions = new List<ReversiBufferPosition>();
            AllEnabledPositions = new List<ReversiPiecePosition>();
        }

        public List<ReversiPiecePosition> GetReversePositions(ReversiPiecePosition setPosition)
        {
            foreach (ReversiBufferPosition rbp in AllBufferPositions)
            {
                if (rbp.Position.Equals(setPosition)) return rbp.AllReversePositions;
            }
            return null;
        }

        public void AddNewEnabledPosition(ReversiPiecePosition position)
        {
            AllBufferPositions.Add(new ReversiBufferPosition(position));
            AllEnabledPositions.Add(position);
        }

        public bool IsEmpty()
        {
            if (AllBufferPositions == null || AllBufferPositions.Count <= 0) return true;
            else return false;
        }

        

        /*public void Add(ReversipiecePosition position)
        {
            AllBufferPositions.Add(new ReversiBufferPosition(position));
        }

        public bool Contain(ReversipiecePosition position)
        {
            foreach (ReversiBufferPosition p in AllBufferPositions)
            {
                if (p.Position.X == position.X && p.Position.Y == position.Y)
                    return true;
            }
            return false;
        }

        public void Update()
        {
        }

        public void SetNewBufferPositions(ReversipiecePosition position)
        {
            foreach (ReversiBufferPosition p in AllBufferPositions)
            {
                if (p.Position.X == position.X && p.Position.Y == position.Y)
                {
                    p.nextBufferPosition = new List<ReversiBufferPosition>();
                    AllBufferPositions = null;
                    AllBufferPositions = p.nextBufferPosition;
                    Add(position);
                    break;
                }
            }
        }*/
    }
}
