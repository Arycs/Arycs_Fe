using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;

public abstract class Item
{
    [Serializable]
    public enum OwnerType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow = 0,

        /// <summary>
        /// 角色
        /// </summary>
        Role,

        /// <summary>
        /// 商店
        /// </summary>
        Shop,

        /// <summary>
        /// 地图
        /// </summary>
        Map
    }
    
    private Sys_ItemEntity m_Info;
    protected ItemData self { get; set; }

    public abstract ItemType ItemType { get; }

    public ulong guid
    {
        get { return self.guid; }
    }

    public int itemId
    {
        get { return self.itemId; }
    }

    public int durability
    {
        get { return self.durability; }
    }

    public bool isBroken
    {
        get { return self.durability <= 0; }
    }

    public OwnerType ownerType
    {
        set { self.ownerType = value;}
        get { return self.ownerType; }
    }

    protected Item(Sys_ItemEntity info, ulong guid)
    {
        this.m_Info = info;
        this.self = new ItemData()
        {
            guid = guid
        };
    }

    /// <summary>
    /// 从模板读取可变数据
    /// </summary>
    /// <param name="data"></param>
    public virtual void Load(ItemData data)
    {
        data.CopyTo(this.self);
    }

    public void Dispose()
    {
        
    }
}

public class Weapon : Item
{
    public override ItemType ItemType
    {
        get { return ItemType.Weapon; }
    }
    
    public WeaponType weaponType;

    public int level;

    public int attack;

    public int minRange;

    public int maxRange;

    public int weight;

    public int hit;
    
    public int durability;

    public int luk;

    public FightProperties fightProperties;
    public Weapon(Sys_ItemEntity info, ulong guid) : base(info, guid)
    {
    }

}

public class Consumable : Item
{
    public override ItemType ItemType
    {
        get { return ItemType.Consumable; }
    }

    public int stackingNumber;

    public int amountUsed;

    public int usingEffectId;
    
    public Consumable(Sys_ItemEntity info, ulong guid) : base(info, guid)
    {
    }
}

public class Ornament : Item
{
    public override ItemType ItemType
    {
        get { return ItemType.Ornament; }
    }

    public int hp;

    public int mp;

    public FightProperties fightProperties;

    public int luk;
    
    public float movePoint;


    public Ornament(Sys_ItemEntity info, ulong guid) : base(info, guid)
    {
    }

}