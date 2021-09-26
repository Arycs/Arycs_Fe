using System;

/// <summary>
/// 角色类型
/// </summary>
[Serializable]
public enum RoleType
{
    /// <summary>
    /// 领主
    /// </summary>
    Unique,
    /// <summary>
    /// 从属/杂兵
    /// </summary>
    Following
}