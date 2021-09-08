using System;
using System.Xml.Serialization;

/// <summary>
/// 道具类型
/// </summary>
[Serializable]
public enum ItemType
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknow = 0,

    /// <summary>
    /// 武器
    /// </summary>
    Weapon,

    /// <summary>
    /// 饰品
    /// </summary>
    Ornament,

    /// <summary>
    /// 消耗品
    /// </summary>
    Consumable
}