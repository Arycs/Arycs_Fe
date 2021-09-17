using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class GameDirector : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool m_DebugInfo = true;

        [SerializeField] private string m_FirstScenario = "main";

        [SerializeField] private bool m_FirstScenarioIsTxt = true;

        private IGameAction m_GameAction = null;
        private Coroutine m_Coroutine = null;

        #endregion

        #region Properties

        /// <summary>
        /// 是否输出日志
        /// </summary>
        public bool debugInfo
        {
            get => m_DebugInfo;
            set => m_DebugInfo = value;
        }
        
        /// <summary>
        /// 第一个剧本
        /// </summary>
        public string firstScenario
        {
            get => m_FirstScenario;
            set => m_FirstScenario = value;
        }

        /// <summary>
        /// 是否是Txt
        /// </summary>
        public bool firstScenarioIsTxt
        {
            get => m_FirstScenarioIsTxt;
            set => m_FirstScenarioIsTxt = value;
        }

        #endregion
        
        
    }
}