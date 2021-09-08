using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "EditorAssets/ItemEditor")]
public class ItemEditor : ScriptableObject, IEditorToBytes
{
    public bool IsEditor;

    [EnableIf("IsEditor")] public string FileName = "Sys_Item";

    [LabelText("生成文件路径")] [FolderPath] [EnableIf("IsEditor")]
    public string SaveFilePath = "";

    [EnableIf("IsEditor")] public List<ItemInfo> ItemInfos = new List<ItemInfo>();

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Bytes文件")]
    public void CreateByteFile()
    {
        byte[] buffer = ToBytes();
        FileStream fs = new FileStream(string.Format("{0}/{1}", SaveFilePath, FileName + ".bytes"), FileMode.Create);
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
        Debug.LogErrorFormat("客户端 ==> {0}.bytes 文件生成完毕", FileName);
    }

    public bool CheckFile()
    {
        List<int> tempIdList = new List<int>();
        tempIdList.Clear();
        for (int i = 0; i < ItemInfos.Count; i++)
        {
            if (!(tempIdList.Contains(ItemInfos[i].id)))
            {
                tempIdList.Add(ItemInfos[i].id);
            }
            else
            {
                Debug.LogError("物品编辑器 ： 物品ID 有重复，请检查，重复ID：" + ItemInfos[i].id);
                return true;
            }
        }

        Debug.LogError("物品编辑器 ： 无重复ID");
        return false;
    }

    [EnableIf("IsEditor")]
    [Button("排序")]
    public void ListSort()
    {
        if (ItemInfos.Count <= 0) return;
        ItemInfos = ItemInfos.OrderBy(s => s.id).ToList();
    }

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建Txt文件")]
    public void CreateTextFile()
    {
        if (CheckFile()) return;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < ItemInfos.Count; i++)
        {
            sb.AppendFormat("物品ID :{0}, 物品名称 :{1}, 道具描述 :{2}, 图标名称 :{3}, 价格 :{4}, 物品类型 :{5}", ItemInfos[i].id,
                ItemInfos[i].name, ItemInfos[i].desc,
                ItemInfos[i].icon, ItemInfos[i].price, ItemInfos[i].itemType.ToString());
            sb.Append("\n");
            if (ItemInfos[i].itemType == ItemType.Weapon)
            {
                sb.AppendFormat("武器等级 :{0}, 攻击力 :{1}, 最小攻击范围 :{2}, 最大攻击距离 :{3}, 重量 :{4}, 耐久度: {5}", ItemInfos[i].level,
                    ItemInfos[i].attack, ItemInfos[i].minRange, ItemInfos[i].maxRange, ItemInfos[i].weight
                    , ItemInfos[i].durability);
            }
            else if (ItemInfos[i].itemType == ItemType.Ornament)
            {
                sb.AppendFormat("生命值加成 :{0}, 魔法值加成 :{1}, 幸运值加成 :{2}, 移动力加成 :{3}", ItemInfos[i].hp, ItemInfos[i].mp,
                    ItemInfos[i].luk, ItemInfos[i].movePoint);
                sb.Append("\n");
                sb.Append("战斗属性加成：");
                for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
                {
                    sb.Append(ItemInfos[i].fightProperties[j]);
                    sb.Append(" ");
                }
            }
            else if (ItemInfos[i].itemType == ItemType.Consumable)
            {
                sb.AppendFormat("消耗品最大堆叠次数 :{0}, 使用次数 :{1}, 使用效果ID :{2}", ItemInfos[i].stackingNumber,
                    ItemInfos[i].amountUsed, ItemInfos[i].usingEffectId, ItemInfos[i].movePoint);
            }
            else
            {
                sb.Append("特殊物品，没有特殊属性");
            }

            sb.Append("\n");
            sb.Append("\n");
        }

        FileStream fs = new FileStream(string.Format("{0}\\{1}", SaveFilePath, FileName + ".txt"), FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(sb);
        sw.Close();
        Debug.LogErrorFormat("客户端表格=>" + "{0}.txt生成完毕", FileName);
    }

    [HorizontalGroup("按钮组")]
    [Button(ButtonSizes.Medium)]
    [LabelText("创建实体和管理类文件")]
    public void CreateCSFile()
    {
        CreateDBModel();
        CreateEntity();
    }

