using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Arycs_Fe.CombatManagement
{
    [DisallowMultipleComponent, RequireComponent(typeof(Combat))]
    public class CombatAnimaController : MonoBehaviour
    {

        #region  Event
        /// <summary>
        /// 当动画播放 开始/结束时
        /// Args : CombatAnimaController combatAnima,
        ///         bool inMap ， //是否是地图动画
        /// </summary>
        [Serializable]
        public class OnAnimaPlayEvent : UnityEvent<CombatAnimaController, bool> { }

        [SerializeField]
        private OnAnimaPlayEvent m_OnPlayEvent = new OnAnimaPlayEvent();

        /// <summary>
        /// 当动画播放开始时
        /// </summary>
        public OnAnimaPlayEvent onPlay
        {
            get{
                if (m_OnPlayEvent == null)
                {
                    m_OnPlayEvent = new OnAnimaPlayEvent();
                }
                return m_OnPlayEvent;
            }
            set { m_OnPlayEvent = value; }
        }

        private OnAnimaPlayEvent m_OnStopEvent = new OnAnimaPlayEvent();

        /// <summary>
        /// 当动画播放结束时
        /// </summary>
        public OnAnimaPlayEvent onStop
        {
            get
            {
                if (m_OnStopEvent == null)
                {
                    m_OnPlayEvent = new OnAnimaPlayEvent();
                }
                return m_OnStopEvent;
            } 
            set { m_OnPlayEvent = value; }
        }
        
        /// <summary>
        /// 当每次行动 开始/结束时
        /// Args :
        ///     CombatAnimaController combatAnima,
        ///     int index, //step 下标
        ///     float wait, // 每一次行动的动画播放时间
        ///     bool end , //step 的播放开始还是结束
        /// </summary>
        [Serializable]
        public class OnAnimaStepEvent :UnityEvent<CombatAnimaController,int,float,bool>{}
        
        [SerializeField]
        private OnAnimaStepEvent m_OnStepEvent = new OnAnimaStepEvent();
        
        /// <summary>
        /// 当每次行动开始 / 结束时
        /// </summary>
        public OnAnimaStepEvent onStep
        {
            get{
                if (m_OnStepEvent == null)
                {
                    m_OnStepEvent = new OnAnimaStepEvent();
                }

                return m_OnStepEvent;
            }
            set { m_OnStepEvent = value; }
        }

        #endregion



        private Combat m_Combat;
        public Combat combat
        {
            get
            {
                if (m_Combat == null)
                {
                    m_Combat = GetComponent<Combat>();
                }

                return m_Combat;
            }
        }

        [SerializeField] private float m_AnimationInterval = 1f;

        /// <summary>
        /// 每个动画的间隔时间
        /// </summary>
        public float animationInterval
        {
            get { return m_AnimationInterval; }
            set { m_AnimationInterval = value; }
        }

        private Coroutine m_AnimaCoroutine;

        public bool isAnimaRuning
        {
            get { return m_AnimaCoroutine != null; }
        }

        public bool isCombatLoaded
        {
            get { return combat.isLoaded; }
        }

        public bool isBattleCalced
        {
            get { return combat.stepCount > 0; }
        }

        public int stepCount
        {
            get { return combat.stepCount; }
        }

        public bool LoadCombatUnit(MapClass mapClass0, MapClass mapClass1)
        {
            return combat.LoadCombatUnit(mapClass0, mapClass1);
        }

        /// <summary>
        /// 运行动画
        /// </summary>
        /// <param name="isMap"></param>
        public void PlayAnimas(bool inMap)
        {
            if (combat == null || !isCombatLoaded || isAnimaRuning)
            {
                return;
            }

            //如果没有计算则现计算
            if (!isBattleCalced)
            {
                combat.BattleBegin();
                if (!isBattleCalced)
                {
                    Debug.LogError("CombatAnimaController -> calculate error! check the `Combat` code");
                    return;
                }
            }

            m_AnimaCoroutine = StartCoroutine(RunningAnimas(inMap));
        }

        private IEnumerator RunningAnimas(bool inMap)
        {
            onPlay?.Invoke(this,inMap);
            if (inMap)
            {
                //在地图中
                yield return RunningAnimasInMap();
            }
            else
            {
                //单独场景 UI
            }

            m_AnimaCoroutine = null;
        }

        private IEnumerator RunningAnimasInMap()
        {
            CombatUnit unit0 = combat.GetCombatUnit(0);
            CombatUnit unit1 = combat.GetCombatUnit(1);
            List<CombatStep> steps = combat.steps;

            Direction[] dirs = new Direction[2];
            dirs[0] = GetAnimaDirectionInMap(unit0.mapClass.cellPosition, unit1.mapClass.cellPosition);
            dirs[1] = GetAnimaDirectionInMap(unit1.mapClass.cellPosition, unit0.mapClass.cellPosition);
            yield return null;
            int curIndex = 0;
            CombatStep step;
            while (curIndex < steps.Count)
            {
                step = steps[curIndex];
                //根据动画不懂，播放时间应该是不同的
                //者需要一些参数或者算法来控制
                //（例如一些魔法，在配置表中加上一个特殊的变量）
                //人物事发动画是这个，特效还要另算，需要计算在内
                // 这里只是简单的定义为同时播放

                float len0 = RunAnimAndGetLengthInMap(step.atkVal, step.defVal, dirs);
                float len1 = RunAnimAndGetLengthInMap(step.defVal, step.atkVal, dirs);
                float wait = Mathf.Max(len0, len1);
                onStep?.Invoke(this,curIndex,wait,false);
                yield return new WaitForSeconds(wait);
                onStep?.Invoke(this,curIndex,wait,true);
                yield return new WaitForSeconds(animationInterval);
                curIndex++;
            }
        }

        /// <summary>
        /// 运行动画，并返回长度
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="other"></param>
        /// <param name="dirs"></param>
        /// <returns></returns>
        private float RunAnimAndGetLengthInMap(CombatVariable actor, CombatVariable other, Direction[] dirs)
        {
            CombatUnit actorUnit = combat.GetCombatUnit(actor.position);
            if (actorUnit == null || actorUnit.mapClass == null)
            {
                return 0f;
            }

            ClassAnimatorController actorAnima = actorUnit.mapClass.animatorController;
            Direction dir = dirs[actor.position];
            float length = 0.5f;
            switch (actor.animaType)
            {
                case CombatAnimaType.Prepare:
                    actorAnima.PlayPrepareAttack(dir, actorUnit.weaponType);
                    break;
                case CombatAnimaType.Attack:
                case CombatAnimaType.Heal:
                    actorAnima.PlayAttack();
                    length = actorAnima.GetAttackAnimationLength(dir, actorUnit.weaponType);
                    break;
                case CombatAnimaType.Evade:
                    actorAnima.PlayEvade();
                    length = actorAnima.GetEvadeAnimationLength(dir);
                    break;
                case CombatAnimaType.Damage:
                    actorAnima.PlayDamage();
                    length = actorAnima.GetDamageAnimationLength(dir);
                    //TODO 收到暴击的动画是额外的
                    break;
                case CombatAnimaType.Dead:
                    //TODO 播放死亡动画
                    break;
                default:
                    break;
            }

            return length;
        }

        protected Direction GetAnimaDirectionInMap(Vector3Int cellPosition0, Vector3Int cellPosition1)
        {
            Vector3Int offset = cellPosition1 - cellPosition0;
            if (Mathf.Abs(offset.x) < Mathf.Abs(offset.y))
            {
                return offset.y > 0 ? Direction.Up : Direction.Down;
            }
            else
            {
                return offset.x > 0 ? Direction.Right : Direction.Left;
            }
        }
    }
}