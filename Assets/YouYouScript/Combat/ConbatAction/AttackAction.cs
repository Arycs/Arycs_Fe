using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Arycs_Fe.CombatManagement
{
    [CreateAssetMenu(fileName = "CombatAttackAction.asset", menuName = "SRPG/Combat Attack Action")]
    public class AttackAction : BattleAction
    {
        public override BattleActionType actionType
        {
            get { return BattleActionType.Attack; }
        }

        public override CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            CombatUnit atker = combat.GetCombatUnit(atkVal.position);
            CombatUnit defer = combat.GetCombatUnit(defVal.position);

            //攻击方动画
            atkVal.animaType = CombatAnimaType.Attack;

            //判断是否暴击
            bool crit = false; //TODO 是否暴击
            //判断是否命中， 如果暴击则说明必中
            bool isHit = false;
            //判断真实伤害，是否需要暴击
            int realAtk = 0;
            if (crit)
            {
                isHit = true;
                realAtk = atker.atk * 3; // 暴击伤害 * 3
            }
            else
            {
                //真实命中率 = 攻击者命中 - 防守者回避
                int realHit = atker.hit - defer.avoidance;
                //概率是否命中
                int hitRate = Random.Range(0, 100);
                isHit = hitRate <= realHit;
                realAtk = atker.atk;
            }


            if (isHit)
            {
                // TODO 触发伤害技能，这里写触发技能后伤害变化
                // realAtk += 触发的伤害
                // TODO 或者这里触发某些状态

                //掉血 = 攻击者攻击力 - 防御者防御力，0为没破防
                int damageHp = Mathf.Max(0, realAtk - defer.def);
                defVal.hp = Mathf.Max(0, defVal.hp - damageHp);


                atkVal.crit = crit;
                defVal.animaType = CombatAnimaType.Damage;

                // 更新此次攻击信息
                this.message = string.Format(
                    "{0} 对 {1} 的攻击造成了 {2} 点伤害{3}。",
                    atker.role.character.info.CharacterName,
                    defer.role.character.info.CharacterName,
                    damageHp,
                    crit ? "(爆击)" : string.Empty);
                if (defVal.isDead)
                {
                    this.message += string.Format("{0}被击败了", defer.role.character.info.CharacterName);
                }
            }
            else
            {
                defVal.animaType = CombatAnimaType.Evade;
                // 更新此次躲闪信息
                this.message = string.Format(
                    "{1} 躲闪了 {0} 的攻击。",
                    atker.role.character.info.CharacterName,
                    defer.role.character.info.CharacterName);
            }

            //只有玩家才会减低耐久度
            if (atker.role.attitudeTowards == AttitudeTowards.Player)
            {
                //攻击者武器耐久度-1
                atkVal.durability = Mathf.Max(0, atkVal.durability - 1);
            }

            //攻击者行动过了
            atkVal.action = true;

            CombatStep step = new CombatStep(atkVal, defVal);
            return step;
        }

        public override bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            //防守者死亡
            if (defVal.isDead)
            {
                return true;
            }
            
            //如果防守者行动过了
            if (defVal.action)
            {
                //CombatUnit atker = GetCombatUnit(atkVal.position);
                //CombatUnit defer = GetCombatUnit(defVal.position);

                // TODO 是否继续攻击，必要时需要在 CombatVariable 加入其它控制变量
                // 比如，触发过技能或物品了
                // atker.role.skill/item 包含继续战斗的技能或物品
                // defer.role.skill/item 包含继续战斗的技能或物品
                //if ( 已经触发过继续战斗技能或物品 )
                //{
                //    // return true;
                //}
            }

            return false;
        }
    }
}