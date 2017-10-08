using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Reversi;

namespace ReversiDebugUI
{
    /// <summary>
    /// pieceControl.xaml 的交互逻辑
    /// </summary>
    public partial class PieceControl : UserControl
    {
        public PieceControl()
        {
            InitializeComponent();
        }

        private static SolidColorBrush WhitePreviewColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xBB, 0xBB, 0xBB));
        private static SolidColorBrush BlackPreviewColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x44));


        private ReversiPiece currentpiece = ReversiPiece.Empty;
        public ReversiPiece CurrentPiece
        {
            get
            {
                return currentpiece;
            }
            set
            {
                if (value == ReversiPiece.White) pieceEllipse.Fill = Brushes.White;
                else if (value == ReversiPiece.Black) pieceEllipse.Fill = Brushes.Black;
                else pieceEllipse.Fill = Brushes.Transparent;
            }
        }

        public bool DebugLabel
        {
            set
            {
                if (value) pieceRectangle.Fill = Brushes.Brown;
                else pieceRectangle.Fill = Brushes.Transparent;
            }
        }

        public bool SetPreview
        {
            set
            {
                if (value)
                {
                    if (ReversiGame.CurrentGame.CurrentPiece == ReversiPiece.White) pieceEllipse.Fill = WhitePreviewColor;
                    else if (ReversiGame.CurrentGame.CurrentPiece == ReversiPiece.Black) pieceEllipse.Fill = BlackPreviewColor;
                }
                else pieceEllipse.Fill = Brushes.Transparent;
            }
        }

        public bool IsLastPiece
        {
            set
            {
                if (value)
                {
                    if (ReversiGame.CurrentGame.CurrentPiece == ReversiPiece.White) pieceEllipse.Stroke = Brushes.OrangeRed;
                    else if (ReversiGame.CurrentGame.CurrentPiece == ReversiPiece.Black) pieceEllipse.Stroke = Brushes.Red;
                }
                else pieceEllipse.Stroke = Brushes.Transparent;
            }
        }
    }
}