    public void CreateDBModel()
    {
        if (ItemInfos.Count <= 0)
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
        sbr.AppendFormat("/// 物品-数据管理\r\n");
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
        sbr.Append("        int ItemCount = ms.ReadInt();\n");
        sbr.Append("        for (int i = 0; i < ItemCount; i++)\r\n");
        sbr.Append("        {\r\n");
        sbr.AppendFormat("            {0}Entity entity = new {0}Entity();\r\n", FileName);
        sbr.Append("            entity.ItemId = ms.ReadInt();\n");
        sbr.Append("            entity.ItemName = ms.ReadUTF8String();\n");
        sbr.Append("            entity.ItemDesc = ms.ReadUTF8String();\n");
        sbr.Append("            entity.ItemIcon = ms.ReadUTF8String();\n");
        sbr.Append("            entity.ItemPrice = ms.ReadInt();\n");
        sbr.Append("            entity.ItemType = (ItemType)ms.ReadInt();\n");
        sbr.Append("            if (entity.ItemType == ItemType.Weapon)\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.WeaponType = (WeaponType)ms.ReadInt();\n");
        sbr.Append("                entity.Level = ms.ReadInt();\n");
        sbr.Append("                entity.Attack = ms.ReadInt();\n");
        sbr.Append("                entity.MinRange = ms.ReadInt();\n");
        sbr.Append("                entity.MaxRange = ms.ReadInt();\n");
        sbr.Append("                entity.Weight = ms.ReadInt();\n");
        sbr.Append("                entity.Durability = ms.ReadInt();\n");
        sbr.Append("            }\r\n");
        sbr.Append("            else if (entity.ItemType == ItemType.Ornament)\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.Hp = ms.ReadInt();\n");
        sbr.Append("                entity.Mp = ms.ReadInt();\n");
        sbr.Append("                for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)\n");
        sbr.Append("                {\r\n");
        sbr.Append("                    entity.FightProperties[j] = ms.ReadInt();\n");
        sbr.Append("                }\r\n");
        sbr.Append("                entity.Luk = ms.ReadInt();\n");
        sbr.Append("                entity.MovePoint = ms.ReadFloat();\n");
        sbr.Append("            }else if (entity.ItemType == ItemType.Consumable)\n");
        sbr.Append("            {\r\n");
        sbr.Append("                entity.StackingNumber = ms.ReadInt();\n");
        sbr.Append("                entity.AmountUsed = ms.ReadInt();\n");
        sbr.Append("                entity.UsingEffectId = ms.ReadInt();\n");
        sbr.Append("            }\r\n");
        sbr.Append("\r\n");
        sbr.Append("            m_List.Add(entity);\r\n");
        sbr.Append("            m_Dic[entity.ItemId] = entity;\r\n");
        sbr.Append("        }\r\n");
        sbr.Append("    }\r\n");
        sbr.Append("}");
        using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.cs", SaveFilePath, FileName),
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
        if (ItemInfos.Count <= 0)
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
        sbr.Append("    /// 物品ID\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int ItemId;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 名称\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string ItemName;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 道具描述\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string ItemDesc;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 图标\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public string ItemIcon;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 价格，当价格是-1时，不可买卖与交易\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int ItemPrice;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 分类\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public ItemType ItemType;\r\n");
        sbr.Append("    \r\n");
        sbr.Append(
            "    //=====================================================武器属性==============================================\r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 物品独有信息\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public WeaponType WeaponType;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 武器等级\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Level;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 攻击力\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Attack;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 最小攻击范围\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int MinRange;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 最大攻击范围\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int MaxRange;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 重量\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Weight;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 耐久度\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Durability;\r\n");
        sbr.Append("    \r\n");
        sbr.Append(
            "    //=====================================================饰品属性==============================================\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 生命值加成\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Hp;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 魔法值加成\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Mp;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 战斗属性加成\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public FightProperties FightProperties;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 幸运加成\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int Luk;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 移动力加成\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public float MovePoint;\r\n");
        sbr.Append("    \r\n");
        sbr.Append(
            "    //=====================================================消耗品属性==============================================\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 最大堆叠次数\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int StackingNumber;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 使用次数\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int AmountUsed;\r\n");
        sbr.Append("    \r\n");
        sbr.Append("    /// <summary>\r\n");
        sbr.Append("    /// 使用效果ID\r\n");
        sbr.Append("    /// </summary>\r\n");
        sbr.Append("    public int UsingEffectId;\r\n");
        sbr.Append("\r\n");
        sbr.Append("}\r\n");
        
        using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.cs", SaveFilePath, FileName),
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
            ms.WriteInt(ItemInfos.Count);
            for (int i = 0; i < ItemInfos.Count; i++)
            {
                //写入物品ID
                ms.WriteInt(ItemInfos[i].id);
                //写入物品名称
                ms.WriteUTF8String(ItemInfos[i].name);
                //写入道具描述
                ms.WriteUTF8String(ItemInfos[i].desc);
                //写入道具图标
                ms.WriteUTF8String(ItemInfos[i].icon);
                //写入价格
                ms.WriteInt(ItemInfos[i].price);
                //写入分类
                ms.WriteInt((int) ItemInfos[i].itemType);
                if (ItemInfos[i].itemType == ItemType.Weapon)
                {
                    //写入武器类型
                    ms.WriteInt((int) ItemInfos[i].weaponType);
                    //写入武器等级
                    ms.WriteInt(ItemInfos[i].level);
                    //写入攻击力
                    ms.WriteInt(ItemInfos[i].attack);
                    //写入最小攻击范围
                    ms.WriteInt(ItemInfos[i].minRange);
                    //写入最大攻击距离
                    ms.WriteInt(ItemInfos[i].maxRange);
                    //写入重量
                    ms.WriteInt(ItemInfos[i].weight);
                    //写入耐久度
                    ms.WriteInt(ItemInfos[i].durability);
                }
                else if (ItemInfos[i].itemType == ItemType.Ornament)
                {
                    //写入生命值加成
                    ms.WriteInt(ItemInfos[i].hp);
                    //写入魔法值加成
                    ms.WriteInt(ItemInfos[i].mp);
                    //写入战斗属性加成
                    // 力量，魔力，技巧，速度，防御，魔防
                    for (int j = 0; j < (int) FightPropertyType.MaxLength; j++)
                    {
                        ms.WriteInt(ItemInfos[i].fightProperties[j]);
                    }

                    //写入幸运加成
                    ms.WriteInt(ItemInfos[i].luk);
                    //写入移动力加成
                    ms.WriteFloat(ItemInfos[i].movePoint);
                }
                else if (ItemInfos[i].itemType == ItemType.Consumable)
                {
                    //写入最大堆叠数量
                    ms.WriteInt(ItemInfos[i].stackingNumber);
                    //写入使用次数
                    ms.WriteInt(ItemInfos[i].amountUsed);
                    //写入使用效果触发ID
                    ms.WriteInt(ItemInfos[i].usingEffectId);
                }
            }

            buffer = ms.ToArray();
        }

