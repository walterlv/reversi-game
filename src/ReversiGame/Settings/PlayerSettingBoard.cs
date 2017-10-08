using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using Reversi;
using ReversiXNAGame.ReversiBoard;
using ReversiXNAGame.Players;
using ReversiXNAGame.Messages;
using Walterlv.ReversiGame.FrameworkInterop;

namespace ReversiXNAGame.Settings
{
    internal enum SettingType
    {
        Start,
        Pause,
    }
    internal enum SettingBoardState
    {
        Showing,
        Show,
        HidingForNewGame,
        HidingForResume,
        HideForNewGame,
        HideForResume,
    }

    internal class PlayerSettingBoard : DrawableGameComponent
    {
        public PlayerTypes[] PlayerType = new PlayerTypes[2];
        ReversiGame reversiGame;
        // 面板状态
        SettingBoardState settingBoardState;
        // 鼠标状态
        MouseState currentMouseState = new MouseState();
        MouseState lastMouseState = new MouseState();
        // 键盘状态
        KeyboardState currentKeyboardState = new KeyboardState();
        KeyboardState lastKeyboardState = new KeyboardState();
        // 设置棋盘
        Rectangle boardRectangle;
        Texture2D board;
        Texture2D settingBoard;
        Piece[] showPieces = new Piece[2];
        Piece[] playerTypePieces = new Piece[4];
        int AIMaxCount = 5, AICount;
        int[] AIIndex = new int[2];
        Piece[,] aiTypePieces;
        int[] chooseIndex = new int[2];
        StartButton startButton;
        SettingType settingType = SettingType.Start;
        public SettingType CurrentType
        {
            set { settingType = value; }
        }

