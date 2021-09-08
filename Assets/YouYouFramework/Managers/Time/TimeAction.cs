using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 定时器
    /// </summary>
    public class TimeAction
    {
        /// <summary>
        /// 定时器名字
        /// </summary>
        public string TimeName
        {
            get; private set;
        }

        /// <summary>
        /// 是否运行中
        /// </summary>
        public bool IsRuning
        {
            get; private set;
        }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool m_IsPause = false;

        /// <summary>
        /// 当前运行的时间
        /// </summary>
        private float m_CurrRunTime;

        /// <summary>
        /// 当前循环次数
        /// </summary>
        private int m_CurrLoop;

        /// <summary>
        /// 延迟时间
        /// </summary>
        private float m_DelayTime;

        /// <summary>
        /// 间隔(秒)
        /// </summary>
        private float m_Interval;

        /// <summary>
        /// 循环次数(参数中设置,-1表示无限循环 ,0也会循环一次, 1以上设置多少循环多少)
        /// </summary>
        private int m_Loop;

        /// <summary>
        /// 最后暂停时间
        /// </summary>
        private float m_LastPauseTime;

        /// <summary>
        /// 暂停了多久
        /// </summary>
        private float m_PauseTime;

        /// <summary>
        /// 开始运行
        /// </summary>
        public Action OnStartAction
        {
            get; private set;
        }

        /// <summary>
        /// 运行中, 回调参数表示剩余次数
        /// </summary>
        public Action<int> OnUpdateAction
        {
            get; private set;
        }

        /// <summary>
        /// 运行完毕
        /// </summary>
        public Action OnCompleteAction
        {
            get; private set;
        }

        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="interval">间隔</param>
        /// <param name="loop">循环次数</param>
        /// <param name="onStart">开始回调</param>
        /// <param name="onUpdate">运行回调</param>
        /// <param name="onComplete">结束回调</param>
        /// <returns></returns>
        public TimeAction Init(string timeName = null, float delayTime = 0, float interval = 0, int loop = 1, Action onStart = null, Action<int> onUpdate = null, Action onComplete = null)
        {
            TimeName = timeName;
            m_DelayTime = delayTime;
            m_Interval = interval;
            m_Loop = loop;
            OnStartAction = onStart;
            OnUpdateAction = onUpdate;
            OnCompleteAction = onComplete;

            return this;
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            //1.需要先把自己 加入到TimeManager(事件管理器)的链表中
            GameEntry.Time.RegisterTimeAction(this);
            //2.设置当前运行的时间
            m_CurrRunTime = Time.time;

            m_IsPause = false;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            m_LastPauseTime = Time.time;
            m_IsPause = true;
            Debug.LogError("暂停运行");
        }

        public void Resume()
        {
            m_IsPause = false;

            m_PauseTime = Time.time - m_LastPauseTime;
            Debug.LogError("恢复运行 暂停了m_PauseTime" + m_PauseTime);
        }

        public void Stop()
        {
            if (OnCompleteAction != null)
            {
                OnCompleteAction();
            }
            IsRuning = false;
            GameEntry.Time.RemoveTimeAction(this);
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        public void OnUpdate()
        {
            if (m_IsPause)
            {
                return;
            }

            if (Time.time > m_CurrRunTime + m_DelayTime + m_PauseTime)
            {
                if (!IsRuning)
                {
                    //当程序执行到这里的时候,表示已经第一次过了延迟时间
                    m_CurrRunTime = Time.time;
                    m_PauseTime = 0;

                    if (OnStartAction != null)
                    {
                        OnStartAction();
                    }
                }
                IsRuning = true;
            }

            if (!IsRuning)
            {
                return;
            }

            if (Time.time > m_CurrRunTime + m_PauseTime)
            {
                m_CurrRunTime = Time.time + m_Interval;
                m_PauseTime = 0;
                //以下代码 间隔m_Interval 时间执行一次
                if (OnUpdateAction != null)
                {
                    OnUpdateAction(m_Loop - m_CurrLoop);
                }

                if (m_Loop > -1)
                {
                    m_CurrLoop++;
                    if (m_CurrLoop >= m_Loop)
                    {
                        Stop();
                    }
                }
            }
        }
    }
}