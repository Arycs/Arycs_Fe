using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可用武器等级： 
/// 0 不可用，
/// 1 F，2 E，3 D，4 C，5 B，6 A，7 S，8 星
/// </summary>
[Serializable]
public struct AvailableWeapons
{
    /// <summary>
    /// 剑
    /// </summary>
    public int sword;

    /// <summary>
    /// 枪
    /// </summary>
    public int lance;

    /// <summary>
    /// 斧
    /// </summary>
    public int axe;

    /// <summary>
    /// 弓
    /// </summary>
    public int bow;

    /// <summary>
    /// 杖
    /// </summary>
    public int staff;

    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case (int) WeaponType.Sword:
                    return sword;
                // 其它由于没有动画，全部都禁用了Obsolete, 但是不会报错
                case (int) WeaponType.Lance:
                    return lance;
                case (int) WeaponType.Axe:
                    return axe;
                case (int) WeaponType.Bow:
                    return bow;
                case (int) WeaponType.Staff:
                    return staff;
                default:
                    return 0;
                //throw new IndexOutOfRangeException("Not supported.");
            }
        }
        set
        {
            switch (index)
            {
                case (int) WeaponType.Sword:
                    sword = value;
                    break;
                case (int) WeaponType.Lance:
                    lance = value;
                    break;
                case (int) WeaponType.Axe:
                    axe = value;
                    break;
                case (int) WeaponType.Bow:
                    bow = value;
                    break;
                case (int) WeaponType.Staff:
                    staff = value;
                    break;
                default:
                    Debug.LogErrorFormat("武器类型索引：{0}, 无效", index);
                    break;
            }
        }
    }

    public int this[WeaponType type]
    {
        get
        {
            switch (type)
            {
                case WeaponType.Sword:
                    return sword;
                // 其它由于没有动画，全部都禁用了Obsolete。
                case WeaponType.Lance:
                    return lance;
                case WeaponType.Axe:
                    return axe;
                case WeaponType.Bow:
                    return bow;
                case WeaponType.Staff:
                    return staff;
                default:
                    return 0;
                //throw new IndexOutOfRangeException("Not supported.");
            }
        }
        set
        {
            switch (type)
            {
                case WeaponType.Sword:
                    sword = value;
                    break;
                case WeaponType.Lance:
                    lance = value;
                    break;
                case WeaponType.Axe:
                    axe = value;
                    break;
                case WeaponType.Bow:
                    bow = value;
                    break;
                case WeaponType.Staff:
                    staff = value;
                    break;
                default:
                    Debug.LogErrorFormat("武器类型索引：{0}, 无效", type.ToString());
                    break;
            }
        }
    }
}