        public PlayerSettingBoard(Rectangle boardRec)
        {
            reversiGame = ReversiGame.CurrentGame;
            boardRectangle = boardRec;

            aiTypePieces = new Piece[2, AIMaxCount];
            AICount = ReversiAIType.GetAINames().Length;
            AIIndex[0] = GameSettings.DefaultAIIndex;
            AIIndex[1] = GameSettings.DefaultAIIndex;
            board = curGame.LoadContent<Texture2D>(@"Images\Board");
            settingBoard = curGame.LoadContent<Texture2D>(@"Images\SettingBoard");
            int pieceSize = boardRec.Width / ReversiGame.BoardSize;
            startButton = CreateChild<StartButton, Rectangle>(new Rectangle(boardRec.X + pieceSize * 3, boardRec.Y + pieceSize * 6, pieceSize * 2, pieceSize));
            startButton.Initialize(settingType);
            showPieces[0] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X + pieceSize * 3, boardRec.Y + pieceSize * 2, pieceSize, pieceSize));
            showPieces[0].PieceState = PieceState.Black;
            showPieces[1] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X + pieceSize * 4, boardRec.Y + pieceSize * 2, pieceSize, pieceSize));
            showPieces[1].PieceState = PieceState.White;
            playerTypePieces[0] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X + pieceSize * 1, boardRec.Y + pieceSize * 3, pieceSize, pieceSize));
            playerTypePieces[0].CurrentDisplay = DisplayState.CanMove;
            chooseIndex[0] = 0;
            PlayerType[0] = PlayerTypes.Human;
            playerTypePieces[1] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X + pieceSize * 2, boardRec.Y + pieceSize * 3, pieceSize, pieceSize));
            playerTypePieces[1].CurrentDisplay = DisplayState.Normal;
            playerTypePieces[2] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X + pieceSize * 5, boardRec.Y + pieceSize * 3, pieceSize, pieceSize));
            playerTypePieces[2].CurrentDisplay = DisplayState.CanMove;
            chooseIndex[1] = 2;
            PlayerType[1] = PlayerTypes.AI;
            playerTypePieces[3] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X + pieceSize * 6, boardRec.Y + pieceSize * 3, pieceSize, pieceSize));
            playerTypePieces[3].CurrentDisplay = DisplayState.Normal;
            for (int i = 0; i < AIMaxCount; i++)
                aiTypePieces[0, i] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X, boardRec.Y + pieceSize * (i + 3), pieceSize, pieceSize));
            for (int i = 0; i < AIMaxCount; i++)
                aiTypePieces[1, i] = CreateChild<Piece, Rectangle>(new Rectangle(boardRec.X + pieceSize * 7, boardRec.Y + pieceSize * (i + 3), pieceSize, pieceSize));
            foreach (Piece piece in showPieces) piece.ForceShow = true;
            foreach (Piece piece in playerTypePieces) piece.ForceShow = true;
            foreach (Piece piece in aiTypePieces) piece.ForceShow = true;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public void Show(SettingType st)
        {
            settingType = st;
            startButton.Initialize(settingType);
            settingBoardState = SettingBoardState.Showing;
        }

        public override void Update(GameTime gameTime)
        {
            if (settingBoardState == SettingBoardState.Showing)
            {
                settingBoardState = SettingBoardState.Show;
            }
            else if (settingBoardState == SettingBoardState.Show)
            {
                if (startButton.StartButtonClicked)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        PlayerSettings.Player[i].Type = PlayerType[i];
                        if (PlayerType[i] == PlayerTypes.AI)
                        {
                            PlayerSettings.Player[i].AIIndex = AIIndex[i];
                        }
                    }
                    settingBoardState = SettingBoardState.HidingForNewGame;
                }
                else if (startButton.ResumeButtonClicked)
                {
                    settingBoardState = SettingBoardState.HidingForResume;
                }
                else
                {
                    // 绘制选择状态
                    foreach (Piece piece in showPieces) piece.CurrentDisplay = DisplayState.Normal;
                    foreach (Piece piece in playerTypePieces) piece.CurrentDisplay = DisplayState.Normal;
                    foreach (int i in chooseIndex) playerTypePieces[i].CurrentDisplay = DisplayState.CanMove;
                    for (int i = 0; i < AICount; i++)
                    {
                        if (chooseIndex[0] == 1) aiTypePieces[0, i].PieceState = PieceState.Black;
                        else aiTypePieces[0, i].PieceState = PieceState.Empty;
                        if (chooseIndex[1] == 2) aiTypePieces[1, i].PieceState = PieceState.White;
                        else aiTypePieces[1, i].PieceState = PieceState.Empty;
                    }
                    if (chooseIndex[0] == 1) aiTypePieces[0, AIIndex[0]].CurrentDisplay = DisplayState.WillReverse;
                    if (chooseIndex[1] == 2) aiTypePieces[1, AIIndex[1]].CurrentDisplay = DisplayState.WillReverse;
                    if (curGame.IsActive)
                    {
                        // 获取鼠标状态
                        lastMouseState = currentMouseState;
                        currentMouseState = Mouse.GetState();
                        ReversiPiecePosition tempPosition = Board.IsMouseInPiece(currentMouseState);
                        // 如果移动鼠标
                        if (tempPosition != null)
                        {
                            // 如果划过的是黑白子
                            if (tempPosition.Y == 2)
                            {
                                if (tempPosition.X == 3) showPieces[0].CurrentDisplay = DisplayState.WillMove;
                                else if (tempPosition.X == 4) showPieces[1].CurrentDisplay = DisplayState.WillMove;
                            }
                            // 如果划过的是玩家类型选择
                            else if (tempPosition.Y == 3)
                            {
                                int index = -1;
                                if (tempPosition.X == 1) index = 0;
                                else if (tempPosition.X == 2) index = 1;
                                else if (tempPosition.X == 5) index = 2;
                                else if (tempPosition.X == 6) index = 3;
                                if (index >= 0) playerTypePieces[index].CurrentDisplay = DisplayState.WillMove;
                            }
                            // 如果划过的是AI类型
                            if ((tempPosition.X == 0 || tempPosition.X == 7) && (tempPosition.Y >= 3 && tempPosition.Y <= 2 + AICount))
                            {
                                if (chooseIndex[0] == 1 && tempPosition.X == 0)
                                {
                                    int index = tempPosition.Y - 3;
                                    aiTypePieces[0, index].CurrentDisplay = DisplayState.WillMove;
                                }
                                if (chooseIndex[1] == 2 && tempPosition.X == 7)
                                {
                                    int index = tempPosition.Y - 3;
                                    aiTypePieces[1, index].CurrentDisplay = DisplayState.WillMove;
                                }
                            }
                            // 如果按下鼠标左键
                            if ((currentMouseState.LeftButton == ButtonState.Pressed) && (lastMouseState.LeftButton == ButtonState.Released))
                            {
                                // 如果点击的是黑白子
                                if (tempPosition.Y == 2)
                                {
                                    if (tempPosition.X == 3)
                                    {
                                        chooseIndex[0] = 0; PlayerType[0] = PlayerTypes.Human;
                                        chooseIndex[1] = 2; PlayerType[1] = PlayerTypes.AI;
                                    }
                                    else if (tempPosition.X == 4)
                                    {
                                        chooseIndex[0] = 1; PlayerType[0] = PlayerTypes.AI;
                                        chooseIndex[1] = 3; PlayerType[1] = PlayerTypes.Human;
                                    }
                                }
                                // 如果点击的是玩家类型选择
                                else if (tempPosition.Y == 3)
                                {
                                    if (tempPosition.X == 1) { chooseIndex[0] = 0; PlayerType[0] = PlayerTypes.Human; }
                                    if (tempPosition.X == 2) { chooseIndex[0] = 1; PlayerType[0] = PlayerTypes.AI; }
                                    if (tempPosition.X == 5) { chooseIndex[1] = 2; PlayerType[1] = PlayerTypes.AI; }
                                    if (tempPosition.X == 6) { chooseIndex[1] = 3; PlayerType[1] = PlayerTypes.Human; }
                                }
                                // 如果点击的是AI类型选择
                                if (tempPosition.X == 0 || tempPosition.X == 7)
                                {
                                    if (chooseIndex[0] == 1 && tempPosition.X == 0) AIIndex[0] = tempPosition.Y - 3;
                                    if (chooseIndex[1] == 2 && tempPosition.X == 7) AIIndex[1] = tempPosition.Y - 3;
                                }
                            }
                        }
                    }
                    foreach (Piece piece in showPieces) piece.Update(gameTime);
                    foreach (Piece piece in playerTypePieces) piece.Update(gameTime);
                    foreach (Piece piece in aiTypePieces) piece.Update(gameTime);
                    startButton.Update(gameTime);
                }
                // 获取键盘状态
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
                // 当按下 Escape 键时
                if (currentKeyboardState.IsKeyUp(Keys.Escape) && lastKeyboardState.IsKeyDown(Keys.Escape))
                {
                    if (settingType == SettingType.Pause) settingBoardState = SettingBoardState.HidingForResume;
                }
            }
            else if (settingBoardState == SettingBoardState.HidingForNewGame)
            {
                // TODO: 隐藏动画
                settingBoardState = SettingBoardState.HideForNewGame;
            }
            else if (settingBoardState == SettingBoardState.HidingForResume)
            {
                // TODO: 隐藏动画
                settingBoardState = SettingBoardState.HideForResume;
            }
            else if (settingBoardState == SettingBoardState.HideForNewGame)
            {
                curGame.State = GameState.StartLoad;
            }
            else if (settingBoardState == SettingBoardState.HideForResume)
            {
                curGame.State = GameState.InGame;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (settingBoardState == SettingBoardState.Showing)
            {
            }
            else if (settingBoardState == SettingBoardState.Show)
            {
                spriteBatch.Draw(board, boardRectangle, Color.White);
                foreach (Piece piece in showPieces) piece.Draw(gameTime);
                foreach (Piece piece in playerTypePieces) piece.Draw(gameTime);
                foreach (Piece piece in aiTypePieces) piece.Draw(gameTime);
                spriteBatch.Draw(settingBoard, boardRectangle, Color.White);
                startButton.Draw(gameTime);
            }
            else if (settingBoardState == SettingBoardState.HidingForNewGame || settingBoardState == SettingBoardState.HidingForResume)
            {
                spriteBatch.Draw(board, boardRectangle, Color.White);
                spriteBatch.Draw(settingBoard, boardRectangle, Color.White);
            }
            else
            {
                spriteBatch.Draw(board, boardRectangle, Color.White);
                spriteBatch.Draw(settingBoard, boardRectangle, Color.White);
            }
            base.Draw(gameTime);
        }
    }
}
