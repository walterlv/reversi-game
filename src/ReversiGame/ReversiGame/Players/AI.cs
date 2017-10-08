using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Reversi;
using ReversiXNAGame.ReversiBoard;
using ReversiXNAGame.Settings;

namespace ReversiXNAGame.Players
{
    internal class AI : Player
    {
        // AI落子等待
        int waitTime;
        int passedTime;
        // AI对象
        ReversiAI reversiAI;
        ReversiPiecePosition AIPosition;
        List<ReversiPiecePosition> AIReversePositions;

        public void SetAIType(int type)
        {
            reversiAI = ReversiAIType.GetAI(type);
            reversiAI.AIInitialize(myPieceColor);
        }

        public AI(Game game, SpriteBatch screenSpriteBatch, Rectangle boardRec, Piece[,] allPieces, ReversiPiece myColor)
            : base(game, screenSpriteBatch, boardRec, allPieces, myColor)
        {
            myType = PlayerTypes.AI;
            Name = "电脑";
            Initialize();
        }

        public override void Initialize()
        {
            waitTime = GameSettings.WaitTime;
            passedTime = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (isMyTurn) passedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (isMyTurn && !isMovingPiece)
            {
                isMovePieceCompleted = false;
                AIPosition = reversiAI.GetNextpiece();
                AIReversePositions = reversiGame.GetReversePositions(AIPosition);
                MovePiece(AIPosition);
            }
            if (isMovePieceCompleted && isMyTurn && passedTime > waitTime)
            {
                passedTime = 0;
                curGame.board.CurrentBoardState = BoardState.FreshGame;
                if (!reversiGame.IsGameOver && reversiGame.CurrentPiece == reversiGame.LastPiece)
                {
                    // TODO: 添加无子可下的显示
                }
                isMyTurn = false;
                isMovingPiece = false;
                isMovePieceCompleted = true;
            }

            base.Update(gameTime);
        }

        public override void MovePiece_Confirmed(bool confirmed, ReversiPiece piece, ReversiPiecePosition position, List<ReversiPiecePosition> positions)
        {
        }
    }
}
