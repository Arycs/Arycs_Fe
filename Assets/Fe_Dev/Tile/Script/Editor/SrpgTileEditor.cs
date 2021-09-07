using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    [CustomEditor(typeof(SrpgTile))]
    [CanEditMultipleObjects]
    public class SrpgTileEditor : RuleTileEditor
    {
        public SrpgTile srpgTile {
            get { return target as SrpgTile;}
        }

        public override void OnInspectorGUI()
        {
            //渲染新增的数据
            EditorGUI.BeginChangeCheck();
            srpgTile.terrainType = (TerrainType) EditorGUILayout.EnumPopup("Terrain Type", srpgTile.terrainType);
            srpgTile.avoidRate = EditorGUILayout.IntSlider("Avoid Rate", srpgTile.avoidRate, -100, 100);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            
            // 渲染RuleTile的内容
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}