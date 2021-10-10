using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;
using YouYou;

namespace Arycs_Fe.Models
{
    public abstract class Role
    {
        /// <summary>
        /// 角色武器
        /// </summary>
        protected Weapon m_EquipedWeapon = null;

        /// <summary>
        /// 角色背包
        /// </summary>
        protected readonly Item[] m_Items = new Item[SettingVars.k_RoleItemCount];

        protected RoleData self { get; set; }

        public ulong guid
        {
            get { return self.guid; }
        }

        public abstract RoleType roleType { get; }

        public int characterId
        {
            get { return self.characterId; }
        }

        public Character character
        {
            get { return GameEntry.Data.RoleDataManager.GetOrCreateCharacter(self.characterId); }
        }

        public int classId
        {
            get { return self.classId; }
        }

        public Class cls
        {
            get { return GameEntry.Data.RoleDataManager.GetOrCreateClass(self.classId); }
        }

        public MoveConsumption moveConsumption
        {
            get { return cls.moveConsumption; }
        }

        public AttitudeTowards attitudeTowards
        {
            get { return self.attitudeTowards; }
            set { self.attitudeTowards = value; }
        }

        public int level
        {
            get { return self.level; }
            set { self.level = value; }
        }

        public virtual FightProperties fightProperties
        {
            get { return self.fightProperties; }
        }

        public virtual int maxHp
        {
            get { return self.hp; }
        }

        public virtual int maxMp
        {
            get { return self.mp; }
        }

        public int hp
        {
            get { return self.hp; }
        }

        public int mp
        {
            get { return self.mp; }
        }

        public virtual int luk
        {
            get { return self.luk; }
        }

        public int money
        {
            get { return self.money; }
        }

        public float movePoint
        {
            get { return self.movePoint; }
        }

        public bool holding
        {
            get { return self.holding; }
        }

        /// <summary>
        /// 物理攻击力
        /// </summary>
        public int attack
        {
            get
            {
                if (equipedWeapon == null)
                {
                    return 0;
                }

                int atk = equipedWeapon.attack;
                atk += fightProperties[FightPropertyType.STR];
                atk += GetItemFightPropertySum(FightPropertyType.STR);
                return atk;
            }
        }

        /// <summary>
        /// 魔法攻击力
        /// </summary>
        public int mageAttack
        {
            get
            {
                if (equipedWeapon == null)
                {
                    return 0;
                }

                int mag = equipedWeapon.attack;
                mag += fightProperties[FightPropertyType.MAG];
                mag += GetItemFightPropertySum(FightPropertyType.MAG);
                return mag;
            }
        }

        /// <summary>
        /// 物理防御力
        /// </summary>
        public int defence
        {
            get
            {
                int def = fightProperties[FightPropertyType.DEF];
                def += GetItemFightPropertySum(FightPropertyType.DEF);
                return def;
            }
        }

        /// <summary>
        /// 魔法防御力
        /// </summary>
        public int mageDefence
        {
            get
            {
                int mdf = fightProperties[FightPropertyType.MDF];
                mdf += GetItemFightPropertySum(FightPropertyType.MDF);
                return mdf;
            }
        }

        /// <summary>
        /// 攻速
        /// </summary>
        public int speed
        {
            get
            {
                if (equipedWeapon == null)
                {
                    return 0;
                }

                int spd = fightProperties[FightPropertyType.SPD];
                spd += GetItemFightPropertySum(FightPropertyType.SPD);
                spd -= equipedWeapon.weight;
                return spd;
            }
        }

        /// <summary>
        /// 命中率
        /// </summary>
        public int hit
        {
            get
            {
                if (equipedWeapon == null)
                {
                    return 0;
                }

                int skl = fightProperties[FightPropertyType.SKL];
                skl += GetItemFightPropertySum(FightPropertyType.SKL);
                int hit = equipedWeapon.hit + skl * 2;
                return hit;
            }
        }

        /// <summary>
        /// 回避率
        /// </summary>
        public int avoidance
        {
            get
            {
                int spd = fightProperties[FightPropertyType.SPD];
                spd += GetItemFightPropertySum(FightPropertyType.SPD);
                int avd = spd * 2 + luk + GetItemLukSum();
                return avd;
            }
        }

