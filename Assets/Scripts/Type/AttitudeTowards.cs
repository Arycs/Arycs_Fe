using System;

/// <summary>
/// 阵营类型
/// </summary>
[Serializable]
public enum AttitudeTowards
{
    /// <summary>
    /// 玩家
    /// </summary>
    Player,

    /// <summary>
    /// 敌人
    /// </summary>
    Enemy,

    /// <summary>
    /// 盟友
    /// </summary>
    Ally,

    /// <summary>
    /// 中立
    /// </summary>
    Neutral
}