
//===================================================
//作    者：Arycs 
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using YouYou;

/// <summary>
/// Sys_Class实体
/// </summary>
public partial class Sys_ClassEntity : DataTableEntityBase
{
    /// <summary>
    /// 职业ID
    /// </summary>
    public int Id;

    /// <summary>
    /// 职业预制体
    /// </summary>
    public string Prefab;

    /// <summary>
    /// 动画名称
    /// </summary>
    public string AnimatorName;

    /// <summary>
    /// 职业名称
    /// </summary>
    public string ClassName;

    /// <summary>
    /// 职业类型
    /// </summary>
    public ClassType ClassType;

    /// <summary>
    /// 移动力
    /// </summary>
    public float MovePoint;

    /// <summary>
    /// 战斗属性
    /// </summary>
    public FightProperties FightProperties;

    /// <summary>
    /// 最大战斗属性
    /// </summary>
    public FightProperties MaxFightProperties;

    /// <summary>
    /// 武器属性
    /// </summary>
    public AvailableWeapons AvailableWeapons;

}
