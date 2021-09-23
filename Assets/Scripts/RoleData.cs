using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;

namespace Arycs_Fe.Models
{
    /// <summary>
    /// 阵营类型
    /// </summary>
    [Serializable]
    public enum AttitudeTowards
    {
        /// <summary>
        /// 玩家
        /// </summary>
        Player,

        /// <summary>
        /// 敌人
        /// </summary>
        Enemy,

        /// <summary>
        /// 盟友
        /// </summary>
        Ally,

        /// <summary>
        /// 中立
        /// </summary>
        Neutral
    }

    [Serializable]
    public class RoleData : RuntimeData<RoleData>
    {
        public ulong guid;

        public int characterId;

        public int classId;

        public AttitudeTowards attitudeTowards;

        public int level = 1;

        public int exp = 0;

        public int hp = 1;

        public int mp;

        public FightProperties fightProperties;

        public int luk;

        public int money;

        public float movePoint;

        public override void CopyTo(RoleData data)
        {
            if (data == null)
            {
                Debug.LogError("RuntimeData -> CopyTo : data is null");
                return;
            }

            if (data == this)
            {
                return;
            }

            data.characterId = characterId;
            data.classId = classId;
            data.level = level;
            data.exp = exp;
            data.hp = hp;
            data.mp = mp;
            data.fightProperties = fightProperties;
            data.luk = luk;
            data.money = money;
            data.movePoint = movePoint;
        }
    }
}