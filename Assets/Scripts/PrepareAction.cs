using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Arycs_Fe.CombatManagement;
using UnityEngine;

namespace Arycs_Fe.CombatManagement
{
    [CreateAssetMenu(fileName = "CombatPrepareAction.asset", menuName = "SRPG/Combat Prepare Action")]
    public class PrepareAction : BattleAction
    {
        public override BattleActionType actionType
        {
            get { return BattleActionType.Prepare; }
        }
        public override CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            CombatUnit atker = combat.GetCombatUnit(0);
            CombatUnit defer = combat.GetCombatUnit(1);
            
            //是否可反击
            bool canDeferAtk = false;
            if (defer.role.equipedWeapon != null)
            {
                Vector3Int offset = defer.mapClass.cellPosition - atker.mapClass.cellPosition;
                int dist = Mathf.Abs(offset.x) + Mathf.Abs(offset.y);
                int atkMinRange = defer.role.equipedWeapon.minRange;
                int atkMaxRange = defer.role.equipedWeapon.maxRange;
                if (dist >= atkMinRange && dist <= atkMaxRange)
                {
                    canDeferAtk = true;
                }
            }
            
            //根据熟读初始化攻击者与防御者
            if (canDeferAtk)
            {
                if (atker.speed  < defer.speed)
                {
                    CombatUnit tmp = atker;
                    atker = defer;
                    defer = tmp;
                }
            }

            this.message = "战斗开始";
            atkVal = new CombatVariable(atker.position, atker.hp, atker.mp, true, atker.durability,
                CombatAnimaType.Prepare);
            defVal = new CombatVariable(defer.position, defer.hp, defer.mp, canDeferAtk, defer.durability,
                CombatAnimaType.Prepare);

            
            //准备阶段
            CombatStep firststep = new CombatStep(atkVal, defVal);
            return firststep;
        }

        public override bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            return false;
        }
    }
}