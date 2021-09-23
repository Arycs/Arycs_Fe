using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Arycs_Fe.Models;
using CharacterInfo = Arycs_Fe.Models.CharacterInfo;

[CreateAssetMenu(menuName = "EditorAssets/CharacterEditor")]
public class CharacterEditor : ScriptableObject, IEditorToBytes
{
    public bool IsEditor;

    [EnableIf("IsEditor")] public string FileName = "Sys_Character";

    [LabelText("生成Txt文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveTxtFilePath = "";

    [LabelText("生成Bytes文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveByteFilePath = "";

    [LabelText("生成CS文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveCSFilePath = "";

    [EnableIf("IsEditor")] [LabelText("人物信息")]
    public List<CharacterInfo> CharacterInfos = new List<CharacterInfo>();

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Txt文件")]
    public void CreateTextFile()
    {
        if (CheckFile()) return;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < CharacterInfos.Count; i++)
        {
            sb.AppendFormat("ID:{0},名称:{1},头像:{2},职业ID:{3},等级:{4},生命值:{5},魔法值:{6},幸运值:{7}",
                CharacterInfos[i].characterId, CharacterInfos[i].characterName,
                CharacterInfos[i].characterProfile, CharacterInfos[i].classId, CharacterInfos[i].characterLevel,
                CharacterInfos[i].characterHp, CharacterInfos[i].characterMp, CharacterInfos[i].characterLuk);
            sb.Append("\n");
            sb.Append("战斗属性：");
            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
            {
                sb.Append(CharacterInfos[i].characterFightProperties[j]);
                sb.Append(" ");
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
        Debug.LogErrorFormat("客户端 ==> {0}.bytes 文件生成完毕", FileName);
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

    public void CreateDBModel()
    {
        if (CharacterInfos.Count <= 0)
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
        sbr.Append("        int CharacterCount = ms.ReadInt();\n");
        sbr.Append("        for (int i = 0; i < CharacterCount; i++)\r\n");
        sbr.Append("        {\r\n");
        sbr.AppendFormat("            {0}Entity entity = new {0}Entity();\r\n", FileName);
        sbr.Append("            entity.CharacterId = ms.ReadInt();\n");
        sbr.Append("            entity.CharacterName = ms.ReadUTF8String();\n");
        sbr.Append("            entity.CharacterProfile = ms.ReadUTF8String();\n");
        sbr.Append("            entity.CharacterClassId = ms.ReadInt();\n");
        sbr.Append("            entity.CharacterLevel = ms.ReadInt();\n");
        sbr.Append("            entity.CharacterHp = ms.ReadInt();\n");
        sbr.Append("            entity.CharacterMp = ms.ReadInt();\n");
        sbr.Append("            entity.CharacterLuk = ms.ReadInt();\n");
        sbr.Append("            for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.CharacterFightProperties[j] = ms.ReadInt();\n");
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
        if (CharacterInfos.Count <= 0)
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
        sbr.Append("    /// 人物ID\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int CharacterId;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 人物名称\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string CharacterName;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 人物头像\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string CharacterProfile;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 人物职业ID\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int CharacterClassId;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 人物基本等级\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int CharacterLevel;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 人物基础血量\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int CharacterHp;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 人物基础蓝量(包含各种消耗，比如怒气，能量，并不单一指代蓝量)\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int CharacterMp;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 人物幸运值\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int CharacterLuk;\r\n");
        sbr.Append("\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 战斗属性\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public FightProperties CharacterFightProperties;\r\n");
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

    public bool CheckFile()
    {
        List<int> tempIdList = new List<int>();
        tempIdList.Clear();
        for (int i = 0; i < CharacterInfos.Count; i++)
        {
            if (!(tempIdList.Contains(CharacterInfos[i].characterId)))
            {
                tempIdList.Add(CharacterInfos[i].characterId);
            }
            else
            {
                Debug.LogError("职业编辑器 ： 人物ID 有重复，请检查，重复ID：" + CharacterInfos[i].characterId);
                return true;
            }
        }

        Debug.LogError("角色编辑器 ： 无重复ID");
        return false;
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
            ms.WriteInt(CharacterInfos.Count);
            for (int i = 0; i < CharacterInfos.Count; i++)
            {
                //写入人物ID
                ms.WriteInt(CharacterInfos[i].characterId);
                //写入人物名称
                ms.WriteUTF8String(CharacterInfos[i].characterName);
                //写入人物头像
                ms.WriteUTF8String(CharacterInfos[i].characterProfile);
                //写入人物职业ID
                ms.WriteInt(CharacterInfos[i].classId);
                //写入人物基本等级
                ms.WriteInt(CharacterInfos[i].characterLevel);
                //写入人物血量
                ms.WriteInt(CharacterInfos[i].characterHp);
                //写入人物蓝量（包括一些不同的消耗，比如怒气，能量等）
                ms.WriteInt(CharacterInfos[i].characterMp);
                //写入人物幸运值
                ms.WriteInt(CharacterInfos[i].characterLuk);
                //写入人物战斗属性
                // 力量，魔力，技巧，速度，防御，魔防
                for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
                {
                    ms.WriteInt(CharacterInfos[i].characterFightProperties[j]);
                }
            }

            buffer = ms.ToArray();
        }

        return buffer;
    }
}

namespace Arycs_Fe.Models
{
    [Serializable]
    public class CharacterInfo
    {
        /// <summary>
        /// 人物ID
        /// </summary>
        public int characterId;

        /// <summary>
        /// 名称
        /// </summary>
        public string characterName = "人物名称";

        /// <summary>
        /// 头像
        /// </summary>
        [FolderPath] public string characterProfile = "人物头像";

        /// <summary>
        /// 人物的职业ID
        /// </summary>
        public int classId;

        /// <summary>
        /// 基本等级
        /// </summary>
        public int characterLevel;

        /// <summary>
        /// 基本生命值
        /// </summary>
        public int characterHp;

        /// <summary>
        /// 基本魔法值（如果不同职业有不同副能量，还需要能量类型参数，比如怒气，能量等）
        /// </summary>
        public int characterMp;

        /// <summary>
        /// 幸运值
        /// </summary>
        public int characterLuk;

        /// <summary>
        /// 战斗属性
        /// </summary>
        public FightProperties characterFightProperties;
    }
}