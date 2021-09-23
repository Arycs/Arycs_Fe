using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/LanguageEditor")]
public class LanguageEditor : ScriptableObject,IEditorToBytes
{
    public bool IsEditor;
    
    [EnableIf("IsEditor")]
    public string FileName = "Sys_Language";

    [LabelText("生成Txt文件路径")]
    [FolderPath]
    [EnableIf("IsEditor")]
    public string SaveTxtFilePath = "";
    
    [LabelText("生成Bytes文件路径")]
    [FolderPath]
    [EnableIf("IsEditor")]
    public string SaveByteFilePath = "";
    
    [LabelText("生成CS文件路径")]
    [FolderPath]
    [EnableIf("IsEditor")]
    public string SaveCSFilePath = "";


    [EnableIf("IsEditor")]
    [LabelText("语言包信息")]
    public List<LanguageInfo> LanguageInfos = new List<LanguageInfo>();
    
    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Txt文件")]
    public void CreateTextFile()
    {
        if (CheckFile()) return;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < LanguageInfos.Count; i++)
        {
            sb.AppendFormat("ID : {0} \t 内容 : {1}", LanguageInfos[i].languageId, LanguageInfos[i].chineseStr);
            sb.Append("\n");
        }
        FileStream fs = new FileStream(string.Format("{0}\\{1}", SaveTxtFilePath, FileName + ".txt"), FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(sb);
        sw.Close();
        Debug.LogErrorFormat("客户端表格=>" + "{0}.txt生成完毕",FileName);
        AssetDatabase.Refresh();
    }
    
    public byte[] ToBytes()
    {
        if (CheckFile()) return null;
        byte[] buffer = null;
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteInt(LanguageInfos.Count);
            for (int i = 0; i < LanguageInfos.Count; i++)
            {
                //写入语言包Id
                ms.WriteInt(LanguageInfos[i].languageId);
                //写入语言包内容
                ms.WriteUTF8String(LanguageInfos[i].chineseStr);
            }

            buffer = ms.ToArray();
        }

        return buffer;
    }
    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Bytes文件")]
    public void CreateByteFile()
    {
        byte[] buffer = ToBytes();
        FileStream fs = new FileStream(string.Format("{0}/{1}", SaveByteFilePath, FileName + ".bytes"), FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        Debug.LogError("客户端 ==> Language.bytes 文件生成完毕");
        AssetDatabase.Refresh();
    }

    public bool CheckFile()
    {
        List<int> tempIdList = new List<int>();
        tempIdList.Clear();
        for (int i = 0; i < LanguageInfos.Count; i++)
        {
            if (!(tempIdList.Contains(LanguageInfos[i].languageId)))
            {
                tempIdList.Add(LanguageInfos[i].languageId);
            }
            else
            {
                Debug.LogError("语言包编辑器 ： 语言包ID 有重复，请检查，重复ID：" + LanguageInfos[i].languageId);
                return true;
            }
        }
        Debug.LogError("语言包编辑器 ： 无重复ID");
        return false;
    }
    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建实体和管理类文件")]
    public void CreateCSFile()
    {
        CreateDBModel();
        CreateEntity();
        AssetDatabase.Refresh();
    }

    public void CreateDBModel()
    {
        if (LanguageInfos.Count <= 0)
            return;
        StringBuilder sbr = new StringBuilder();
        sbr.Append("\r\n");
        sbr.Append("//===================================================\r\n");
        sbr.Append("//作    者：Arycs \r\n");
        sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
        sbr.Append("//===================================================\r\n");
        sbr.Append("using System.Collections;\r\n");
        sbr.Append("using System.Collections.Generic;\r\n");
        sbr.Append("using System;\r\n");
        sbr.Append("using YouYou;\r\n");
        sbr.Append("\r\n");
        sbr.Append("/// <summary>\r\n");
        sbr.AppendFormat("/// 语言包-数据管理\r\n");
        sbr.Append("/// </summary>\r\n");
        sbr.AppendFormat("public partial class {0}DBModel : DataTableDBModelBase<{0}DBModel, {0}Entity>\r\n", FileName);
        sbr.Append("{\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 文件名称\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.AppendFormat("    public override string DataTableName {{ get {{ return \"{0}\"; }} }}\r\n", FileName);
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 语言字典\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public Dictionary<int,string> LanguageDic = new Dictionary<int, string>();");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 加载列表\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    protected override void LoadList(MMO_MemoryStream ms)\r\n");
        sbr.Append("    {\r\n");
        sbr.Append("        int ClassCount = ms.ReadInt();\n");
        sbr.Append("        for (int i = 0; i < ClassCount; i++)\r\n");
        sbr.Append("        {\r\n");
        sbr.Append("            LanguageDic[ms.ReadInt()] =  ms.ReadUTF8String();");
        sbr.Append("        }\r\n");
        sbr.Append("    }\r\n");
        sbr.Append("}");
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
    }
}
[Serializable]
public class LanguageInfo
{
    public int languageId;

    public string chineseStr;
}