using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    [CreateAssetMenu]
    public class MapEventInfo: ScriptableObject
    {
        [PropertyTooltip("地图事件.")]
        public MapEvent[] events;
        
        [PropertyTooltip("结束地图的下一个场景.")]
        public string nextScene;

        [PropertyTooltip("剧本名称.")]
        public string scenarioName;

        [PropertyTooltip("鼠标光标信息.")]
        public MouseCursorInfo mouseCursor;

        [PropertyTooltip("鼠标光标名称.")]
        public string cursor;
        
        [Serializable]
        public struct MouseCursorInfo
        {
            public string prefab;
            public int x;
            public int y;
        }

        [PropertyTooltip("开始事件.")]
        public MapEventNoCondition startEvent;

        [PropertyTooltip("结束事件.")]
        public MapEventWinLose[] resultEvents;

    }
}