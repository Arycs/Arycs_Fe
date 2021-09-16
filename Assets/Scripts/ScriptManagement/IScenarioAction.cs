using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本动作接口
    /// </summary>
    public interface IScenarioAction : IGameAction
    {
        /// <summary>
        /// 当前剧本
        /// </summary>
        Iscenario scenario { get; }
        
        /// <summary>
        /// 剧本状态
        /// </summary>
        ScenarioContentType status { get; }
        
        /// <summary>
        /// 运行到哪一条命令
        /// </summary>
        int token { get; }

        /// <summary>
        /// 读取剧本
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        bool LoadScenario(Iscenario scenario);
    }
}