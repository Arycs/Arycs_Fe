using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.Models;
using UnityEngine;

namespace Arycs_Fe.CombatManagement
{
    /// <summary>
    /// 战斗实体
    /// </summary>
    public class CombatUnit :IDisposable
    {
        public MapClass mapClass { get; private set; }
        public Role role
        {
            get { return mapClass.role; }
        }
        
        /// <summary>
        /// 战斗中的位置
        /// </summary>
        public int position { get; private set; }

        /// <summary>
        /// 生命值
        /// </summary>
        public int hp { get; private set; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int maxHp { get; private set; }
        
        /// <summary>
        /// 魔法值
        /// </summary>
        public int mp { get; private set; }
        
        /// <summary>
        /// 最大魔法值
        /// </summary>
        public int maxMp { get; private set; }
        
        /// <summary>
        /// 攻击
        /// </summary>
        public int atk { get; private set; }
        
        /// <summary>
        /// 魔法攻击
        /// </summary>
        public int magAttack { get; private set; }
        
        /// <summary>
        /// 防御
        /// </summary>
        public int def { get; private set; }
        
        /// <summary>
        /// 魔法防御
        /// </summary>
        public int mageDef { get; private set; }
        
        /// <summary>
        /// 攻速
        /// </summary>
        public  int speed { get; private set; }
        
        /// <summary>
        /// 命中率
        /// </summary>
        public int hit { get; private set; }
        
        /// <summary>
        /// 暴击率
        /// </summary>
        public int crit { get; private set; }
        
        /// <summary>
        /// 回避率
        /// </summary>
        public int avoidance { get; private set; }
        
        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType weaponType { get; private set; }
        
        /// <summary>
        /// 武器耐久度
        /// </summary>
        public int durability { get; private set; }

        public CombatUnit(int position)
        {
            this.position = position;
        }

        public bool Load(MapClass mapClass)
        {
            if (mapClass == null)
            {
                return false;
            }

            if (mapClass.role == null)
            {
                return false;
            }

            this.mapClass = mapClass;
            this.hp = role.hp;
            this.mp = role.mp;
            this.maxHp = role.maxHp;
            this.maxMp = role.maxMp;
            this.atk = role.attack;
            this.magAttack = role.mageAttack;
            this.def = role.defence;
            this.mageDef = role.mageDefence;
            this.speed = role.speed;
            this.hit = role.hit;
            //this.crit = role.crit;
            this.avoidance = role.avoidance;

            if (role.equipedWeapon == null)
            {
                this.weaponType = WeaponType.Unknow;
                this.durability = 0;
            }
            else
            {
                this.weaponType = role.equipedWeapon.weaponType;
                this.durability = role.equipedWeapon.durability;
            }

            return true;
        }
        

        public void Dispose()
        {
            this.mapClass = null;
            this.position = -1; 
        }
        
        public void ClearMapClass()
        {
            this.mapClass = null;
        }
    }
}