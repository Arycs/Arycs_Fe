using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/MoveConsumptionEditor")]
public class MoveConsumptionEditor : ScriptableObject, IEditorToBytes
{
    public bool IsEditor;

    [EnableIf("IsEditor")] public string FileName = "Sys_MoveConsumption";

    [LabelText("生成Txt文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveTxtFilePath = "";

    [LabelText("生成Bytes文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveByteFilePath = "";

    [LabelText("生成CS文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveCSFilePath = "";


    [EnableIf("IsEditor")] [LabelText("语言包信息")]
    public List<MoveConsumption> MoveConsumptionInfos = new List<MoveConsumption>();

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Txt文件")]
    public void CreateTextFile()
    {
        if (CheckFile()) return;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < MoveConsumptionInfos.Count; i++)
        {
            sb.AppendFormat("职业类型 : {0} \n", MoveConsumptionInfos[i].classType.ToString());
            for (int j = 0; j < MoveConsumptionInfos[i].consumptions.Length; j++)
            {
                sb.AppendFormat("{0}", MoveConsumptionInfos[i].consumptions[j]);
                sb.Append("\t");
            }

            sb.Append("\n");
        }

        FileStream fs = new FileStream(string.Format("{0}\\{1}", SaveTxtFilePath, FileName + ".txt"), FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(sb);
        sw.Close();
        Debug.LogErrorFormat("客户端表格=>" + "{0}.txt生成完毕", FileName);
        AssetDatabase.Refresh();
    }

    public byte[] ToBytes()
    {
        if (CheckFile()) return null;
        byte[] buffer = null;
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteInt(MoveConsumptionInfos.Count);
            for (int i = 0; i < MoveConsumptionInfos.Count; i++)
            {
                //写入职业类型
                ms.WriteInt((int) MoveConsumptionInfos[i].classType);
                //写入数组长度
                ms.WriteInt(MoveConsumptionInfos[i].consumptions.Length);
                for (int j = 0; j < MoveConsumptionInfos[i].consumptions.Length; j++)
                {
                    ms.WriteFloat(MoveConsumptionInfos[i].consumptions[j]);
                }
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
        FileStream fs = new FileStream(string.Format("{0}/{1}", SaveByteFilePath, FileName + ".bytes"),
            FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        Debug.LogError("客户端 ==> MoveConsumption.bytes 文件生成完毕");
        AssetDatabase.Refresh();
    }

    public bool CheckFile()
    {
        List<int> tempIdList = new List<int>();
        tempIdList.Clear();
        for (int i = 0; i < MoveConsumptionInfos.Count; i++)
        {
            if (!(tempIdList.Contains((int) MoveConsumptionInfos[i].classType)))
            {
                tempIdList.Add((int) MoveConsumptionInfos[i].classType);
            }
            else
            {
                Debug.LogError("移动消耗编辑器 ： 职业类型 有重复，请检查，重复职业类型：" + MoveConsumptionInfos[i].classType.ToString());
                return true;
            }
        }

        Debug.LogError("移动消耗编辑器 ： 无重复职业");
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
        if (MoveConsumptionInfos.Count <= 0)
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
        sbr.Append("    /// 加载列表\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    protected override void LoadList(MMO_MemoryStream ms)\r\n");
        sbr.Append("    {\r\n");
        sbr.Append("        int ClassCount = ms.ReadInt();\n");
        sbr.Append("        for (int i = 0; i < ClassCount; i++)\r\n");
        sbr.Append("        {\r\n");
        sbr.Append("            Sys_MoveConsumptionEntity entity = new Sys_MoveConsumptionEntity();\r\n");
        sbr.Append("            entity.classType = (ClassType)ms.ReadInt();\r\n");
        sbr.Append("            int Length = ms.ReadInt();\r\n");
        sbr.Append("            entity.consumptions = new float[Length];\r\n");
        sbr.Append("            for(int j = 0; j < Length; j++)\r\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.consumptions[j] = ms.ReadFloat();\r\n");
        sbr.Append("            }\r\n");
        sbr.Append("\r\n");
        sbr.Append("            m_List.Add(entity);\r\n");
        sbr.Append("            m_Dic[(int)entity.classType] = entity;\r\n");
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
        if (MoveConsumptionInfos.Count <= 0)
            return;
        StringBuilder sbr = new StringBuilder();
        sbr.Append("\r\n");
        sbr.Append("//===================================================\r\n");
        sbr.Append("//作    者：Arycs \r\n");
        sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
        sbr.Append("//===================================================\r\n");
        sbr.Append("using System.Collections;\r\n");
        sbr.Append("using YouYou;\r\n");
        sbr.Append("\r\n");
        sbr.Append("/// <summary>\r\n");
        sbr.AppendFormat("/// {0}实体\r\n", FileName);
        sbr.Append("/// </summary>\r\n");
        sbr.AppendFormat("public partial class {0}Entity : DataTableEntityBase\r\n", FileName);
        sbr.Append("{\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 职业类型\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public ClassType classType;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 在各个地形的移动消耗具体数值\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public float[] consumptions;\r\n");
        sbr.Append("    \r\n");
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
}

[Serializable]
public class MoveConsumption
{
    /// <summary>
    /// 职业类型
    /// </summary>
    public ClassType classType;

    /// <summary>
    /// 在各个地形的移动消耗具体数值
    /// </summary>
    public float[] consumptions;
}