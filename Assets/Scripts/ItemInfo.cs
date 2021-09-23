using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[System.Serializable]
public class ItemInfo
{
    /// <summary>
    /// 物品ID
    /// </summary>
    public int id = 1;

    /// <summary>
    /// 名称
    /// </summary>
    public string name = "道具名称";

    /// <summary>
    /// 道具描述
    /// </summary>
    public string desc = "道具描述";

    /// <summary>
    /// 图标
    /// </summary>
    public string icon = "图片名称";

    /// <summary>
    /// 价格，当价格是-1时，不可买卖与交易
    /// </summary>
    public int price = 1;

    /// <summary>
    /// 分类
    /// </summary>
    public ItemType itemType;

    //=====================================================武器属性==============================================
    /// <summary>
    /// 物品独有信息
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public WeaponType weaponType;

    /// <summary>
    /// 武器等级
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int level;

    /// <summary>
    /// 攻击力
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int attack;

    /// <summary>
    /// 最小攻击范围
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int minRange;

    /// <summary>
    /// 最大攻击范围
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int maxRange;

    /// <summary>
    /// 重量
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int weight;

    /// <summary>
    /// 耐久度
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int durability;

    //=====================================================饰品属性==============================================

    /// <summary>
    /// 生命值加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public int hp;

    /// <summary>
    /// 魔法值加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public int mp;

    /// <summary>
    /// 战斗属性加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public FightProperties fightProperties;

    /// <summary>
    /// 幸运加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public int luk;

    /// <summary>
    /// 移动力加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public float movePoint;

    //=====================================================消耗品属性==============================================

    /// <summary>
    /// 最大堆叠次数
    /// </summary>
    [ShowIf("itemType", ItemType.Consumable)]
    public int stackingNumber;

    /// <summary>
    /// 使用次数
    /// </summary>
    [ShowIf("itemType", ItemType.Consumable)]
    public int amountUsed;

    /// <summary>
    /// 使用效果ID
    /// </summary>
    [ShowIf("itemType", ItemType.Consumable)]
    public int usingEffectId;
}