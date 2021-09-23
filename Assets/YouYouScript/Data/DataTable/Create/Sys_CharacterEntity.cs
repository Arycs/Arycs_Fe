
//===================================================
//作    者：Arycs 
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using YouYou;

/// <summary>
/// Sys_Character实体
/// </summary>
public partial class Sys_CharacterEntity : DataTableEntityBase
{
    /// <summary>
    /// 人物ID
    /// </summary>
    public int CharacterId;

    /// <summary>
    /// 人物名称
    /// </summary>
    public string CharacterName;

    /// <summary>
    /// 人物头像
    /// </summary>
    public string CharacterProfile;

    /// <summary>
    /// 人物职业ID
    /// </summary>
    public int CharacterClassId;

    /// <summary>
    /// 人物基本等级
    /// </summary>
    public int CharacterLevel;

    /// <summary>
    /// 人物基础血量
    /// </summary>
    public int CharacterHp;

    /// <summary>
    /// 人物基础蓝量(包含各种消耗，比如怒气，能量，并不单一指代蓝量)
    /// </summary>
    public int CharacterMp;

    /// <summary>
    /// 人物幸运值
    /// </summary>
    public int CharacterLuk;

    /// <summary>
    /// 战斗属性
    /// </summary>
    public FightProperties CharacterFightProperties;

}
