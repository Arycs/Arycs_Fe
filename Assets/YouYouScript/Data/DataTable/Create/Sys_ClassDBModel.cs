
//===================================================
//作    者：Arycs 
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using YouYou;

/// <summary>
/// 职业-数据管理
/// </summary>
public partial class Sys_ClassDBModel : DataTableDBModelBase<Sys_ClassDBModel, Sys_ClassEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "Sys_Class"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        int ClassCount = ms.ReadInt();
        for (int i = 0; i < ClassCount; i++)
        {
            Sys_ClassEntity entity = new Sys_ClassEntity();
            entity.Id = ms.ReadInt();
            entity.Prefab = ms.ReadUTF8String();
            entity.AnimatorName = ms.ReadUTF8String();
            entity.ClassName = ms.ReadUTF8String();
            entity.ClassType = (ClassType)ms.ReadInt();
            entity.MovePoint = ms.ReadFloat();
            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
            {
                entity.FightProperties[j] = ms.ReadInt();
            }
            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
            {
                entity.MaxFightProperties[j] = ms.ReadInt();
            }
            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
            {
                entity.AvailableWeapons[j] = ms.ReadInt();
            }

            m_List.Add(entity);
            m_Dic[entity.Id] = entity;
        }
    }
}