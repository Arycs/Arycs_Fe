using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.CombatManagement
{
    public enum CombatAnimaType
    {
        Unknow,
        Prepare, // 准备
        Attack, // 攻击
        Heal, //治疗
        Evade, // 躲闪
        Damage, // 受到攻击
        Dead // 死亡

        // 其它自定义
    }

    public struct CombatVariable
    {
        /// <summary>
        /// 位置
        /// </summary>
        public int position;

        /// <summary>
        /// 生命值
        /// </summary>
        public int hp;

        /// <summary>
        /// 魔法值
        /// </summary>
        public int mp;

        /// <summary>
        /// 是否可攻击
        /// </summary>
        public bool canAtk;

        /// <summary>
        /// 耐久度
        /// </summary>
        public int durability;

        /// <summary>
        /// 动画类型
        /// </summary>
        public CombatAnimaType animaType;

        /// <summary>
        /// 是否暴击
        /// </summary>
        public bool crit;

        /// <summary>
        /// 是否行动过
        /// </summary>
        public bool action;

        /// <summary>
        /// 是否已经死亡
        /// </summary>
        public bool isDead
        {
            get { return hp <= 0; }
        }

        public CombatVariable(int position, int hp, int mp, bool canAtk, CombatAnimaType animaType)
        {
            this.position = position;
            this.hp = hp;
            this.mp = mp;
            this.canAtk = canAtk;
            this.durability = 0;
            this.animaType = animaType;
            this.crit = false;
            this.action = false;
        }
        
        public CombatVariable(int position, int hp, int mp, bool canAtk,int durability, CombatAnimaType animaType)
        {
            this.position = position;
            this.hp = hp;
            this.mp = mp;
            this.canAtk = canAtk;
            this.durability = durability;
            this.animaType = animaType;
            this.crit = false;
            this.action = false;
        }
    }
}