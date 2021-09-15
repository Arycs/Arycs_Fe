using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    /// <summary>
    /// 地图上每个格子的信息
    /// </summary>
    public class CellData : IDisposable
    {
        #region Common Field/Property
        
        private Vector3Int m_Position;
        private MapObject m_MapObject;
        private CellStatus m_Status = CellStatus.None;
        
        /// <summary>
        /// 坐标位置
        /// </summary>
        public Vector3Int position
        {
            get { return m_Position; }
        }

        /// <summary>
        /// 是否有Tile
        /// </summary>
        public bool hasTile
        {
            get { return CheckStatus(CellStatus.TerrainTile,false); }
            set { SwitchStatus(CellStatus.TerrainTile, value); }
        }

        /// <summary>
        /// 是否有Cursor
        /// </summary>
        public bool hasCursor
        {
            get { return CheckStatus(CellStatus.MoveCursor | CellStatus.AttackCursor, false); }
            set { SwitchStatus(CellStatus.MoveCursor| CellStatus.AttackCursor,value);}
        }
        
        /// <summary>
        /// 是否移动范围光标
        /// </summary>
        public bool hasMoveCursor
        {
            get { return CheckStatus(CellStatus.MoveCursor, false); }
            set { SwitchStatus(CellStatus.MoveCursor, value); }
        }
        /// <summary>
        /// 是否有攻击范围光标
        /// </summary>
        public bool hasAttackCursor
        {
            get { return CheckStatus(CellStatus.MoveCursor, false); }
            set { SwitchStatus(CellStatus.MoveCursor, value); }
        }

        /// <summary>
        /// 地图对象
        /// </summary>
        public MapObject mapObject
        {
            get { return m_MapObject; }
            set
            {
                m_MapObject = value;
                SwitchStatus(CellStatus.MapObject,value != null);
            }
        }

        /// <summary>
        /// 是否有地图对象
        /// </summary>
        public bool hasMapObject
        {
            get { return m_MapObject != null; }
        }

        /// <summary>
        /// 是否可移动
        /// </summary>
        public bool canMove
        {
            get { return hasTile && !hasMapObject; }
        }

        /// <summary>
        /// 获取状态开关
        /// </summary>
        /// <returns></returns>
        public CellStatus GetStatus()
        {
            return m_Status;
        }

        #endregion

        #region Constructor
        public CellData(Vector3Int position)
        {
            m_Position = position;
        }

        public CellData(int x, int y)
        {
            m_Position = new Vector3Int(x, y, 0);
        }
        #endregion

        #region AStar Field/Property

        private List<CellData> m_Adjacents = new List<CellData>();
        private CellData m_Previous;
        private Vector2 m_AStarGH;
        
        /// <summary>
        /// 邻居CellData
        /// </summary>
        public List<CellData> adjacents
        {
            get { return m_Adjacents; }
        }
        
        /// <summary>
        /// 寻找的前一个CellData
        /// </summary>
        public CellData previous
        {
            get { return m_Previous; }
            set { m_Previous = value; }
        }

        /// <summary>
        /// AStar的G值，移动消耗
        /// </summary>
        public float g
        {
            get { return m_AStarGH.x; }
            set { m_AStarGH.x = value; }
        }

        /// <summary>
        /// AStar的H值，预计消耗
        /// </summary>
        public float h
        {
            get { return m_AStarGH.y; }
            set { m_AStarGH.y = value; }
        }

        /// <summary>
        /// AStar的F值， F = G + H
        /// </summary>
        public float f
        {
            get { return m_AStarGH.x + m_AStarGH.y; }
        }

        #endregion

        #region Reset AStar Method

        public void ResetAStar()
        {
            m_Previous = null;
            m_AStarGH = Vector2.zero;
        }

        #endregion

        /// <summary>
        /// 设置状态开关
        /// </summary>
        /// <param name="status"></param>
        /// <param name="isOn"></param>
        public void SwitchStatus(CellStatus status, bool isOn)
        {
            if (isOn)
            {
                //打开开关， 二进制的 | 直接就可以达成
                m_Status |= status;
            }
            else
            {
                //关闭开关， 先对当前状态取反，然后在与运算即可
                m_Status &= ~status;
            }
        }

        /// <summary>
        /// 检查开关
        /// </summary>
        /// <param name="status"></param>
        /// <param name="any">
        ///     true :判断在status中是否存在开启项
        ///     false :判断status中是否全部开启
        /// </param>
        /// <returns></returns>
        public bool CheckStatus(CellStatus status, bool any)
        {
            return any ? (m_Status & status) != 0 :(m_Status & status) == status;
        } 

        public void Dispose()
        {
            m_Position = Vector3Int.zero;
            m_MapObject = null;
            m_Adjacents = null;
            m_Previous = null;
            m_AStarGH = Vector2.zero;
        }
    }
}