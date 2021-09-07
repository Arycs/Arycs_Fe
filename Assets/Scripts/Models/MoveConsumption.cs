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
        private MoveConsumptionInfo m_MoveConsumptionInfo;

        public float this[TerrainType terrainType]
        {
            get
            {
                if (terrainType == TerrainType.MaxLength)
                {
                    Debug.LogError("MoveConsuption -> TerrainType can not be MaxLength.");
                    return 0;
                }

                return m_MoveConsumptionInfo.consumptions[terrainType.ToInteger()];
            }
        }

        public MoveConsumption(ClassType classType)
        {
            //TODO Load from config file
            m_MoveConsumptionInfo = new MoveConsumptionInfo
            {
                type =  classType,
                consumptions =  new float[TerrainType.MaxLength.ToInteger()]
            };
            for (int i = 0; i < m_MoveConsumptionInfo.consumptions.Length; i++)
            {
                m_MoveConsumptionInfo.consumptions[i] = UnityEngine.Random.Range(0.5f, 3f);
            }
        }
    }
}