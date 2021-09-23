using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 游戏剧情管理器
    /// </summary>
    public class GameDirectorManager : ManagerBase, IDisposable
    {
        private IGameAction m_GameAction;

        private bool isLoaded;
        public IGameAction CurrentAction
        {
            get { return m_GameAction; }
            protected set { m_GameAction = value; }
        }

        private Coroutine m_Coroutine = null;
        public override void Init()
        {
            ScenarioAction action = new ScenarioAction(m_GameAction);
            Type[] executorTypes = GameAction.GetDefaultExecutorTypesForScenarioAction().ToArray();
            action.LoadExecutors(executorTypes);
            CurrentAction = action;
            isLoaded = LoadScenario("test");
        }

        /// <summary>
        /// 读取剧本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool LoadScenario(string name)
        {
            TextAsset tempTxt = Resources.Load<TextAsset>(name);
            TxtScript txt = new TxtScript();
            txt.Load("序章", tempTxt.text);
            (CurrentAction as ScenarioAction)?.LoadScenario(txt);
            return true;
        }

        public void Update()
        {
            if (isLoaded)
            {
                CurrentAction.Update();
            }
        }

        public void Dispose()
        {
            m_GameAction?.Dispose();
        }
        /// <summary>
        /// 开始运行
        /// </summary>
        public void RunGameAction()
        {
            m_Coroutine = GameEntry.Instance.StartCoroutine(RunningGameAction());
        }

        /// <summary>
        /// 运行中
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunningGameAction()
        {
            yield return null;

            while (true)
            {
                if (CurrentAction == null)
                {
                    yield return null;
                    continue;
                }

                if (CurrentAction.Update())
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            m_Coroutine = null;
        }
        
        /// <summary>
        /// 读取地图
        /// </summary>
        /// <param name="argsSceneName"></param>
        /// <returns></returns>
        public bool LoadMap(string argsSceneName)
        {
            //TODO 事件订阅 加载新地图
            //TODO 场景的加载-切换场景
            return true;
        }

        private void LoadMap_OnSceneLoaded(string message, object sender)
        {
            //TODO 移除订阅事件
            StopGameAction();

            MapAction action = new MapAction(CurrentAction);
            if (!action.Load(ScenarioBlackboard.mapScript))
            {
                Debug.LogError(action.error);
                action.Dispose();
                return;
            }
            
            CurrentAction.Pause();
            CurrentAction = action;
        }

        public void StopGameAction()
        {
            if (m_Coroutine == null)
            {
                return;
            }
            GameEntry.Instance.StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }

        public void BackGameAction()
        {
            IGameAction old = CurrentAction;
            CurrentAction = CurrentAction.previous;
            old.Dispose();

            if (CurrentAction == null)
            {
                //TODO 返回游戏的开始场景
            }
        }
    }
}