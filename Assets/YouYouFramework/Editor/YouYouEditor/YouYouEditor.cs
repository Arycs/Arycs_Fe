using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class YouYouEditor : OdinMenuEditorWindow
{
    [MenuItem("YouYouTools/YouYouEditor")]
    private static void OpenYouYouEditor()
    {
        var window = GetWindow<YouYouEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.AddAssetAtPath("YouYouFramework", "YouYouFramework/YouYouAssets/AboutUs.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("MacroSettings", "YouYouFramework/YouYouAssets/MacroSettings.asset").AddIcon(EditorIcons.AlertCircle);;
        tree.AddAssetAtPath("ParamsSettings", "YouYouFramework/YouYouAssets/ParamsSettings.asset").AddIcon(EditorIcons.Letter);
        tree.AddAssetAtPath("AssetBundleSettings", "YouYouFramework/YouYouAssets/AssetBundleSettings.asset").AddIcon(EditorIcons.List);
        tree.AddAssetAtPath("ShareDataSettings", "YouYouFramework/YouYouAssets/ShareDataSettings.asset").AddIcon(EditorIcons.Clouds);
        tree.AddAssetAtPath("PoolAnalyze/PoolAnalyze_AssetBundlePool", "YouYouFramework/YouYouAssets/PoolAnalyze_AssetBundlePool.asset").AddIcon(EditorIcons.CharGraph);
        tree.AddAssetAtPath("PoolAnalyze/PoolAnalyze_AssetPool", "YouYouFramework/YouYouAssets/PoolAnalyze_AssetPool.asset").AddIcon(EditorIcons.Link);
        tree.AddAssetAtPath("PoolAnalyze/PoolAnalyze_ClassObjectPool", "YouYouFramework/YouYouAssets/PoolAnalyze_ClassObjectPool.asset").AddIcon(EditorIcons.FileCabinet);
        return tree;
    }
}
