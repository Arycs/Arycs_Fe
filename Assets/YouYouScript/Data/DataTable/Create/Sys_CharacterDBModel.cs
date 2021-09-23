
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
public partial class Sys_CharacterDBModel : DataTableDBModelBase<Sys_CharacterDBModel, Sys_CharacterEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "Sys_Character"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        int CharacterCount = ms.ReadInt();
        for (int i = 0; i < CharacterCount; i++)
        {
            Sys_CharacterEntity entity = new Sys_CharacterEntity();
            entity.CharacterId = ms.ReadInt();
            entity.CharacterName = ms.ReadUTF8String();
            entity.CharacterProfile = ms.ReadUTF8String();
            entity.CharacterClassId = ms.ReadInt();
            entity.CharacterLevel = ms.ReadInt();
            entity.CharacterHp = ms.ReadInt();
            entity.CharacterMp = ms.ReadInt();
            entity.CharacterLuk = ms.ReadInt();
            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
            {
                entity.CharacterFightProperties[j] = ms.ReadInt();
            }

            m_List.Add(entity);
            m_Dic[entity.Id] = entity;
        }
    }
}