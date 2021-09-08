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
        tree.AddAssetAtPath("职业编辑器", "EditorAssets/ClassEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("角色编辑器", "EditorAssets/CharacterEditor.asset").AddIcon(EditorIcons.Airplane);
        tree.AddAssetAtPath("物品编辑器", "EditorAssets/ItemEditor.asset").AddIcon(EditorIcons.Airplane);
        return tree;
    }
}
