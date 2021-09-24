using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.CombatManagement
{

    public enum BattleActionType
    {
        Unknow,
        Prepare,
        Attack,
        MageAttack,
        Heal,
        
    }

    public abstract class BattleAction : ScriptableObject
    {
        private string m_Message = "Unknow battle message";

        public string message
        {
            get { return m_Message; }
            protected set { m_Message = value; }
        }
        
        public abstract BattleActionType actionType { get; }

        public abstract CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal);

        public abstract bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal);

        public override string ToString()
        {
            return m_Message;
        }
    }
}