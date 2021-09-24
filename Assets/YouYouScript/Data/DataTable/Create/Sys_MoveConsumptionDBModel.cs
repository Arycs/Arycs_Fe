
//===================================================
//作    者：Arycs 
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using YouYou;

/// <summary>
/// 语言包-数据管理
/// </summary>
public partial class Sys_MoveConsumptionDBModel : DataTableDBModelBase<Sys_MoveConsumptionDBModel, Sys_MoveConsumptionEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "Sys_MoveConsumption"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        int ClassCount = ms.ReadInt();
        for (int i = 0; i < ClassCount; i++)
        {
            Sys_MoveConsumptionEntity entity = new Sys_MoveConsumptionEntity();
            entity.classType = (ClassType)ms.ReadInt();
            int Length = ms.ReadInt();
            entity.consumptions = new float[Length];
            for(int j = 0; j < Length; j++)
            {
                entity.consumptions[j] = ms.ReadFloat();
            }
            
            m_List.Add(entity);
            m_Dic[(int)entity.classType] = entity;
        }
    }
}