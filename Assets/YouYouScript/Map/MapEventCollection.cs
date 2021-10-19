    using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 事件集合
    /// </summary>
    public class MapEventCollection
    {
        /// <summary>
        /// 所有事件
        /// </summary>
        protected readonly Dictionary<int, MapEvent> m_Events = new Dictionary<int, MapEvent>();

        /// <summary>
        /// 触发过的事件
        /// </summary>
        protected readonly Dictionary<int, MapEvent> m_TriggeredEvents = new Dictionary<int, MapEvent>();

        /// <summary>
        /// 进入地图事件
        /// </summary>
        protected readonly List<MapEvent> startEvents = new List<MapEvent>();

        /// <summary>
        /// 每回合开始事件
        /// </summary>
        public readonly List<MapEvent> turnEvents = new List<MapEvent>();

        /// <summary>
        /// 角色死亡事件
        /// </summary>
        public readonly Dictionary<int, MapEvent> deadEvents = new Dictionary<int, MapEvent>();

        /// <summary>
        /// 对话事件
        /// </summary>
        public readonly Dictionary<Vector2Int, MapEvent> roleTalkEvents = new Dictionary<Vector2Int, MapEvent>();

        /// <summary>
        /// 战斗对话事件
        /// </summary>
        public readonly Dictionary<Vector2Int, MapEvent> roleCombatTalkEvents = new Dictionary<Vector2Int, MapEvent>();

        /// <summary>
        /// 移动到坐标事件
        /// </summary>
        public readonly Dictionary<Vector3Int, MapEvent> posEvents = new Dictionary<Vector3Int, MapEvent>();

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public bool Add(MapEvent me)
        {
            if (m_Events.ContainsKey(me.id))
            {
                return false;
            }
            m_Events.Add(me.id,me);

            switch (me.entryConditionType)      
            {
                case MapEventConditionType.NoneCondition:
                    startEvents.Add(me);
                    break;
                case MapEventConditionType.TurnCondition:
                    turnEvents.Add(me);
                    break;
                case MapEventConditionType.PositionCondition:
                    PositionCondition pc = me.entryCondititon as PositionCondition;
                    posEvents.Add(new Vector3Int(pc.x,pc.y,0),me);
                    break;
                case MapEventConditionType.RoleDeadCondition :
                    RoleDeadCondition rdc = me.entryCondititon as RoleDeadCondition;
                    deadEvents.Add(rdc.characterId,me);
                    break;
                case MapEventConditionType.RoleTalkCondition:
                    RoleTalkCondition rtc = me.entryCondititon as RoleTalkCondition;
                    if (rtc.characterId == rtc.targetId)
                    {
                        return false;
                    }
                    roleTalkEvents.Add(new Vector2Int(rtc.characterId,rtc.targetId),me);
                    break;
                case MapEventConditionType.RoleCombatTalkCondition:
                    RoleCombatTalkCondition rctc = me.entryCondititon as RoleCombatTalkCondition;
                    if (rctc.characterId == rctc.targetId)
                    {
                        return false;
                    }
                    roleCombatTalkEvents.Add(new Vector2Int(rctc.characterId,rctc.targetId),me);
                    break;
                default:
                    return false;
            }

            return true;
        }
        
        public virtual IEnumerator TriggerStartEvents(MapAction action, Action<MapEvent> onTriggered = null, Action<string> onError = null)
        {
            int i = 0;
            while (i < startEvents.Count)
            {
                MapEvent me = startEvents[i];
                yield return me.Trigger(action, onTriggered, onError);
                if (me.isTriggered)
                {
                    startEvents.RemoveAt(i);
                    m_TriggeredEvents.Add(me.id, me);
                }
                else
                {
                    i++;
                }
            }
        }

        public virtual IEnumerator TriggerTurnEvents(MapAction action, Action<MapEvent> onTriggered = null, Action<string> onError = null)
        {
            int i = 0;
            while (i < turnEvents.Count)
            {
                MapEvent me = turnEvents[i];
                yield return me.Trigger(action, onTriggered, onError);
                if (me.isTriggered)
                {
                    turnEvents.RemoveAt(i);
                    m_TriggeredEvents.Add(me.id, me);
                }
                else
                {
                    i++;
                }
            }
        }

        public virtual IEnumerator TriggerDeadEvents(MapAction action, int id, Action<MapEvent> onTriggered = null, Action<string> onError = null)
        {
            MapEvent me;
            if (!deadEvents.TryGetValue(id, out me))
            {
                yield break;
            }

            yield return me.Trigger(action, onTriggered, onError);
            if (me.isTriggered)
            {
                deadEvents.Remove(id);
                m_TriggeredEvents.Add(me.id, me);
            }
        }

        public virtual IEnumerator TriggerRoleTalkEvents(MapAction action, int id, int target, Action<MapEvent> onTriggered = null, Action<string> onError = null)
        {
            Vector2Int key = new Vector2Int(id, target);
            MapEvent me;
            if (!roleTalkEvents.TryGetValue(key, out me))
            {
                yield break;
            }

            yield return me.Trigger(action, onTriggered, onError);
            if (me.isTriggered)
            {
                roleTalkEvents.Remove(key);
                m_TriggeredEvents.Add(me.id, me);
            }
        }

        public virtual IEnumerator TriggerRoleCombatTalkEvents(MapAction action, int id1, int id2, Action<MapEvent> onTriggered = null, Action<string> onError = null)
        {
            Vector2Int key = new Vector2Int(id1, id2);
            MapEvent me;
            if (!roleCombatTalkEvents.TryGetValue(key, out me))
            {
                key = new Vector2Int(id2, id1);
                if (!roleCombatTalkEvents.TryGetValue(key, out me))
                {
                    yield break;
                }
            }

            yield return me.Trigger(action, onTriggered, onError);
            if (me.isTriggered)
            {
                roleCombatTalkEvents.Remove(key);
                m_TriggeredEvents.Add(me.id, me);
            }
        }

        public virtual IEnumerator TriggerPositionEvents(MapAction action, Vector3Int position, Action<MapEvent> onTriggered = null, Action<string> onError = null)
        {
            MapEvent me;
            if (!posEvents.TryGetValue(position, out me))
            {
                yield break;
            }

            yield return me.Trigger(action, onTriggered, onError);
            if (me.isTriggered)
            {
                posEvents.Remove(position);
                m_TriggeredEvents.Add(me.id, me);
            }
        }
        
        /// <summary>
        /// 删除所有事件
        /// </summary>
        public void Clear()
        {
            m_Events.Clear();
            m_TriggeredEvents.Clear();
            startEvents.Clear();
            turnEvents.Clear();
            deadEvents.Clear();
            posEvents.Clear();
            roleTalkEvents.Clear();
            roleCombatTalkEvents.Clear();
            posEvents.Clear();
        }
    }
}