        public virtual Weapon equipedWeapon
        {
            get { return m_EquipedWeapon; }
        }

        public void EquipWeapon( Weapon weapon)
        {
            m_EquipedWeapon = weapon;
        }

        public Item[] items
        {
            get { return m_Items; }
        }

        public bool isDead
        {
            get { return self.hp <= 0; }
        }

        protected Role()
        {
            self = new RoleData();
        }

        protected Role(ulong guid) : this()
        {
            self.guid = guid;
        }

        public virtual bool Load(RoleData data)
        {
            if (data == null)
            {
                return false;
            }

            data.CopyTo(self);
            return true;
        }

        /// <summary>
        /// 获取物品空位
        /// </summary>
        /// <returns></returns>
        protected int GetNullItemIndex()
        {
            int index = -1;
            for (int i = 0; i < SettingVars.k_RoleItemCount; i++)
            {
                if (m_Items[i] == null)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int AddItem(Item item)
        {
            if (item == null)
            {
                return -1;
            }

            int index = GetNullItemIndex();
            if (index != -1)
            {
                m_Items[index] = item;
                //如果是武器，则判断装备的武器是否为null
                if (item.ItemType == ItemType.Weapon && m_EquipedWeapon == null)
                {
                    m_EquipedWeapon = item as Weapon;
                }
            }

            return index;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Item RemoveItem(int index)
        {
            if (index < 0 || index >= SettingVars.k_RoleItemCount || m_Items[index] == null)
            {
                return null;
            }

            Item item = m_Items[index];
            m_Items[index] = null;

            //如果是装备的武器
            if (item.ItemType == ItemType.Weapon && m_EquipedWeapon == item)
            {
                m_EquipedWeapon = null;
                for (int i = 0; i < SettingVars.k_RoleItemCount; i++)
                {
                    if (m_Items[i] != null && m_Items[i].ItemType == ItemType.Weapon)
                    {
                        m_EquipedWeapon = m_Items[i] as Weapon;
                        break;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// 物品属性叠加
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetItemFightPropertySum(FightPropertyType type)
        {
            if (type == FightPropertyType.MaxLength)
            {
                return 0;
            }

            int value = 0;

            //如果装备的武器不为空
            if (equipedWeapon != null)
            {
                value += equipedWeapon.fightProperties[type];
            }

            //叠加所有饰品的属性
            foreach (Item item in items)
            {
                if (item != null && item.ItemType == ItemType.Ornament)
                {
                    value += ((Ornament) item).fightProperties[type];
                }
            }

            return value;
        }

        /// <summary>
        /// 物品幸运叠加
        /// </summary>
        /// <returns></returns>
        public int GetItemLukSum()
        {
            int value = 0;
            //如果装备武器部位null，则叠加武器幸运
            if (equipedWeapon != null)
            {
                value += equipedWeapon.luk;
            }

            //叠加所有饰品幸运
            foreach (Item item in items)
            {
                if (item != null && item.ItemType == ItemType.Ornament)
                {
                    value += ((Ornament) item).luk;
                }
            }

            return value;
        }

        public void OnBattleEnd(int hp, int mp, int durablity)
        {
            self.hp = hp;
            self.mp = mp;
            if (attitudeTowards == AttitudeTowards.Player)
            {
                equipedWeapon.durability = durablity;
            }
        }

        public void ResetMovePoint()
        {
            throw new System.NotImplementedException();
        }

        public void OnMoveEnd(float consume)
        {
            self.movePoint -= consume;
        }

        public void Holding(bool holding)
        {
            self.holding = holding;
        }

        public void AddFightProperty(FightPropertyType propertyType, int value)
        {
            self.fightProperties[propertyType] += value;
        }
    }

    /// <summary>
    /// 特有角色
    /// </summary>
    public class UniqueRole : Role
    {
        public override RoleType roleType
        {
            get { return RoleType.Unique; }
        }

        public UniqueRole() : base()
        {
        }
    }

    /// <summary>
    /// 从属角色
    /// </summary>
    public class FollowingRole : Role
    {
        public override RoleType roleType
        {
            get { return RoleType.Following; }
        }

        public FollowingRole(ulong guid) : base(guid)
        {
        }
    }
}