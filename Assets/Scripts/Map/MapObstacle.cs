using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ColorPalette;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    /// <summary>
    /// 地图障碍物
    /// </summary>
    public class MapObstacle : MapObject
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private ColorSwapper m_Swapper;

        public Animator animator
        {
            get { return m_Animator; }
            set { m_Animator = value; }
        }

        public ColorSwapper swapper
        {
            get { return m_Swapper; }
            set { m_Swapper = value; }
        }

        public override MapObjectType mapObjectType
        {
            get { return MapObjectType.Obstacle; }
        }
    }
}