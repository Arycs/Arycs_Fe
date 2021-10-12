using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu]
public class RoleEditor : ScriptableObject
{
    [FolderPath]
    public string Path;
    
    [BoxGroup("AboutUs")]
    [HorizontalGroup("AboutUs/Split", 160)]
    [VerticalGroup("AboutUs/Split/Left")]
    [HideLabel,PreviewField(160,ObjectFieldAlignment.Center)]
    public Texture characterHeadIcon;

    /// <summary>
    /// 名称
    /// </summary>
    [BoxGroup("AboutUs")]
    [HorizontalGroup("AboutUs/Split")]
    [VerticalGroup("AboutUs/Split/Right")]
    public string characterName = "人物名称";
    
    /// <summary>
    /// 人物的职业ID
    /// </summary>
    public int classId;

    /// <summary>
    /// 基本等级
    /// </summary>
    [BoxGroup("AboutUs")]
    [HorizontalGroup("AboutUs/Split")]
    [VerticalGroup("AboutUs/Split/Right")]
    public int characterLevel;

    /// <summary>
    /// 基本生命值
    /// </summary>
    [BoxGroup("AboutUs")]
    [HorizontalGroup("AboutUs/Split")]
    [VerticalGroup("AboutUs/Split/Right")]
    public int characterHp;

    /// <summary>
    /// 基本魔法值（如果不同职业有不同副能量，还需要能量类型参数，比如怒气，能量等）
    /// </summary>
    [BoxGroup("AboutUs")]
    [HorizontalGroup("AboutUs/Split")]
    [VerticalGroup("AboutUs/Split/Right")]
    public int characterMp;

    /// <summary>
    /// 幸运值
    /// </summary>
    [BoxGroup("AboutUs")]
    [HorizontalGroup("AboutUs/Split")]
    [VerticalGroup("AboutUs/Split/Right")]
    public int characterLuk;

    /// <summary>
    /// 战斗属性
    /// </summary>
    public FightProperties characterFightProperties;
    
    [Button]
    private void CreateAsset()
    {
        ScriptableObject m_scriptableObject = CreateInstance<RoleEditor>();
        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
        }
        string m_SavePath = string.Format("{0}/{1}.asset", Path, characterName);
        //创建asset
        AssetDatabase.CreateAsset(m_scriptableObject,m_SavePath);
    }
}
