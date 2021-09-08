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

        /// <summary>
        /// 当前世界坐标
        /// </summary>
        private Vector3Int m_Position;
        /// <summary>
        /// 是否有Tile
        /// </summary>
        private bool m_HasTile;
        /// <summary>
        /// 有哪个地图对象在上面，比如角色，建筑等--
        /// </summary>
        private MapObject m_MapObject;

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
            get { return m_HasTile; }
            set { m_HasTile = value; }
        }

        /// <summary>
        /// 地图对象
        /// </summary>
        public MapObject mapObject
        {
            get { return m_MapObject; }
            set { m_MapObject = value; }
        }

        /// <summary>
        /// 是否有地图对象
        /// </summary>
        public bool hasMapObject
        {
            get { return m_MapObject != null; }
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