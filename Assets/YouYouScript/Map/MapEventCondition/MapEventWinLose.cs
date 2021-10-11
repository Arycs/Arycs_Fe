using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    [Serializable]
    public class MapEventWinLose : MapEvent
    {
        public Result resultTrigger;

        public override bool CanTrigger(MapAction action)
        {
            if (!onlyonce)
            {
                onlyonce = true;
            }

            return base.CanTrigger(action);
        }
        
        public override IEnumerator Trigger(MapAction action, Action<MapEvent> onTriggered = null, Action<string> onError = null)
        {
            // 触发过了
            if (isTriggered)
            {
                yield break;
            }

            // 是否满足所有触发条件
            if (!CanTrigger(action))
            {
                yield break;
            }

            // 触发事件
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
                            onError(string.Format("MapEvent {0} -> Event Trigger error.", id));
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

            if ((resultTrigger == null)
                || (resultTrigger.type != MapEventResultType.WinResult && resultTrigger.type != MapEventResultType.LoseResult)
                || (!resultTrigger.Trigger(action)))
            {
                if (onError != null)
                {
                    onError(string.Format("MapEventWinLose {0} -> Event Trigger error.", id));
                }
                isTriggered = true;
                action.MapEndCommand(-1);
                yield break;
            }

            // 如果只能触发一次，则设置触发过事件了
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