        return buffer;
    }
}

[System.Serializable]
public class ItemInfo
{
    /// <summary>
    /// 物品ID
    /// </summary>
    public int id = 1;

    /// <summary>
    /// 名称
    /// </summary>
    public string name = "道具名称";

    /// <summary>
    /// 道具描述
    /// </summary>
    public string desc = "道具描述";

    /// <summary>
    /// 图标
    /// </summary>
    public string icon = "图片名称";

    /// <summary>
    /// 价格，当价格是-1时，不可买卖与交易
    /// </summary>
    public int price = 1;

    /// <summary>
    /// 分类
    /// </summary>
    public ItemType itemType;

    //=====================================================武器属性==============================================
    /// <summary>
    /// 物品独有信息
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public WeaponType weaponType;

    /// <summary>
    /// 武器等级
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int level;

    /// <summary>
    /// 攻击力
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int attack;

    /// <summary>
    /// 最小攻击范围
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int minRange;

    /// <summary>
    /// 最大攻击范围
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int maxRange;

    /// <summary>
    /// 重量
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int weight;

    /// <summary>
    /// 耐久度
    /// </summary>
    [ShowIf("itemType", ItemType.Weapon)] public int durability;

    //=====================================================饰品属性==============================================

    /// <summary>
    /// 生命值加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public int hp;

    /// <summary>
    /// 魔法值加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public int mp;

    /// <summary>
    /// 战斗属性加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public FightProperties fightProperties;

    /// <summary>
    /// 幸运加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public int luk;

    /// <summary>
    /// 移动力加成
    /// </summary>
    [ShowIf("itemType", ItemType.Ornament)]
    public float movePoint;

    //=====================================================消耗品属性==============================================

    /// <summary>
    /// 最大堆叠次数
    /// </summary>
    [ShowIf("itemType", ItemType.Consumable)]
    public int stackingNumber;

    /// <summary>
    /// 使用次数
    /// </summary>
    [ShowIf("itemType", ItemType.Consumable)]
    public int amountUsed;

    /// <summary>
    /// 使用效果ID
    /// </summary>
    [ShowIf("itemType", ItemType.Consumable)]
    public int usingEffectId;
}