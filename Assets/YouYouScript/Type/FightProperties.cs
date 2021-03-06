using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 战斗属性
/// str, mag, skl, spd, def, mdf
/// </summary>
[Serializable]
public struct FightProperties
{
    /// <summary>
    /// 力量
    /// </summary>
    [ProgressBar(0, 60)]
    public int str;

    /// <summary>
    /// 魔力
    /// </summary>
    [ProgressBar(0, 60)]
    public int mag;

    /// <summary>
    /// 技巧
    /// </summary>
    [ProgressBar(0, 60)]
    public int skl;

    /// <summary>
    /// 速度
    /// </summary>
    [ProgressBar(0, 60)]
    public int spd;

    /// <summary>
    /// 防御
    /// </summary>
    [ProgressBar(0, 60)]
    public int def;

    /// <summary>
    /// 魔防
    /// </summary>
    [ProgressBar(0, 60)]
    public int mdf;

    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case (int)FightPropertyType.STR:
                    return str;
                case (int)FightPropertyType.MAG:
                    return mag;
                case (int)FightPropertyType.SKL:
                    return skl;
                case (int)FightPropertyType.SPD:
                    return spd;
                case (int)FightPropertyType.DEF:
                    return def;
                case (int)FightPropertyType.MDF:
                    return mdf;
                default:
                    return 0;
                //throw new IndexOutOfRangeException("Not Supported");
            }
            
        }
        set
        {
            switch (index)
            {
                case (int)FightPropertyType.STR:
                    str = value;
                    break;
                case (int)FightPropertyType.MAG:
                    mag = value;
                    break;
                case (int)FightPropertyType.SKL:
                    skl = value;
                    break;
                case (int)FightPropertyType.SPD:
                    spd = value;
                    break;
                case (int)FightPropertyType.DEF:
                    def = value;
                    break;
                case (int)FightPropertyType.MDF:
                    mdf = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Not Supported");
            }
        }
    }

    public int this[FightPropertyType type]
    {
        get
        {
            switch (type)
            {
                case FightPropertyType.STR:
                    return str;
                case FightPropertyType.MAG:
                    return mag;
                case FightPropertyType.SKL:
                    return skl;
                case FightPropertyType.SPD:
                    return spd;
                case FightPropertyType.DEF:
                    return def;
                case FightPropertyType.MDF:
                    return mdf;
                default:
                    return 0;
                //throw new IndexOutOfRangeException("Not Supported");
            }
        }
        set
        {
            switch (type)
            {
                case FightPropertyType.STR:
                    str = value;
                    break;
                case FightPropertyType.MAG:
                    mag = value;
                    break;
                case FightPropertyType.SKL:
                    skl = value;
                    break;
                case FightPropertyType.SPD:
                    spd = value;
                    break;
                case FightPropertyType.DEF:
                    def = value;
                    break;
                case FightPropertyType.MDF:
                    mdf = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Not Supported");
            }
        }
    }

    public static FightProperties Clamp(FightProperties a,FightProperties b)
    {
        FightProperties fight = new FightProperties()
        {
            str = Mathf.Clamp(a.str, 0, b.str),
            mag = Mathf.Clamp(a.mag, 0, b.mag),
            skl = Mathf.Clamp(a.skl, 0, b.skl),
            spd = Mathf.Clamp(a.spd, 0, b.spd),
            def = Mathf.Clamp(a.def, 0, b.def),
            mdf = Mathf.Clamp(a.mdf, 0, b.mdf),
        };
        return fight;
    }

    public static FightProperties operator +(FightProperties lhs, FightProperties rhs)
    {
        FightProperties fight = new FightProperties
        {
            str = lhs.str + rhs.str,
            mag = lhs.mag + rhs.mag,
            skl = lhs.skl + rhs.skl,
            spd = lhs.spd + rhs.spd,
            def = lhs.def + rhs.def,
            mdf = lhs.mdf + rhs.mdf,
        };
        return fight;
    }
}