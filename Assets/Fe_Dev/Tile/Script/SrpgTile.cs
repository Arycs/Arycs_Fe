using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New SRPG Tile.asset",menuName = "SRPG/Tile")]
    public class SrpgTile : RuleTile
    {
        [Header("地形")]
        [SerializeField]
        private TerrainType m_TerrainType = TerrainType.Plain;
        [Header("回避率")]
        [SerializeField]
        private int m_AvoidRate = 0;

        /// <summary>
        /// 地形类型
        /// </summary>
        public TerrainType terrainType
        {
            get { return m_TerrainType; }
            set { m_TerrainType = value; }
        }

        /// <summary>
        /// 回避率
        /// </summary>
        public int avoidRate
        {
            get { return m_AvoidRate; }
            set { m_AvoidRate = value; }
        }
    }
}