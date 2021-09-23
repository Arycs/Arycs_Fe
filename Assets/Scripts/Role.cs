using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;

namespace Arycs_Fe.Models
{
    public abstract class Role
    {
        protected Weapon m_EquipedWeapon = null;

        //TODO 5为背包数量，后续通过Setting修改
        protected readonly Item[] m_Items = new Item[SettingVars.k_RoleItemCount];

        protected RoleData self { get; set; }

        public ulong guid
        {
            get { return self.guid; }
        }

        public abstract RoleType RoleType { get; }
        public AttitudeTowards attitudeTowards { get; set; }

        //TODO 其他属性

        protected Role()
        {
            self = new RoleData();
        }
        protected Role(ulong guid) : this()
        {
            self.guid = guid;
        }
        protected virtual bool Load(RoleData data)
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
                return - 1;
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

        public void ResetMovePoint()
        {
            throw new System.NotImplementedException();
        }
    }

    public class UniqueRole : Role
    {
        public override RoleType RoleType
        {
            get { return RoleType.Unique; }
        }

        public UniqueRole() : base()
        {
            
        }
    }

    public class FollowingRole : Role
    {
        public override RoleType RoleType
        {
            get { return RoleType.Following; }
        }

        public FollowingRole(ulong guid) : base(guid)
        {
            
        }
    }
}