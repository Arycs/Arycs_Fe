using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class MacroSettings : ScriptableObject
{
    private string m_Macor;

    [BoxGroup("MacroSettings")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    [HideLabel]
    public MacroData[] Settings;

    //ButtonSizes.Medium 这个属性表示按钮的高度
    //ResponsiveButtonGroup("DefaultButtonSize") 有这句话按钮会排序
    //ResponsiveButtonGroup 表示按钮的分组 如果分组名字一样，横着排序，如果不一样则竖着排序
    //PropertyOrder 表示当前回之的所有组件的顺序

    [Button(ButtonSizes.Medium), ResponsiveButtonGroup("DefaultButtonSize"), PropertyOrder(1)]
    public void SaveMacro()
    {
#if UNITY_EDITOR
        m_Macor = string.Empty;
        foreach (var item in Settings)
        {
            if (item.Enable)
            {
                m_Macor += string.Format("{0};", item.Macro);
            }

            if (item.Macro.Equals("DISABLE_ASSETBUNDLE",StringComparison.CurrentCultureIgnoreCase))
            {
                EditorBuildSettingsScene[] arrScene = EditorBuildSettings.scenes;
                for (int i = 0; i < arrScene.Length; i++)
                {
                    if (arrScene[i].path.IndexOf("download",StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        arrScene[i].enabled = item.Enable;
                    }
                }
                EditorBuildSettings.scenes = arrScene;
            }
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, m_Macor);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, m_Macor);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, m_Macor);
        Debug.LogError("Save Macro Sucess");
#endif
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        m_Macor = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        for (int i = 0; i < Settings.Length; i++)
        {
            if (!string.IsNullOrEmpty(m_Macor) && m_Macor.IndexOf(Settings[i].Macro) != -1)
            {
                Settings[i].Enable = true;
            }
            else
            {
                Settings[i].Enable = false;
            }
        }
#endif
    }
}

[Serializable]
public class MacroData
{
    /// <summary>
    /// 启用
    /// </summary>
    [TableColumnWidth(80, Resizable = false)]
    public bool Enable;

    /// <summary>
    /// 宏名称
    /// </summary>
    public string Name;

    /// <summary>
    /// 宏
    /// </summary>
    public string Macro;
}