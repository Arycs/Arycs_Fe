using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arycs_Fe.Models;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/ClassEditor")]
public class ClassEditor : ScriptableObject, IEditorToBytes
{
    public bool IsEditor;
    
    [EnableIf("IsEditor")]
    public string FileName = "Sys_Class";

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
    [LabelText("职业信息")]
    public List<ClassInfo> ClassInfos = new List<ClassInfo>();

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Txt文件")]
    public void CreateTextFile()
    {
        if (CheckFile()) return;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < ClassInfos.Count; i++)
        {
            sb.AppendFormat("职业ID:{0},预制体名称:{1},动画名称:{2},职业名称:{3},移动点数:{4}", ClassInfos[i].id, ClassInfos[i].prefab,
                ClassInfos[i].animator, ClassInfos[i].name, ClassInfos[i].movePoint);
            sb.Append("\n");
            sb.Append("战斗属性：");
            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
            {
                sb.Append(ClassInfos[i].fightProperties[j]);
                sb.Append(" ");
            }

            sb.Append("\n");
            sb.Append("最大战斗属性：");
            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
            {
                sb.Append(ClassInfos[i].maxFightProperties[j]);
                sb.Append(" ");
            }

            sb.Append("\n");
            sb.Append("武器属性：");
            for (int j = 0; j < (int) WeaponType.MaxLength; j++)
            {
                sb.Append(ClassInfos[i].availableWeapons[j]);
                sb.Append(" ");
            }

            sb.Append("\n");
        }

        FileStream fs = new FileStream(string.Format("{0}\\{1}", SaveTxtFilePath, FileName + ".txt"), FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(sb);
        sw.Close();
        Debug.LogError("客户端表格=>" + "ClassInfo.txt生成完毕");
        AssetDatabase.Refresh();
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
        Debug.LogError("客户端 ==> ClassInfo.bytes 文件生成完毕");
        AssetDatabase.Refresh();
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
    public byte[] ToBytes()
    {
        if (CheckFile())
        {
            return null;
        }

        byte[] buffer = null;
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteInt(ClassInfos.Count);
            for (int i = 0; i < ClassInfos.Count; i++)
            {
                //写入职业ID
                ms.WriteInt(ClassInfos[i].id);
                //写入预制体名称
                ms.WriteUTF8String(ClassInfos[i].prefab);
                //写入动画名称
                ms.WriteUTF8String(ClassInfos[i].animator);
                //写入职业名称
                ms.WriteUTF8String(ClassInfos[i].name);
                //写入职业类型
                ms.WriteInt((int) ClassInfos[i].classType);
                //写入移动点数
                ms.WriteFloat(ClassInfos[i].movePoint);
                //写入战斗属性
                // 力量，魔力，技巧，速度，防御，魔防
                for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
                {
                    ms.WriteInt(ClassInfos[i].fightProperties[j]);
                }

                //写入战斗最大属性
                for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
                {
                    ms.WriteInt(ClassInfos[i].maxFightProperties[j]);
                }

                //写入可用武器 剑，枪，斧，弓，杖
                for (int j = 0; j < (int) WeaponType.MaxLength; j++)
                {
                    ms.WriteInt(ClassInfos[i].availableWeapons[j]);
                }
            }

            buffer = ms.ToArray();
        }

        return buffer;
    }
    public bool CheckFile()
    {
        List<int> tempIdList = new List<int>();
        tempIdList.Clear();
        for (int i = 0; i < ClassInfos.Count; i++)
        {
            if (!(tempIdList.Contains(ClassInfos[i].id)))
            {
                tempIdList.Add(ClassInfos[i].id);
            }
            else
            {
                Debug.LogError("职业编辑器 ： 职业ID 有重复，请检查，重复ID：" + ClassInfos[i].id);
                return true;
            }
        }

        Debug.LogError("职业编辑器 ： 无重复ID");
        return false;
    }
    public void CreateDBModel()
    {
        if (ClassInfos.Count <= 0)
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
        sbr.AppendFormat("/// 职业-数据管理\r\n");
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
        sbr.AppendFormat("            {0}Entity entity = new {0}Entity();\r\n", FileName);
        sbr.Append("            entity.Id = ms.ReadInt();\n");
        sbr.Append("            entity.Prefab = ms.ReadUTF8String();\n");
        sbr.Append("            entity.AnimatorName = ms.ReadUTF8String();\n");
        sbr.Append("            entity.ClassName = ms.ReadUTF8String();\n");
        sbr.Append("            entity.ClassType = (ClassType)ms.ReadInt();\n");
        sbr.Append("            entity.MovePoint = ms.ReadFloat();\n");
        sbr.Append("            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.FightProperties[j] = ms.ReadInt();\n");
        sbr.Append("            }\r\n");
        sbr.Append("            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.MaxFightProperties[j] = ms.ReadInt();\n");
        sbr.Append("            }\r\n");
        sbr.Append("            for (int j = 0; j < (int) WeaponType.MaxLength; j++)\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.AvailableWeapons[j] = ms.ReadInt();\n");
        sbr.Append("            }\r\n");
        sbr.Append("\r\n");
        sbr.Append("            m_List.Add(entity);\r\n");
        sbr.Append("            m_Dic[entity.Id] = entity;\r\n");
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
        if (ClassInfos.Count <= 0)
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
        sbr.Append("    /// 职业ID\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Id;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 职业预制体\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string Prefab;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 动画名称\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string AnimatorName;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 职业名称\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string ClassName;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 职业类型\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public ClassType ClassType;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 移动力\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public float MovePoint;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 战斗属性\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public FightProperties FightProperties;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 最大战斗属性\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public FightProperties MaxFightProperties;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 武器属性\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public AvailableWeapons AvailableWeapons;\r\n");
        sbr.Append("\r\n");
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