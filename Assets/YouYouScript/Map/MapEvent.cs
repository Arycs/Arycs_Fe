using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class MapEvent
    {
        public int id;

        /// <summary>
        /// 是否只能触发一次
        /// </summary>
        public bool onlyonce;

        /// <summary>
        /// 进入事件条件的类型
        /// </summary>
        public MapEventConditionType entryConditionType;

        /// <summary>
        /// 进入事件条件
        /// </summary>
        public Condition entryCondititon;

        /// <summary>
        /// 额外的事件条件
        /// </summary>
        public List<Condition> conditions = new List<Condition>();

        /// <summary>
        /// 事件结果
        /// </summary>
        public List<Result> triggers = new List<Result>();
        
        /// <summary>
        /// 是否已经触发过
        /// </summary>
        public bool isTriggered { get; protected set; }

        public MapEvent()
        {
            isTriggered = false;
        }

        protected bool CanConditionTrigger(Condition condition, MapAction action)
        {
            if (condition == null)
            {
                return true;
            }

            return condition.GetResult(action);
        }

        public virtual bool CanTrigger(MapAction action)
        {
            if (!CanConditionTrigger(entryCondititon,action))
            {
                return false;
            }

            if (conditions != null && conditions.Count != 0)
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (!CanConditionTrigger(conditions[i],action))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onTrigger"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        public virtual IEnumerator Trigger(MapAction action, Action<MapEvent> onTriggered = null,
            Action<string> onError = null)
        {
            // 触发过了
            if (isTriggered)
            {
                yield break;
            }

            //是否满足所有触发条件
            if (!CanTrigger(action))
            {
                yield break;
            }
            
            //触发事件
            if (triggers != null && triggers.Count != 0)
            {
                int i = 0;
                while (i < triggers.Count)
                {
                    Result trigger = triggers[i++];
                    if (trigger == null)
                    {
                        continue;
                    }

                    if (!trigger.Trigger(action))
                    {
                        if (onError != null)
                        {
                            onError($"MapEvent {id} -> Event Trigger error.");
                        }

                        isTriggered = true;
                        yield break;
                    }

                    do
                    {
                        yield return null;
                    } while (action.Status != ActionStatus.WaitInput);
                }
            }
            //如果只能触发一次，则设置触发过事件了
            if (onlyonce)
            {
                isTriggered = true;
            }

            if (onTriggered != null)
            {
                onTriggered(this);
            }
        }
    }
}