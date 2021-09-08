#region ---------- File Info ----------

/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				WeaponType.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 02 Feb 2018 00:00:41 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************

#endregion ---------- File Info ----------

using System;

/// <summary>
/// 武器类型
/// </summary>
[Serializable]
public enum WeaponType : int
{
    Unknow = -1,

    /// <summary>
    /// 剑
    /// </summary>
    Sword = 0,

    /// <summary>
    /// 枪
    /// </summary>
    [Obsolete("No animation", false)] Lance = 1,

    /// <summary>
    /// 斧
    /// </summary>
    [Obsolete("No animation", false)] Axe = 2,

    /// <summary>
    /// 弓
    /// </summary>
    [Obsolete("No animation", false)] Bow = 3,

    /// <summary>
    /// 杖
    /// </summary>
    [Obsolete("No animation", false)] Staff = 4,
    
    /// <summary>
    /// 最大长度
    /// </summary>
    MaxLength = 5,
}