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

        public IGameAction CurrentAction
        {
            get { return m_GameAction; }
            protected set { m_GameAction = value; }
        }

        public override void Init()
        {
            ScenarioAction action = new ScenarioAction(m_GameAction);
            Type[] executorTypes = GameAction.GetDefaultExecutorTypesForScenarioAction().ToArray();
            action.LoadExecutors(executorTypes);
            CurrentAction = action;
        }

        /// <summary>
        /// 读取剧本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool LoadScenario(string name)
        {
            TxtScript txt = new TxtScript();
            return true;
        }

        public void Dispose()
        {
            m_GameAction?.Dispose();
        }

        public bool LoadMap(string argsSceneName)
        {
            throw new NotImplementedException();
        }
    }
}