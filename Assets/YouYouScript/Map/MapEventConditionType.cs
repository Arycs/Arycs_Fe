using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    [Serializable]
    public enum MapEventConditionType
    {
        /// <summary>
        /// 无条件，进入地图触发
        /// </summary>
        NoneCondition = 0,
        
        /// <summary>
        /// 回合条件，当回合开始时触发
        /// </summary>
        TurnCondition,
        
        /// <summary>
        /// 位置条件，当角色到达此坐标时触发，常与RoleCondition连用
        /// </summary>
        PositionCondition,
        
        /// <summary>
        /// 角色条件， 不是主条件，通常为其他条件的子条件所用
        /// </summary>
        RoleCondition,
        
        /// <summary>
        /// 属性条件，不是主条件，通常为其他条件的子条件使用
        /// </summary>
        PropertyCondition,
        
        /// <summary>
        /// 角色死亡，角色死亡时触发 
        /// </summary>
        RoleDeadCondition,
        
        /// <summary>
        /// 对话条件， 两个角色有对话时触发，常与 TurnCondition连用 （在某些回合范围内可对话）
        /// </summary>
        RoleTalkCondition,
        
        /// <summary>
        /// 战斗对话条件，两个角色发生战斗时触发
        /// </summary>
        RoleCombatTalkCondition,
        
        /// <summary>
        /// 物品条件， 不是主条件，常作为其他条件的子条件使用（例如持有/不持有某些物品时）
        /// </summary>
        ItemCondition
    }
}