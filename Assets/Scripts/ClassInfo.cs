using System;

[Serializable]
public class ClassInfo
{
    /// <summary>
    /// 职业ID
    /// </summary>
    public int id;

    /// <summary>
    /// 预制体名称
    /// </summary>
    public string prefab = "PrefabName";

    /// <summary>
    /// 动画名称，当使用同一个prefab时，可以设置不同的动画
    /// </summary>
    public string animator = "AnimatorName";

    /// <summary>
    /// 职业名称
    /// </summary>
    public string name = "领主";

    /// <summary>
    /// 职业类型
    /// </summary>
    public ClassType classType = ClassType.Archer;

    /// <summary>
    /// 移动点数
    /// </summary>
    public float movePoint = 5;

    /// <summary>
    /// 战斗属性
    /// </summary>
    public FightProperties fightProperties;

    /// <summary>
    /// 最大战斗属性
    /// </summary>
    public FightProperties maxFightProperties;

    /// <summary>
    /// 可用武器
    /// </summary>
    public AvailableWeapons availableWeapons;

    /// <summary>
    /// TODO 包含的技能IDs
    /// </summary>
    //[XmlElement]
    //public int[] skills;
    public int GetKey()
    {
        return this.id;
    }
}