using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.Models;
using UnityEngine;

namespace Arycs_Fe.FindPath
{
    public class PathFinding
    {
        #region Delegate/Event

        public delegate void OnStepDelegate(PathFinding search);

        public event OnStepDelegate onStep;

        #endregion

        public MapGraph Map { get; }

        /// <summary>
        /// 开放列表
        /// </summary>
        public List<CellData> Reachable { get; } = new List<CellData>();

        /// <summary>
        /// 关闭列表
        /// </summary>
        public List<CellData> Explored { get; } = new List<CellData>();

        /// <summary>
        /// 结果
        /// </summary>
        public List<CellData> Result { get; } = new List<CellData>();

        private Vector2 m_Range;

        public Vector2 range
        {
            get { return m_Range; }
        }

        private CellData m_StartCell;
        public CellData EndCell { get; private set; }
        public CellData CurrentCell { get; private set; }

        private bool m_Finished;

        /// <summary>
        /// 迭代次数
        /// </summary>
        public int SearchCount { get; private set; } = 0;

        private IHowToFind m_HowToFind;

        private MoveConsumption m_MoveConsumption;

        // 最大迭代次数
        public int m_MaxSearchCount = 2000;

        #region Constructor

        public PathFinding(MapGraph map)
        {
            Map = map;
        }

        #endregion

        /// <summary>
        /// 搜寻下一次，return finished
        /// </summary>
        /// <returns></returns>
        private bool FindNext()
        {
            //已有结果
            if (Result.Count > 0)
            {
                return true;
            }

            //选择节点 
            CurrentCell = m_HowToFind.ChoseCell(this);

            //判断是否搜索结束
            if (m_HowToFind.IsFinishedOnChose(this))
            {
                //如果结束，建立结果
                m_HowToFind.BuildResult(this);
                return true;
            }

            //当前选择的节点不为null
            if (CurrentCell != null)
            {
                for (int i = 0; i < CurrentCell.adjacents.Count; i++)
                {
                    //是否可以加入到开放集中
                    if (m_HowToFind.CanAddAdjacentToReachable(this, CurrentCell.adjacents[i]))
                    {
                        Reachable.Add(CurrentCell.adjacents[i]);
                    }
                }
            }

            return false;
        }

        private bool SearchRangeInternal()
        {
            while (!m_Finished)
            {
                SearchCount++;
                m_Finished = FindNext();
                if (!m_Finished && onStep != null)
                {
                    onStep(this);
                }

                if (SearchCount >= m_MaxSearchCount)
                {
                    Debug.LogError("Search is timeout. MaxCount: " + m_MaxSearchCount.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 寻找移动范围
        /// </summary>
        /// <param name="howToFind">FindRange</param>
        /// <param name="start">起始位置</param>
        /// <param name="movePoint">移动点数</param>
        /// <returns></returns>
        public bool SearchMoveRange(IHowToFind howToFind, CellData start, float movePoint, MoveConsumption consumption)
        {
            if (howToFind == null || start == null || movePoint < 0)
            {
                return false;
            }

            Reset();

            m_HowToFind = howToFind;
            m_MoveConsumption = consumption;

            m_StartCell = start;
            m_StartCell.ResetAStar();
            m_Range.y = movePoint;

            Reachable.Add(m_StartCell);

            return SearchRangeInternal();
        }

        /// <summary>
        /// 搜寻攻击范围
        /// </summary>
        /// <param name="howToFind">寻路方法</param>
        /// <param name="start">开始Cell信息</param>
        /// <param name="minRange">最小攻击距离</param>
        /// <param name="maxRange">最大攻击距离</param>
        /// <returns></returns>
        public bool SearchAttackRange(IHowToFind howToFind, CellData start, int minRange, int maxRange,
            bool useEndCell = false)
        {
            if (howToFind == null || start == null || minRange < 1 || maxRange < minRange)
            {
                return false;
            }

            Reset();

            m_HowToFind = howToFind;
            m_Range = new Vector2(minRange,maxRange);
            
            //在重置时，不重置 '父亲节点'
            //其一 ： 没有用到
            //其二 ： 二次查找时不破坏路径，否则路径将被破坏
            if (useEndCell)
            {
                EndCell = start;
                CurrentCell.h = 0f;
                Reachable.Add(EndCell);
            }
            else
            {
                m_StartCell = start;
                m_StartCell.h = 0f;
                Reachable.Add(m_StartCell);
            }

            return SearchRangeInternal();
        }

        /// <summary>
        /// 搜寻路径
        /// </summary>
        /// <param name="howToFind"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool SearchPath(IHowToFind howToFind, CellData start, CellData end, MoveConsumption consumption)
        {
            if (howToFind == null || start == null || end == null)
            {
                return false;
            }

            Reset();

            m_HowToFind = howToFind;
            m_MoveConsumption = consumption;
            m_StartCell = start;
            m_StartCell.ResetAStar();
            EndCell = end;
            EndCell.ResetAStar();


            Reachable.Add(m_StartCell);
            m_StartCell.h = m_HowToFind.CalcH(this, m_StartCell);

            return SearchRangeInternal();
        }

        public bool IsCellInExplored(CellData cell)
        {
            return Explored.Contains(cell);
        }

        public bool IsCellInReachable(CellData cell)
        {
            return Reachable.Contains(cell);
        }

        /// <summary>
        /// 获取移动消耗
        /// </summary>
        /// <param name="terrainType"></param>
        /// <returns></returns>
        public float GetMoveConsumption(TerrainType terrainType)
        {
            if (m_MoveConsumption == null)
            {
                return 1f;
            }

            return m_MoveConsumption[terrainType];
        }

        /// <summary>
        /// 建立路径List
        /// </summary>
        /// <param name="endCell"></param>
        /// <param name="useResult"></param>
        /// <returns></returns>
        public List<CellData> BuildPath(CellData endCell, bool useResult)
        {
            if (endCell == null)
            {
                Debug.LogError("PathFinding -> Argument named 'endCell' is null.");
                return null;
            }

            List<CellData> path = useResult ? Result : new List<CellData>();
            CellData current = endCell;
            path.Add(current);
            while (current.previous != null)
            {
                current = current.previous;
                path.Insert(0,current);
            }

            return path;
        }

        /// <summary>
        /// 建立路径Stack
        /// </summary>
        /// <param name="endCell"></param>
        /// <returns></returns>
        public Stack<CellData> BuildPath(CellData endCell)
        {
            if (endCell == null)
            {
                Debug.LogError("PathFinding -> Argument named 'endCell' is null");
                return null;
            }
            Stack<CellData> path = new Stack<CellData>();

            CellData current = endCell;
            path.Push(current);
            while (current.previous != null)
            {
                current = current.previous;
                path.Push(current);
            }

            return path;
        }

        #region Reset Method

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Reachable.Clear();
            Explored.Clear();
            Result.Clear();

            m_Range = Vector2.zero;
            m_StartCell = null;
            EndCell = null;
            CurrentCell = null;
            m_Finished = false;
            m_HowToFind = null;
            m_MoveConsumption = null;

            SearchCount = 0;
        }

        #endregion
    }
}