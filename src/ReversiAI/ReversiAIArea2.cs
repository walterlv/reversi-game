using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
    class ReversiAIArea2:ReversiAI
    {

        private List<ReversiPiecePosition> positionsSafe;   //安全区域的棋子
        private List<ReversiPiecePosition> positionsB;      //B区域的棋子
        private List<ReversiPiecePosition> positionsA;      //A区域的棋子    
        private List<ReversiPiecePosition> positionsX;      //X区域的棋子
        private List<ReversiPiecePosition> positionsC;      //C区域的棋子
        private List<int> positionsCPriority; //C区域的棋子的优先等级
        private List<ReversiPiecePosition> positionsCorner; //角区域的棋子

        private List<ReversiPiecePosition> positionsNext;   //下一步棋子
      
        public override ReversiPiecePosition GetNextpiece()
        {
            //获得可下棋子
            this.enabledPositionList = reversiGame.GetEnabledPositions();

            if (enabledPositionList == null) throw new Exception("计算AI下一步棋子时,棋盘上无AI可下棋子");

            //声明一个即将作为返回结果的棋子
            ReversiPiecePosition result;

            //划分区域
            DivideArea();

            //安全区域
            if((result=SafeArea())!=null) return result;

            //B区域
            if ((result = BArea()) != null) return result;
           
            //Region--安全区域和B区域均无子可下
            result = new ReversiPiecePosition();
            ReversiPiecePosition resultA = new ReversiPiecePosition();
            byte tmpA = AArea(result);
            //A区域
            if (tmpA == 3) return result;
            else if (tmpA == 2)
            {
                resultA.X = result.X; //如果下A位出现被翻转,记下此A位的位置
                resultA.Y = result.Y;
            }
            //C区域 
            if ((CArea(result)) == 2) return result;
            //A区域
            if (tmpA == 2)
                return resultA;
            //EndRegion--安全区域和B区域均无子可下

            return null;
        }

        #region 判断棋子的位置
        /// <summary>
        /// 判断棋子是否在x位
        /// </summary>
        /// <param name="position">棋子位置</param>
        /// <returns>0表示否, 1表示1,1 2表示6,1 3表示1,6 4表示6,6</returns>
        private byte IsX(ReversiPiecePosition position)
        {
            if (position.X == 1 && position.Y == 1)
                return 1;
            else if (position.X == 6 && position.Y == 6)
                return 4;
            else if (position.X == 1 && position.Y == 6)
                return 3;
            else if (position.X == 6 && position.Y == 1)
                return 2;
            else return 0;
        }
        /// <summary>
        /// 判断是否在边角上
        /// </summary>
        /// <param name="position"></param>
        /// <returns>0代表不在,1代表角,2代表c,3代表a,4代表b</returns>
        private byte IsInEdgeOrCorner(ReversiPiecePosition position)
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
            return 0; 
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
        /// 找出B位棋子附近的A位棋子
        /// </summary>
        /// <param name="p">B位棋子的位置</param>
        /// <returns>传入非B位则返回空</returns>
        private ReversiPiecePosition CNearA(ReversiPiecePosition p)
        {
            if (p.X == 2 && p.Y == 0) return new ReversiPiecePosition(1, 0);
            else if (p.X == 5 && p.Y == 0) return new ReversiPiecePosition(6, 0);
            else if (p.X == 0 && p.Y == 2) return new ReversiPiecePosition(0, 1);
            else if (p.X == 0 && p.Y == 5) return new ReversiPiecePosition(0, 6);
            else if (p.X == 2 && p.Y == 7) return new ReversiPiecePosition(1, 7);
            else if (p.X == 5 && p.Y == 7) return new ReversiPiecePosition(6, 7);
            else if (p.X == 7 && p.Y == 2) return new ReversiPiecePosition(7, 1);
            else if (p.X == 7 && p.Y == 5) return new ReversiPiecePosition(7, 6);
            else return null;
        }
        /// <summary>
        /// 找出C位棋子附近的角位棋子
        /// </summary>
        /// <param name="p">C位棋子的位置</param>
        /// <returns>传入非C位则返回空</returns>
        private ReversiPiecePosition CornerNearC(ReversiPiecePosition p)
        {
            if (p.X == 1 && p.Y == 0) return new ReversiPiecePosition(0, 0);
            else if (p.X == 6 && p.Y == 0) return new ReversiPiecePosition(7, 0);
            else if (p.X == 0 && p.Y == 1) return new ReversiPiecePosition(0, 0);
            else if (p.X == 0 && p.Y == 6) return new ReversiPiecePosition(0, 7);
            else if (p.X == 1 && p.Y == 7) return new ReversiPiecePosition(0, 7);
            else if (p.X == 6 && p.Y == 7) return new ReversiPiecePosition(7, 7);
            else if (p.X == 7 && p.Y == 1) return new ReversiPiecePosition(7, 0);
            else if (p.X == 7 && p.Y == 6) return new ReversiPiecePosition(7, 7);
            else return null;
        }
        /// <summary>
        /// 找出C位棋子同边上非附近的角位棋子
        /// </summary>
        /// <param name="p">C位棋子的位置</param>
        /// <returns>传入非C位则返回空</returns>
        private ReversiPiecePosition RemoteSameEdgeCornerOfC(ReversiPiecePosition p)
        {
            if (p.X == 1 && p.Y == 0) return new ReversiPiecePosition(7, 0);
            else if (p.X == 6 && p.Y == 0) return new ReversiPiecePosition(0, 0);
            else if (p.X == 0 && p.Y == 1) return new ReversiPiecePosition(0, 7);
            else if (p.X == 0 && p.Y == 6) return new ReversiPiecePosition(0, 0);
            else if (p.X == 1 && p.Y == 7) return new ReversiPiecePosition(7, 7);
            else if (p.X == 6 && p.Y == 7) return new ReversiPiecePosition(0, 7);
            else if (p.X == 7 && p.Y == 1) return new ReversiPiecePosition(7, 7);
            else if (p.X == 7 && p.Y == 6) return new ReversiPiecePosition(7, 0);
            else return null;
        }
        /// <summary>
        /// 找出C位位置的同边C位位置
        /// </summary>
        /// <param name="p">C位位置 </param>
        /// <returns>同边C位位置</returns>
        private ReversiPiecePosition SameEdgeCOfC(ReversiPiecePosition p)
        {
            if (p.X == 1 && p.Y == 0) return new ReversiPiecePosition(6, 0);
            else if (p.X == 6 && p.Y == 0) return new ReversiPiecePosition(1, 0);
            else if (p.X == 0 && p.Y == 1) return new ReversiPiecePosition(0, 6);
            else if (p.X == 0 && p.Y == 6) return new ReversiPiecePosition(0, 1);
            else if (p.X == 1 && p.Y == 7) return new ReversiPiecePosition(6, 7);
            else if (p.X == 6 && p.Y == 7) return new ReversiPiecePosition(1, 7);
            else if (p.X == 7 && p.Y == 1) return new ReversiPiecePosition(7, 6);
            else if (p.X == 7 && p.Y == 6) return new ReversiPiecePosition(7, 1);
            else return null;
        }
        private ReversiPiecePosition ANearC(ReversiPiecePosition p)
        {
            if (p.X == 1 && p.Y == 0) return new ReversiPiecePosition(2, 0);
            else if (p.X == 6 && p.Y == 0) return new ReversiPiecePosition(5, 0);
            else if (p.X == 0 && p.Y == 1) return new ReversiPiecePosition(0, 2);
            else if (p.X == 0 && p.Y == 6) return new ReversiPiecePosition(0, 5);
            else if (p.X == 1 && p.Y == 7) return new ReversiPiecePosition(2, 7);
            else if (p.X == 6 && p.Y == 7) return new ReversiPiecePosition(5, 7);
            else if (p.X == 7 && p.Y == 1) return new ReversiPiecePosition(7, 2);
            else if (p.X == 7 && p.Y == 6) return new ReversiPiecePosition(7, 5);
            else return null;
        }
        #endregion 判断棋子的位置



        #region 区域下棋
        /// <summary>
        /// 划分区域(可下棋子当中,属于不同的区域(角,X,C,A,B,安全区域),分别区分开来
        /// </summary>
        private void DivideArea()
        {
            positionsSafe = new List<ReversiPiecePosition>();
            positionsB = new List<ReversiPiecePosition>();
            positionsA = new List<ReversiPiecePosition>();
            positionsC = new List<ReversiPiecePosition>();
            positionsX = new List<ReversiPiecePosition>();
            positionsCorner = new List<ReversiPiecePosition>();
            byte tmp;
            foreach (ReversiPiecePosition p in enabledPositionList)
            {
                tmp = IsInEdgeOrCorner(p);
                if (tmp == 1)
                    positionsCorner.Add(p);
                else if (tmp == 2)
                    positionsC.Add(p);
                else if (tmp == 3)
                    positionsA.Add(p);
                else if (tmp == 4)
                    positionsB.Add(p);
                else if (IsX(p) > 0)
                    positionsX.Add(p);
                else positionsSafe.Add(p);
            }
        }
        /// <summary>
        /// 随机返回一个安全区域的棋子
        /// </summary>
        /// <returns>没有返回null,有则返回</returns>
        private ReversiPiecePosition SafeArea()
        {
            if (positionsSafe.Count > 0)
                return positionsSafe[new Random().Next(positionsSafe.Count)];
            else return null;
        }
        /// <summary>
        /// 随机返回一个B区域的棋子
        /// </summary>
        /// <returns>没有返回null,有则返回</returns>
        private ReversiPiecePosition BArea()
        {
            if (positionsB.Count > 0)
                return positionsB[new Random().Next(positionsB.Count)];
            else return null;
        }
        /// <summary>
        /// A区域可下棋子当中,找出一个适合的棋子,结果保存在result中
        /// </summary>
        /// <param name="result"></param>
        /// <returns>0表示可下棋子当中无A位的,1表示可下棋子中有A位但会被占角,2表示下后会被翻转,3表示可下</returns>
        private byte AArea(ReversiPiecePosition result)
        {
           
            if (positionsA.Count == 0) return 0;
            bool changedA=false;
            for (int i = 0; i < positionsA.Count; i++)
            {
                byte tmp = ConsiderA(positionsA[i]);
                if (tmp == 0)
                    continue;
                else if (tmp == 1)
                {
                    result.X = positionsA[i].X;
                    result.Y = positionsA[i].Y;
                    changedA = true;
                    continue;
                }
                result.X = positionsA[i].X;
                result.Y = positionsA[i].Y;
                return 3;
            }
            if (changedA) return 2;
            return 1;
        }
        /// <summary>
        /// C区域可下棋子当中,找出一个适合的棋子,结果保存在result中
        /// </summary>
        /// <param name="result"></param>
        /// <returns>0表示可下棋子当中无C位的,1表示可下棋子中有C位但会被立即占角或者将被占角,2表示可下并且result为最优</returns>
        private byte CArea(ReversiPiecePosition result)
        {
            if (positionsC.Count == 0) return 0;
            CPrority();
            int Max = -1;
            int MaxIndex=-1;
            for (int i = 0; i < positionsCPriority.Count; i++)
            {
                if (positionsCPriority[i] > Max)
                {
                    Max = positionsCPriority[i];
                    MaxIndex = i;
                }
            }
            if (Max > 1)
            {
                result.X = positionsC[MaxIndex].X;
                result.Y = positionsC[MaxIndex].Y;
                return 2;
            }
            else 
            {
                result.X = positionsC[MaxIndex].X;
                result.Y = positionsC[MaxIndex].Y;
                return 1;
            }
        }

        #endregion 区域下棋

        /// <summary>
        /// 确定C区域棋子的优先等级
        /// </summary>
        private void CPrority()
        {
            positionsCPriority = new List<int>();
            for (int i = 0; i < positionsC.Count; i++)
            {
                positionsCPriority.Add((int)ConsiderC(positionsC[i]));
            }
        }
        /// <summary>
        /// 判断下A位棋子是否合适
        /// </summary>
        /// <param name="position">A位棋子</param>
        /// <returns>0表示立马丢角,1表示会被同边上C位翻转,2表示不被占角也不会被翻转</returns>
        private byte ConsiderA(ReversiPiecePosition position)
        {
            if (IsInEdgeOrCorner(position) != 3) throw new Exception("调用ConsiderA方法时,使用非A位的参数");
            TryMoveOne(position);
            List<ReversiPiecePosition> tmp1 = reversiGame.GetEnabledPositions();
            ReversiPiecePosition tmpC1 = CNearA(position); //其同边上附近的C位
            ReversiPiecePosition tmpCorner1 = CornerNearC(tmpC1);
            ReversiPiecePosition tmpCorner2 = RemoteSameEdgeCornerOfC(tmpC1);

            for (int i = 0; i < tmp1.Count; i++)
            {
                //判断是否丢失同边上的角
                if ((tmp1[i].X == tmpCorner1.X && tmp1[i].Y == tmpCorner1.Y)
                        || (tmp1[i].X == tmpCorner2.X && tmp1[i].Y == tmpCorner2.Y))
                {
                    RegretOne();
                    return 0;
                }
            }
            //判断是否会被其同边上的C位翻转
            ReversiPiecePosition tmpC2 = SameEdgeCOfC(tmpC1); //其同边上的另一个C位
            List<ReversiPiecePosition> tmp2 = reversiGame.GetEnabledPositions();
            for(int i=0;i<tmp2.Count;i++)
            {
                if ((tmp2[i].X == tmpC1.X && tmp2[i].Y == tmpC1.Y)
                   || (tmp2[i].X == tmpC2.X && tmp2[i].Y == tmpC2.Y))
                {
                    TryMoveOne(tmp2[i]);
                    if (reversiGame.CurrentBoard[position.X, position.Y] != this.AIColor)
                    {
                        RegretOne();
                        RegretOne();
                        return 1;
                    }
                    RegretOne();
                }
            }

            RegretOne();
            return 2;
        }
        /// <summary>
        /// 判断C位是否可下
        /// </summary>
        /// <param name="position">c位的棋子</param>
        /// <returns>0表示会被占角,1表示将会被占角,2不会被占角,3表示不会被占角且会占到其同边上的非附近的角</returns>
        private byte ConsiderC(ReversiPiecePosition position)
        {
            if (IsInEdgeOrCorner(position) != 2) throw new Exception("使用ConsiderC方法是,参数为非C位");
            TryMoveOne(position);
            List<ReversiPiecePosition> tmp1 = reversiGame.GetEnabledPositions();
            ReversiPiecePosition tmpA = ANearC(position);
            ReversiPiecePosition tmpCorner1 = CornerNearC(position);
            ReversiPiecePosition tmpCorner2 = RemoteSameEdgeCornerOfC(position);

            for (int i = 0; i < tmp1.Count; i++)
            {
                //判断是否立即与其同边的丢角
                if ((tmp1[i].X == tmpCorner1.X && tmp1[i].Y == tmpCorner1.Y)
                    || (tmp1[i].X == tmpCorner2.X && tmp1[i].Y == tmpCorner2.Y))
                {
                    RegretOne();
                    return 0;
                }
                //判断是否将要丢失其附近的角
                else if (tmpA.X == tmp1[i].X && tmpA.Y == tmp1[i].Y)
                {
                    TryMoveOne(tmpA);
                    List<ReversiPiecePosition> tmp2 = reversiGame.GetEnabledPositions();
                    for (int j = 0; j < tmp2.Count; j++)
                    {
                        if(tmp2[i].X == tmpCorner2.X && tmp2[i].Y == tmpCorner2.Y)
                        {
                            RegretOne();
                            RegretOne();
                            return 1;
                        }
                    }
                    RegretOne();
                }
            }
            if (reversiGame.CurrentBoard[tmpCorner1.X, tmpCorner1.Y] != ReversiPiece.Empty)
            {
                TryMoveOne(tmp1[new Random().Next(tmp1.Count)]);
                List<ReversiPiecePosition> tmp2 = reversiGame.GetEnabledPositions();
                for (int i = 0; i < tmp2.Count; i++)
                {
                    //判断是否可以占其同边上非附近的角
                    if (tmp2[i].X == tmpCorner2.X && tmp2[i].Y == tmpCorner2.Y)
                    {
                        RegretOne();
                        RegretOne();
                        return 3;
                    }
                }
                RegretOne();
            }
            RegretOne();
            return 2;
        }


        #region 下子悔棋以及判断丢角情况
        /// <summary>
        /// 尝试下子(用TryMoveOne后必须要用到RegretOne)
        /// </summary>
        /// <param name="p">棋子</param>
        private void TryMoveOne(ReversiPiecePosition p)
        {
            reversiGame.SetPieceMoveArgs(p, MovePieceConfirmed, MovePieceCompleted);
            reversiGame.PieceMoves();
        }
        /// <summary>
        /// 悔棋(在用TryMoveOne后,悔棋恢复棋盘)
        /// </summary>
        /// <param name="p"></param>
        private void RegretOne()
        {
            reversiGame.RegretPieceMove();
        }
        /// <summary>
        /// 判断下子后,对方立即可下角
        /// </summary>
        /// <param name="position">棋子的位置</param>
        /// <returns>布尔值</returns>
        private bool LoseCorner()
        {
            List<ReversiPiecePosition> tmp = reversiGame.GetEnabledPositions();
            foreach (ReversiPiecePosition p in tmp)
            {
                if (IsCorner(p))
                    return true;
            }
            return false;
        }
        private void MovePieceConfirmed(bool confirmed, ReversiPiece piece, ReversiPiecePosition position, List<ReversiPiecePosition> positions)
        {
        }
        private void MovePieceCompleted(bool IsSuccess, bool isGameOver, ReversiPiece nextPiece)
        {
        }
        #endregion 下子悔棋以及判断丢角,被翻转等情况
    }
}
