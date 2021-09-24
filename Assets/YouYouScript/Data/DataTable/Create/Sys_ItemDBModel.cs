
//===================================================
//作    者：Arycs 
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using Arycs_Fe.Models;
using YouYou;

/// <summary>
/// 物品-数据管理
/// </summary>
public partial class Sys_ItemDBModel : DataTableDBModelBase<Sys_ItemDBModel, Sys_ItemEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "Sys_Item"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        int ItemCount = ms.ReadInt();
        for (int i = 0; i < ItemCount; i++)
        {
            Sys_ItemEntity entity = new Sys_ItemEntity();
            entity.ItemId = ms.ReadInt();
            entity.ItemName = ms.ReadUTF8String();
            entity.ItemDesc = ms.ReadUTF8String();
            entity.ItemIcon = ms.ReadUTF8String();
            entity.ItemPrice = ms.ReadInt();
            entity.ItemType = (ItemType)ms.ReadInt();
            if (entity.ItemType == ItemType.Weapon)
            {
                entity.WeaponType = (WeaponType)ms.ReadInt();
                entity.Level = ms.ReadInt();
                entity.Attack = ms.ReadInt();
                entity.MinRange = ms.ReadInt();
                entity.MaxRange = ms.ReadInt();
                entity.Weight = ms.ReadInt();
                entity.Durability = ms.ReadInt();
            }
            else if (entity.ItemType == ItemType.Ornament)
            {
                entity.Hp = ms.ReadInt();
                entity.Mp = ms.ReadInt();
                for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
                {
                    entity.FightProperties[j] = ms.ReadInt();
                }
                entity.Luk = ms.ReadInt();
                entity.MovePoint = ms.ReadFloat();
            }else if (entity.ItemType == ItemType.Consumable)
            {
                entity.StackingNumber = ms.ReadInt();
                entity.AmountUsed = ms.ReadInt();
                entity.UsingEffectId = ms.ReadInt();
            }

            m_List.Add(entity);
            m_Dic[entity.ItemId] = entity;
        }
    }
}