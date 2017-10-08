using System;
using System.Collections.Generic;

namespace Reversi
{
    public class ReversiGameState
    {
        public ReversiPiece[,] CurrentBoard;
        public ReversiPiece CurrentPiece;
        public ReversiPiece LastPiece;
        public ReversiPiecePosition LastPosition;

        public ReversiGameState()
        {
        }
        public ReversiGameState(ReversiPiece[,] board, ReversiPiece piece, ReversiPiece lastPiece, ReversiPiecePosition lastPosition)
        {
            CurrentBoard = new ReversiPiece[ReversiGame.BoardSize, ReversiGame.BoardSize];
            for (int i = 0; i < ReversiGame.BoardSize; i++)
            {
                for (int j = 0; j < ReversiGame.BoardSize; j++)
                {
                    CurrentBoard[i, j] = board[i, j];
                }
            }
            CurrentPiece = piece;
            LastPiece = lastPiece;
            LastPosition = new ReversiPiecePosition(lastPosition.X, lastPosition.Y);
        }
    }
    public class RecersiGameStateStack
    {
        Stack<ReversiGameState> reversiGameState;

        public bool IsEmpty { get; private set; }

        public RecersiGameStateStack()
        {
            reversiGameState = new Stack<ReversiGameState>(ReversiGame.BoardSize * ReversiGame.BoardSize - 4);
            IsEmpty = true;
        }
        public void Push(ReversiPiece[,] board, ReversiPiece piece, ReversiPiece lastPiece, ReversiPiecePosition lastPosition)
        {
            reversiGameState.Push(new ReversiGameState(board, piece, lastPiece, lastPosition));
            IsEmpty = false;
        }
        public ReversiGameState Pop()
        {
            if (reversiGameState.Count == 1) IsEmpty = true;
            return reversiGameState.Pop();
        }
    }
}
