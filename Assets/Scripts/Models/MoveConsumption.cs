using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using UnityEngine;

namespace Arycs_Fe.Models
{
    /// <summary>
    /// 移动消耗
    /// </summary>
    public class MoveConsumption
    {
        private MoveConsumptionInfo m_Info;

        public ClassType ClassType
        {
            get { return m_Info.type; }
        }
        
        public float this[TerrainType terrainType]
        {
            get
            {
                if (terrainType == TerrainType.MaxLength)
                {
                    Debug.LogError("MoveConsuption -> TerrainType can not be MaxLength.");
                    return 0;
                }

                return m_Info.consumptions[terrainType.ToInteger()];
            }
        }

        public MoveConsumption(MoveConsumptionInfo info)
        {
            this.m_Info = info;
        }
    }
}