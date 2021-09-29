using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum MapEventResultType
{
    /// <summary>
    /// 没有任何结果；
    /// </summary>
    NoneResult = 0,
    
    /// <summary>
    /// 剧本结果，触发剧情
    /// </summary>
    ScenarioResult ,
    
    /// <summary>
    /// 创建MapObject结果，创建地图障碍物或角色等
    /// </summary>
    CreateObjectResult,
    
    /// <summary>
    /// 传送位置，标识讲角色传送到的位置
    /// </summary>
    PositionResult,
    
    /// <summary>
    /// 增加/减少属性，标识角色增加或减少的属性
    /// </summary>
    PropertyResult,
    
    /// <summary>
    /// 获得/遗失物品，获取或丢失物品
    /// </summary>
    ItemResult,
    
    /// <summary>
    /// 战斗胜利
    /// </summary>
    WinResult,
    
    /// <summary>
    /// 战斗失败
    /// </summary>
    LoseResult,
}
