using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

/// <summary>
/// 对象池实体
/// </summary>
[System.Serializable]
public class GameObjectPoolEntity
{
    /// <summary>
    /// 对象池编号
    /// </summary>
    public byte PoolId;

    /// <summary>
    /// 对象池名字
    /// </summary>
    public string PoolName;

    /// <summary>
    /// 是否开启缓存池自动清理模式
    /// </summary>
    [Header("是否开启缓存池自动清理模式")]
    public bool CullDespawned = true;

    /// <summary>
    /// 缓存池自动清理 但是始终保留几个对象不清理
    /// </summary>
    [Header("缓存池自动清理 但是始终保留几个对象不清理")]
    public int CullAbove = 5;

    /// <summary>
    /// 多长时间清理一次 单位是秒
    /// </summary>
    [Header("多长时间清理一次 单位是秒")]
    public int CullDelay = 2;

    /// <summary>
    /// 每次清理几个 
    /// </summary>
    [Header("每次清理几个")]
    public int CullMaxPerPass = 2;

    /// <summary>
    /// 对应的游戏物体对象池
    /// </summary>
    /// <returns></returns>
    [Header("对应的游戏物体对象池")]
    public SpawnPool Pool;
}
