using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Reversi
{
    public class ReversiAIArea:ReversiAI
    {
        #region 私有变量
        private bool CLostCorner=false;  //暂时未用到
        private bool ALostCorner = false; //暂时未用到
        private bool ALostC = false;
        private bool GotEdge = false;
        private bool GotThird = false;
        private int ALostCIndex = -1;
        #endregion 私有变量

        /// <summary>
        /// 判断下一步AI该走的棋子
        /// </summary>
        /// <returns>棋子的位置</returns>
        public override ReversiPiecePosition GetNextpiece()
        {
            enabledPositionList = reversiGame.GetEnabledPositions();
            if (enabledPositionList == null) return null;
            ReversiPiecePosition result;
            ReversiPiecePosition resultAC=new ReversiPiecePosition();

            if ((result = Corner()) != null)
                return result;
            #region 安全区域
            if(GotEdge)
            {
                #region 倒数第二,第三圈
                if (GotThird)
                {
                    if ((result = SecondRect()) != null)
                        return result;
                    else if ((result = ThirdRectNotX()) != null)
                        return result;
                }
                else
                {
                    if ((result = ThirdRectNotX()) != null)
                    {
                        GotThird = true;
                        return result;
                    }
                    else if ((result = SecondRect()) != null)
                        return result;
                }
                #endregion 倒数第二第三圈
                if ((result = B()) != null)
                    return result;
            }
            else
            {
                if ((result = B()) != null)
                {
                    GotEdge = true;
                    return result;
                }
                #region 倒数第二,第三圈
                if (GotThird)
                {
                    if ((result = SecondRect()) != null)
                        return result;
                    else if ((result = ThirdRectNotX()) != null)
                        return result;
                }
                else
                {
                    if ((result = ThirdRectNotX()) != null)
                    {
                        GotThird = true;
                        return result;
                    }
                    else if ((result = SecondRect()) != null)
                        return result;
                }
                #endregion 倒数第二第三圈
            }
            //倒数第二第三圈,加边上B位
            #endregion 安全区域


            #region 收尾区域
            if ((result = A()) != null)
                return result;
            else if ((result = C()) != null)
            {
                ALostC = false;
                return result;
            }
            else if (ALostC)
            {
                ALostC = false;
                return enabledPositionList[ALostCIndex];
            }
            else if ((result = X()) != null)
                return result;
            return enabledPositionList[(new Random()).Next(enabledPositionList.Count)];
            #endregion 收尾区域
        }

        #region 判断棋子位置
        /// <summary>
        /// 判断棋子是否在倒数第二圈,如果在并且判断其是否在角
        /// </summary>
        /// <param name="position">棋子的位置</param>
        /// <returns>0代表不在,1表示在角,2表示在边</returns>
        private byte IsInSecondRect(ReversiPiecePosition position)
        {
            if ((position.X == 2 || position.X == 5) && position.Y > 1 && position.Y < 6)
            {
                int tmp = position.X + position.Y;
                if (tmp == 4 || tmp == 7 || tmp == 10)
                    return 1;
                else return 2;
            }
            else if ((position.Y == 2 || position.Y == 5) && position.X > 1 && position.X < 6)
            {
                int tmp = position.X + position.Y;
                if (tmp == 4 || tmp == 7 || tmp == 10)
                    return 1;
                else return 2;
            }
            return 0;
        }
        /// <summary>
        /// 判读棋子是否在倒数第三圈,如果在并且判断其是否在角
        /// </summary>
        /// <param name="position">棋子的位置</param>
        /// <returns>0代表不在,1代表在角,2代表在a1,3代表在边</returns>
        private byte IsInThirdRect(ReversiPiecePosition position)
        {
            if ((position.X == 1 || position.X == 6) && position.Y > 0 && position.Y < 7)
            {
                int tmp = position.X + position.Y;
                if (tmp == 2 || tmp == 7 || tmp == 12)
                    return 1;
                else if(tmp==3 || tmp==6 || tmp==8 || tmp== 11)
                    return 2;
                else return 3;
            }
            else if ((position.Y == 1 || position.Y == 6) && position.X > 0 && position.X < 7)
            {
                int tmp = position.X + position.Y;
                if (tmp == 2 || tmp == 7 || tmp == 12)
                    return 1;
                else if(tmp==3 || tmp==6 || tmp==8 || tmp== 11)
                    return 2;
                else return 3;
            }
            return 0;
        }
        /// <summary>
        /// 判断是否在边上
        /// </summary>
        /// <param name="position"></param>
        /// <returns>0代表不在,1代表角,2代表c,3代表a,4代表b</returns>
        private byte IsInEdge(ReversiPiecePosition position)
        {
            if ((position.X == 0 || position.X == 7) && position.Y > -1 && position.Y < 8)
            {
                int tmp = position.X + position.Y;
                if (tmp == 0 || tmp == 14 || tmp == 7)
                    return 1;
                else if (tmp == 1 || tmp == 6 || tmp == 8 || tmp == 13)
                    return 2;
                else if (tmp == 2 || tmp == 5 || tmp == 9 || tmp == 12)
                    return 3;
                else return 4;
            }
            else if ((position.Y == 0 || position.Y == 7) && position.X > -1 && position.X < 8)
            {
                int tmp = position.X + position.Y;
                if (tmp == 0 || tmp == 14 || tmp == 7)
                    return 1;
                else if (tmp == 1 || tmp == 6 || tmp == 8 || tmp == 13)
                    return 2;
                else if (tmp == 2 || tmp == 5 || tmp == 9 || tmp == 12)
                    return 3;
                else return 4;
            }
            return 0; ;
        }
        /// <summary>
        /// 判断是否为角
        /// </summary>
        /// <param name="position">棋子的位置</param>
        /// <returns>布尔值</returns>
        private bool IsCorner(ReversiPiecePosition position)
        {
            if ((position.X == 0 && position.Y == 0) || (position.X == 7 && position.Y == 7)
                || (position.X == 0 && position.Y == 7) || (position.X == 7 && position.Y == 0))
                return true;
            else return false;
        }
        /// <summary>
        /// 判断棋子是否在x位
        /// </summary>
        /// <param name="position">棋子位置</param>
        /// <returns>0表示否, 1表示1,1 2表示6,1 3表示1,6 4表示6,6</returns>
        private byte IsX(ReversiPiecePosition position)
        {
            if (position.X == 1 && position.Y == 1)
                return 1;
            else if(position.X == 6 && position.Y == 6)
                return 4;
            else if(position.X == 1 && position.Y == 6)
                return 3;
            else if(position.X == 6 && position.Y == 1)
                return 2;
            else return 0;
        }
        /// <summary>
        /// 找出c位附近的A位
        /// </summary>
        /// <param name="positon">c位的位置</param>
        /// <returns></returns>
        private ReversiPiecePosition CNearA(ReversiPiecePosition positon)
        {
            ReversiPiecePosition result = new ReversiPiecePosition();
            if (positon.X == 2 && positon.Y == 0)
            {
                return new ReversiPiecePosition(1, 0);
            }
            else if (positon.X == 0 && positon.Y == 2)
            {
                return new ReversiPiecePosition(0, 1);
            }
            else if (positon.X == 5 && positon.Y == 0)
            {
                return new ReversiPiecePosition(6, 0);
            }
            else if (positon.X == 0 && positon.Y == 5)
            {
                return new ReversiPiecePosition(0, 6);
            }
            else if (positon.X == 2 && positon.Y == 7)
            {
                return new ReversiPiecePosition(1, 7);
            }
            else if (positon.X == 7 && positon.Y == 2)
            {
                return new ReversiPiecePosition(7, 1);
            }
            else if (positon.X == 7 && positon.Y == 5)
            {
                return new ReversiPiecePosition(7, 6);
            }
            else if (positon.X == 5 && positon.Y == 7)
            {
                return new ReversiPiecePosition(6, 7);
            }
            else return null;
        }
        #endregion

        #region 区域判断
        /// <summary>
        /// 当前可下棋子当中,是否有角可下
        /// </summary>
        /// <returns>棋子的位置</returns>
        private ReversiPiecePosition Corner()
        {
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                if (IsCorner(enabledPositionList[i]))
                    return enabledPositionList[i];
            }
            return null;
        }
        /// <summary>
        /// 在可下的棋子当中,查找是否有棋子在倒数第二圈上
        /// </summary>
        /// <returns>返回棋子的位置,空表示没有,优先考虑角存在则返回角的位置</returns>
        private ReversiPiecePosition SecondRect()
        {

            bool InEdge=false;
            int flag=-1;
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                byte tmp;
                tmp = IsInSecondRect(enabledPositionList[i]);
                if (tmp == 1)
                    return enabledPositionList[i];
                if (tmp == 2)
                {
                    InEdge = true;
                    flag = i;
                }
            }
            if (InEdge) return enabledPositionList[flag];
            else return null;
        }
        /// <summary>
        /// 在当前可下棋子当中,查找是否有棋子在倒数第三圈上(不包括X位)
        /// </summary>
        /// <returns>null表示没有,有就返回,优先考虑a1位</returns>
        private ReversiPiecePosition ThirdRectNotX()
        {
            bool InEdge=false;
            int flag = -1;
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                byte tmp=IsInThirdRect(enabledPositionList[i]);
                if (tmp == 3)
                {
                    InEdge = true;
                    flag = i;

                }
                else if (tmp == 2)
                    return enabledPositionList[i];
            }
            if(InEdge) return enabledPositionList[flag];
            else return null;
        }
        /// <summary>
        /// 当前可下棋子中,判断是否有棋子在B位上
        /// </summary>
        /// <returns>没有返回null,有就返回</returns>
        private ReversiPiecePosition B()
        {
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                byte tmp = IsInEdge(enabledPositionList[i]);
                if (tmp == 4)
                    return enabledPositionList[i];
            }
            return null;
        }
        /// <summary>
        /// 判断当前棋子当中,是否有C位(如果C位下后会被占边,也会返回空,IsCDanger的值变为True
        /// </summary>
        /// <returns></returns>
        private ReversiPiecePosition C()
        {
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                if (IsInEdge(enabledPositionList[i]) == 2)
                {
                    if (!CornerLost(enabledPositionList[i]))
                        return enabledPositionList[i];
                    else
                        CLostCorner = true;
                }
            }
            return null;
        }
        /// <summary>
        /// 判断当前棋子当中,是否有可下A位的棋子
        /// </summary>
        /// <returns>安全可下返回,无返回空,丢角,丢C则返回空,修改ALostCorner,ALostC的值</returns>
        private ReversiPiecePosition A()
        {
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                if (IsInEdge(enabledPositionList[i])==3)
                {
                    byte tmp =IsAOK(enabledPositionList[i]);
                    if (tmp == 0)
                        return enabledPositionList[i];
                    else if (tmp == 1)
                        ALostCorner = true;
                    else if (tmp == 2)
                    {
                        ALostC = true;
                        ALostCIndex = i;
                    }
                    else
                    {
                        ALostCorner = true;
                        ALostC = true;
                        ALostCIndex = i;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 判断当前棋子当中,是否有X位可下
        /// </summary>
        /// <returns>有X可下时,当X位附近角位己方,则返回可下X位;有X可下时,当可以舍角取边,则返回可下X位;其他返回空</returns>
        private ReversiPiecePosition X()
        {
            for (int i = 0; i < enabledPositionList.Count; i++)
            {
                byte tmp = IsX(enabledPositionList[i]);
                if (tmp==1)
                {
                    if (reversiGame.CurrentBoard[0, 0] == this.AIColor)
                        return enabledPositionList[i];
                    else if(reversiGame.CurrentBoard[0, 0] == ReversiPiece.Empty)
                    {
                        if (reversiGame.CurrentBoard[0, 1] == ReversiPiece.Empty || reversiGame.CurrentBoard[1, 0] == ReversiPiece.Empty)
                            return enabledPositionList[i];
                    }
                }
                else if (tmp == 2)
                {
                    if (reversiGame.CurrentBoard[7, 0] == this.AIColor)
                        return enabledPositionList[i];
                    else if (reversiGame.CurrentBoard[7, 0] == ReversiPiece.Empty)
                    {
                        if (reversiGame.CurrentBoard[6, 0] == ReversiPiece.Empty || reversiGame.CurrentBoard[7, 1] == ReversiPiece.Empty)
                            return enabledPositionList[i];
                    }
                }
                else if (tmp == 3)
                {
                    if (reversiGame.CurrentBoard[0, 7] == this.AIColor)
                        return enabledPositionList[i];
                    else if (reversiGame.CurrentBoard[0, 7] == ReversiPiece.Empty)
                    {
                        if (reversiGame.CurrentBoard[0, 6] == ReversiPiece.Empty || reversiGame.CurrentBoard[1, 7] == ReversiPiece.Empty)
                            return enabledPositionList[i];
                    }
                }
                else if (tmp == 4)
                {
                    
                    if (reversiGame.CurrentBoard[7, 7] == this.AIColor)
                        return enabledPositionList[i];
                    else if (reversiGame.CurrentBoard[7, 7] == ReversiPiece.Empty)
                    {
                        if (reversiGame.CurrentBoard[7, 6] == ReversiPiece.Empty || reversiGame.CurrentBoard[6, 7] == ReversiPiece.Empty)
                            return enabledPositionList[i];
                    }
                }
            }
            return null;
        }
        #endregion 区域判断

        #region 特殊方法
        /// <summary>
        /// 判断下子后是否丢角
        /// </summary>
        /// <param name="position">棋子的位置</param>
        /// <returns>布尔值</returns>
        private bool CornerLost(ReversiPiecePosition position)
        {
            reversiGame.SetPieceMoveArgs(position, MovePieceConfirmed, MovePieceCompleted);
            reversiGame.PieceMoves();
            List<ReversiPiecePosition> tmp = reversiGame.GetEnabledPositions();
            foreach (ReversiPiecePosition p in tmp)
            {
                if (IsCorner(p))
                {
                    reversiGame.RegretPieceMove();
                    return true;
                }
            }
            reversiGame.RegretPieceMove();
            return false;
        }
        /// <summary>
        /// 判断下A是否合适,禁止传入A位
        /// </summary>
        /// <param name="position">棋子的位置</param>
        /// <returns>0表示合适,1表示会被占角,2表示会丢A附近的C,3表示会丢A附近的C</returns>
        private byte IsAOK(ReversiPiecePosition position)
        {
            bool cornerLost = false;
            bool cLost=false;
            ReversiPiecePosition tmpC=CNearA(position);
            reversiGame.SetPieceMoveArgs(position, MovePieceConfirmed, MovePieceCompleted);
            reversiGame.PieceMoves();
            List<ReversiPiecePosition> tmp = reversiGame.GetEnabledPositions();
            foreach (ReversiPiecePosition p in tmp)
            {
                if (IsCorner(p))
                    cornerLost = true;
                if (tmpC.X == p.X && tmpC.Y == p.Y)
                    cLost = true;
            }
            reversiGame.RegretPieceMove();
            if (cornerLost && (!cLost))
                return 1;
            else if ((!cornerLost) && cLost)
                return 2;
            else if (cornerLost && cLost)
                return 3;
            return 0;
        }
        private void MovePieceConfirmed(bool confirmed, ReversiPiece piece, ReversiPiecePosition position, List<ReversiPiecePosition> positions)
        {
        }
        private void MovePieceCompleted(bool IsSuccess, bool isGameOver, ReversiPiece nextPiece)
        {
        }
        #endregion 特殊方法
    }
}
