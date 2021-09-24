using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.CombatManagement
{
    /// <summary>
    /// 战斗每一步结果
    /// </summary>
    public class CombatStep
    {
        /// <summary>
        /// 当前进攻方
        /// </summary>
        public CombatVariable atkVal { get; private set; }
        
        /// <summary>
        /// 当前防守方
        /// </summary>
        public CombatVariable defVal { get; private set; }

        public CombatStep(CombatVariable atker, CombatVariable defer)
        {
            this.atkVal = atker;
            this.defVal = defer;
        }

        /// <summary>
        /// 根据位置获取战斗变量
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CombatVariable GetCombatVariable(int position)
        {
            if (atkVal.position == position)
            {
                return atkVal;
            }

            if (defVal.position == position)
            {
                return defVal;
            }
            Debug.LogError("CombatStep -> position is out of range");
            return default(CombatVariable);
        }
    }
}