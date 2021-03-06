using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class DataWindowEditor : OdinMenuEditorWindow
{
    [MenuItem("ArycsTools/DataWindow")]
    private static void OpenDataWindowEditor()
    {
        var window = GetWindow<DataWindowEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.AddAssetAtPath("职业编辑器", "YouYouScript/EditorAssets/ClassEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("角色编辑器", "YouYouScript/EditorAssets/CharacterEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("物品编辑器", "YouYouScript/EditorAssets/ItemEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("语言包编辑器", "YouYouScript/EditorAssets/LanguageEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("UI编辑器", "YouYouScript/EditorAssets/UIFormEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("移动消耗编辑器", "YouYouScript/EditorAssets/MoveConsumptionEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("New角色编辑器", "TestScripts/Role.asset").AddIcon(EditorIcons.Airplane);
        return tree;
    }
}
