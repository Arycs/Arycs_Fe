using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    [CreateAssetMenu]
    [ShowOdinSerializedPropertiesInInspector]
    public class MapEventInfo : ScriptableObject, ISerializationCallbackReceiver
    {
        [PropertyTooltip("结束地图的下一个场景.")] [LabelText("下一场景")]
        public string nextScene;

        [PropertyTooltip("当前地图需要执行的剧本名称.")] [LabelText("剧本名称")]
        public string scenarioName;

        [PropertyTooltip("鼠标光标信息.")] [LabelText("光标初始信息")]
        public MouseCursorInfo mouseCursor;

        [PropertyTooltip("鼠标光标名称.")] [LabelText("光标名称")]
        public string cursor;

        [BoxGroup("开始事件")][PropertyTooltip("开始事件.")] [LabelText("")]
        public MapEventNoCondition startEvent;

        [Space(20)] [PropertyTooltip("结束事件.")] [LabelText("地图结束事件")]
        public MapEventWinLose[] resultEvents;

        [Space(20)] [PropertyTooltip("地图发生的所有可能的事件.")]  [LabelText("地图事件")]
        public MapEvent[] events;

        #region OdinSerialize Meth

        [SerializeField, HideInInspector] private SerializationData serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
        }

        #endregion

        #region MouseCursorInfo

        public struct MouseCursorInfo
        {
            public string prefab;
            public int x;
            public int y;
        }

        #endregion
    }
}