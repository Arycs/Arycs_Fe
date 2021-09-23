using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/UIFormEditor")]
public class UIFormEditor : ScriptableObject, IEditorToBytes
{
    public bool IsEditor;

    [EnableIf("IsEditor")] public string FileName = "Sys_UIForm";

    [LabelText("生成Txt文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveTxtFilePath = "";

    [LabelText("生成Bytes文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveByteFilePath = "";

    [LabelText("生成CS文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveCSFilePath = "";

    [EnableIf("IsEditor")] [LabelText("物品信息")]
    public List<UIFormInfo> UIFormInfos = new List<UIFormInfo>();

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Bytes文件")]
    public void CreateByteFile()
    {
        byte[] buffer = ToBytes();
        FileStream fs = new FileStream(string.Format("{0}/{1}", SaveByteFilePath, FileName + ".bytes"),
            FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        Debug.LogError("客户端 ==> ClassInfo.bytes 文件生成完毕");
        AssetDatabase.Refresh();
    }

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Txt文件")]
    public void CreateTextFile()
    {
        if (CheckFile())
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < UIFormInfos.Count; i++)
        {
            sb.AppendFormat(
                "UIID : {0} , UI描述 : {1} , UI名称 : {2} , UI分组编号 : {3} , 是否禁用层级管理 : {4} , 是否锁定 : {5} , 路径 : {6} , 是否允许多实例 : {7} , 显示类型 : {8}",
                UIFormInfos[i].uiFormId, UIFormInfos[i].desc, UIFormInfos[i].uiName, UIFormInfos[i].uiGroup,
                UIFormInfos[i].disableUILayer, UIFormInfos[i].isLock, UIFormInfos[i].assetPath, UIFormInfos[i].canMulit,
                UIFormInfos[i].showMode);
            sb.Append("\n");
        }

        FileStream fs = new FileStream(string.Format("{0}\\{1}", SaveTxtFilePath, FileName + ".txt"), FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(sb);
        sw.Close();
        Debug.LogError("客户端表格=>" + "ClassInfo.txt生成完毕");
        AssetDatabase.Refresh();
        AssetDatabase.Refresh();
    }

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建实体和管理类文件")]
    public void CreateCSFile()
    {
        CreateEntity();
        CreateDBModel();
        AssetDatabase.Refresh();
    }

    public bool CheckFile()
    {
        List<int> tempIdList = new List<int>();
        tempIdList.Clear();
        for (int i = 0; i < UIFormInfos.Count; i++)
        {
            if (!(tempIdList.Contains(UIFormInfos[i].uiFormId)))
            {
                tempIdList.Add(UIFormInfos[i].uiFormId);
            }
            else
            {
                Debug.LogError("UI编辑器 ： UIID 有重复，请检查，重复ID：" + UIFormInfos[i].uiFormId);
                return true;
            }
        }

        Debug.LogError("UI编辑器 ： 无重复ID");
        return false;
    }

    public void CreateDBModel()
    {
        if (UIFormInfos.Count <= 0)
        {
            return;
        }

        StringBuilder sbr = new StringBuilder();
        sbr.Append("//===================================================\r\n");
        sbr.Append("//作    者：Arycs\r\n");
        sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
        sbr.Append("//===================================================\r\n");
        sbr.Append("using System.Collections;\r\n");
        ;
        sbr.Append("using System.Collections.Generic;\r\n");
        sbr.Append("using System;\r\n");
        sbr.Append("using YouYou;\r\n");
        sbr.Append("/// <summary>\r\n");
        sbr.Append("/// Sys_UIForm数据管理\r\n");
        sbr.Append("/// </summary>\r\n");
        sbr.AppendFormat("public partial class {0}DBModel : DataTableDBModelBase<{0}DBModel, {0}Entity>\r\n", FileName);
        sbr.Append("{\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 文件名称\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.AppendFormat("    public override string DataTableName {{ get {{ return \"{0}\"; }} }}\r\n", FileName);
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 加载列表\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    protected override void LoadList(MMO_MemoryStream ms)\r\n");
        sbr.Append("    {\r\n");
        sbr.Append("        int rows = ms.ReadInt();\r\n");
        sbr.Append("        for (int i = 0; i < rows; i++)\r\n");
        sbr.Append("        {\r\n");
        sbr.Append("            Sys_UIFormEntity entity = new Sys_UIFormEntity();\r\n");
        sbr.Append("            entity.Id = ms.ReadInt();\r\n");
        sbr.Append("            entity.Desc = ms.ReadUTF8String();\r\n");
        sbr.Append("            entity.Name = ms.ReadUTF8String();\r\n");
        sbr.Append("            entity.UIGroupId = (byte)ms.ReadByte();\r\n");
        sbr.Append("            entity.DisableUILayer = ms.ReadBool();\r\n");
        sbr.Append("            entity.IsLock = ms.ReadBool();\r\n");
        sbr.Append("            entity.AssetPath_Chinese = ms.ReadUTF8String();\r\n");
        sbr.Append("            entity.CanMulit = ms.ReadBool();\r\n");
        sbr.Append("            entity.ShowMode = (byte)ms.ReadByte();\r\n");
        sbr.Append("            m_List.Add(entity);\r\n");
        sbr.Append("            m_Dic[entity.Id] = entity;\r\n");
        sbr.Append("        }\r\n");
        sbr.Append("    }\r\n");
        sbr.Append("}\r\n");
        using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.cs", SaveCSFilePath, FileName),
            FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(sbr.ToString());
            }
        }

        Debug.LogErrorFormat("创建{0}DBModel.cs 成功", FileName);
    }

    public void CreateEntity()
    {
        if (UIFormInfos.Count <= 0)
        {
            return;
        }

        StringBuilder sbr = new StringBuilder();
        sbr.Append("//===================================================\r\n");
        sbr.Append("//作    者：Arycs\r\n");
        sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
        sbr.Append("//===================================================\r\n");
        sbr.Append("using System.Collections;\r\n");
        sbr.Append("using YouYou;\r\n");
        sbr.Append("/// <summary>\r\n");
        sbr.Append("/// Sys_UIForm实体\r\n");
        sbr.Append("/// </summary>\r\n");
        sbr.AppendFormat("public partial class {0}Entity : DataTableEntityBase\r\n",FileName);
        sbr.Append("{\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 描述\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string Desc;\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 名称\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string Name;\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// UI分组编号\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public byte UIGroupId;\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 禁用层级管理\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public bool DisableUILayer;\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 是否锁定\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public bool IsLock;\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 路径\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string AssetPath_Chinese;\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 允许多实例\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public bool CanMulit;\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 显示类型=普通 1=反切\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public byte ShowMode;\r\n");
        sbr.Append("}\r\n");
        using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.cs", SaveCSFilePath, FileName),
            FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(sbr.ToString());
            }
        }
        
        Debug.LogErrorFormat("创建{0}Entity.cs 成功", FileName);
    }

    public byte[] ToBytes()
    {
        if (CheckFile())
        {
            return null;
        }

        byte[] buffer = null;
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteInt(UIFormInfos.Count);
            for (int i = 0; i < UIFormInfos.Count; i++)
            {
                //写入ID
                ms.WriteInt(UIFormInfos[i].uiFormId);
                //写入描述
                ms.WriteUTF8String(UIFormInfos[i].desc);
                //写入名称
                ms.WriteUTF8String(UIFormInfos[i].uiName);
                //写入分组编号
                ms.WriteByte(UIFormInfos[i].uiGroup);
                //写入 禁用层级管理
                ms.WriteBool(UIFormInfos[i].disableUILayer);
                //写入 是否锁定
                ms.WriteBool(UIFormInfos[i].isLock);
                //写入路径
                ms.WriteUTF8String(UIFormInfos[i].assetPath);
                //写入 是否允许多实例
                ms.WriteBool(UIFormInfos[i].canMulit);
                //写入显示类型
                ms.WriteByte(UIFormInfos[i].showMode);
            }

            buffer = ms.ToArray();
        }

        return buffer;
    }
}

[Serializable]
public class UIFormInfo
{
    /// <summary>
    /// UI编号
    /// </summary>
    public int uiFormId;

    /// <summary>
    /// UI描述
    /// </summary>
    public string desc;

    /// <summary>
    /// UI名称
    /// </summary>
    public string uiName;

    /// <summary>
    /// UI分组编号
    /// </summary>
    public byte uiGroup;

    /// <summary>
    /// 禁用层级管理
    /// </summary>
    public bool disableUILayer;

    /// <summary>
    /// 是否锁定
    /// </summary>
    public bool isLock;

    /// <summary>
    /// 路径
    /// </summary>
    [FolderPath] public string assetPath;

    /// <summary>
    /// 允许多实例
    /// </summary>
    public bool canMulit;

    /// <summary>
    /// 显示类型
    /// </summary>
    public byte showMode;
}