
//===================================================
//作    者：Arycs 
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using Arycs_Fe.Models;
using YouYou;

/// <summary>
/// Sys_Item实体
/// </summary>
public partial class Sys_ItemEntity : DataTableEntityBase
{
    /// <summary>
    /// 物品ID
    /// </summary>
    public int ItemId;
    
    /// <summary>
    /// 名称
    /// </summary>
    public string ItemName;
    
    /// <summary>
    /// 道具描述
    /// </summary>
    public string ItemDesc;
    
    /// <summary>
    /// 图标
    /// </summary>
    public string ItemIcon;
    
    /// <summary>
    /// 价格，当价格是-1时，不可买卖与交易
    /// </summary>
    public int ItemPrice;
    
    /// <summary>
    /// 分类
    /// </summary>
    public ItemType ItemType;
    
    //=====================================================武器属性==============================================
    /// <summary>
    /// 物品独有信息
    /// </summary>
    public WeaponType WeaponType;
    
    /// <summary>
    /// 武器等级
    /// </summary>
    public int Level;
    
    /// <summary>
    /// 攻击力
    /// </summary>
    public int Attack;
    
    /// <summary>
    /// 最小攻击范围
    /// </summary>
    public int MinRange;
    
    /// <summary>
    /// 最大攻击范围
    /// </summary>
    public int MaxRange;
    
    /// <summary>
    /// 重量
    /// </summary>
    public int Weight;
    
    /// <summary>
    /// 耐久度
    /// </summary>
    public int Durability;
    
    //=====================================================饰品属性==============================================
    
    /// <summary>
    /// 生命值加成
    /// </summary>
    public int Hp;
    
    /// <summary>
    /// 魔法值加成
    /// </summary>
    public int Mp;
    
    /// <summary>
    /// 战斗属性加成
    /// </summary>
    public FightProperties FightProperties;
    
    /// <summary>
    /// 幸运加成
    /// </summary>
    public int Luk;
    
    /// <summary>
    /// 移动力加成
    /// </summary>
    public float MovePoint;
    
    //=====================================================消耗品属性==============================================
    
    /// <summary>
    /// 最大堆叠次数
    /// </summary>
    public int StackingNumber;
    
    /// <summary>
    /// 使用次数
    /// </summary>
    public int AmountUsed;
    
    /// <summary>
    /// 使用效果ID
    /// </summary>
    public int UsingEffectId;

}
