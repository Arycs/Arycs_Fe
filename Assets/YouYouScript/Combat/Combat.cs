using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Arycs_Fe.CombatManagement
{
    [DisallowMultipleComponent]
    [AddComponentMenu("SRPG/Combat System/Combat")]
    public class Combat : MonoBehaviour
    {
        public CombatUnit unit0 { get;protected set; } //如果是群攻，则使用数组后list
        public CombatUnit unit1 { get;protected set; } //如果是群攻，则使用数组后list
        public List<CombatStep> steps { get; protected set; }

        public BattleAction[] m_BattleActions;

        private Dictionary<BattleActionType, BattleAction> m_BattleActionsDict =
            new Dictionary<BattleActionType, BattleAction>();
        
        
        public bool isLoaded
        {
            get { return unit0.mapClass != null && unit1.mapClass != null; }
        }

        public int stepCount
        {
            get { return steps.Count; }
        }

        private void Awake()
        {
            if (m_BattleActions != null && m_BattleActions.Length > 0)
            {
                for (int i = 0; i < m_BattleActions.Length; i++)
                {
                    if (m_BattleActions[i] == null)
                    {
                        continue;
                    }

                    BattleAction action = m_BattleActions[i];
                    if (m_BattleActionsDict.ContainsKey(action.actionType))
                    {
                        Debug.LogWarningFormat("Battle Action {0} is exist. OVERRIDE.",action.actionType.ToString());
                    }

                    m_BattleActionsDict[action.actionType] = action;
                }
            }
            unit0 = new CombatUnit(0);
            unit1 = new CombatUnit(1);
            steps = new List<CombatStep>();
        }

        private void OnDestroy()
        {
            unit0.Dispose();
            unit0 = null;
            unit1.Dispose();
            unit1 = null;
            steps = null;
        }

        public bool LoadCombatUnit(MapClass mapClass0, MapClass mapClass1)
        {
            return unit0.Load(mapClass0) && unit1.Load(mapClass1);
        }

        public CombatUnit GetCombatUnit(int position)
        {
            switch (position)
            {
                case 0:
                    return unit0;
                case 1:
                    return unit1;
                default:
                    Debug.LogError("Combat - > GetCombatUnit :index is out of range");
                    return null;
            }
        }

        public void BattleBegin()
        {
            if (!isLoaded)
            {
                Debug.LogError("Combat -> StartBattle :please load combat unit first");
                return;
            }

            if (stepCount > 0)
            {
                Debug.LogError("Combat -> StartBattle: battle is not end");
                return;
            }

            BattleAction action;
            if (!m_BattleActionsDict.TryGetValue(BattleActionType.Prepare,out action))
            {
                Debug.LogError("Combat -> StartBattle: BattleActionType.Prepare is not found, check the code.");
                return;
            }

            CombatStep firststep = action.CalcBattle(this, default(CombatVariable),default(CombatVariable));
            steps.Add(firststep);
            if (!action.IsBattleEnd(this,firststep.atkVal,firststep.defVal))
            {
                CalcBattle(firststep.atkVal,firststep.defVal);
            }
        }

        /// <summary>
        /// 获取行动方式（如何计算战斗数据）
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        private BattleActionType GetBattleActionType(WeaponType weaponType)
        {
            // TODO 由于没有动画支持，所以并没有其他武器
            // 你可以添加其他武器到这里

            switch (weaponType)
            {
                case WeaponType.Sword:
                    //case WeaponType.Lance:
                    //case WeaponType.Axe:
                    //case WeaponType.Bow:
                    return BattleActionType.Attack;
                //case WeaponType.Staff:
                //if ( 如果法杖是治疗 )
                //{
                //    return BattleActionType.Heal;
                //}
                //else if ( 法杖是其它等 )
                //{
                //    return BattleActionType.自定义类型;
                //}
                //case WeaponType.Fire:
                //case WeaponType.Thunder:
                //case WeaponType.Wind:
                //case WeaponType.Holy:
                //case WeaponType.Dark:
                //    return BattleActionType.MageAttack;
                default:
                    return BattleActionType.Unknow;
            }
        }
        
        /// <summary>
        /// 计算战斗数据
        /// </summary>
        /// <param name="atkVal"></param>
        /// <param name="defVal"></param>
        private void CalcBattle(CombatVariable atkVal, CombatVariable defVal)
        {
            CombatUnit atker = GetCombatUnit(atkVal.position);
            BattleActionType actionType = GetBattleActionType(atker.weaponType);
            BattleAction action;
            if (!m_BattleActionsDict.TryGetValue(actionType,out action))
            {
                Debug.LogError("Combat -> StartBattle: BattleActionType.Prepare is not found, check the code.");
                return;
            }

            CombatStep step = action.CalcBattle(this, atkVal, defVal);
            steps.Add(step);

            //如果战斗没有结束，交换攻击者与防守者
            if (!action.IsBattleEnd(this,atkVal,defVal))
            {
                if (defVal.canAtk)
                {
                    CalcBattle(defVal,atkVal);
                }
                else
                {
                    //如果防守方不可反击
                    defVal.action = true;
                    if (!action.IsBattleEnd(this,defVal,atkVal))
                    {
                        CalcBattle(atkVal,defVal);
                    }
                }
            }
            else
            {
                //TODO 如果死亡则播放死亡动画
                if (defVal.isDead)
                {
                    
                }
            }
        }

        /// <summary>
        /// 战斗结束
        /// </summary>
        public void BattleEnd()
        {
            if (stepCount > 0)
            {
                CombatStep result = steps[stepCount - 1];

                CombatVariable unit0Result = result.GetCombatVariable(0);
                CombatVariable unit1Result = result.GetCombatVariable(1);
                
                //TODO 经验值战利品将结果传回角色中
                unit0.mapClass.OnBattleEnd(unit0Result.hp, unit0Result.mp, unit0.durability);
                unit0.mapClass.OnBattleEnd(unit1Result.hp, unit1Result.mp, unit1.durability);
                
                steps.Clear();
            }
            unit0.ClearMapClass();
            unit1.ClearMapClass();
        }
    }
}