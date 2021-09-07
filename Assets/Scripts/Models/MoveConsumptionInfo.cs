using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Models
{
    /// <summary>
    /// 移动消耗信息
    /// </summary>
    [Serializable]
    public class MoveConsumptionInfo
    {
        /// <summary>
        /// 职业类型
        /// </summary>
        public ClassType type;

        /// <summary>
        /// 移动消耗具体数值
        /// </summary>
        public float[] consumptions;
    }
}