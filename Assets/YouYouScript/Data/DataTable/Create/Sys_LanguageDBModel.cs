
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
public partial class Sys_LanguageDBModel : DataTableDBModelBase<Sys_LanguageDBModel, Sys_LanguageEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "Sys_Language"; } }

    /// <summary>
    /// 语言字典
    /// </summary>
    public Dictionary<int,string> LanguageDic = new Dictionary<int, string>();
    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        int ClassCount = ms.ReadInt();
        for (int i = 0; i < ClassCount; i++)
        {
            LanguageDic[ms.ReadInt()] =  ms.ReadUTF8String();        }
    }
}