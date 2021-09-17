using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧情内容解析器接口
    /// </summary>
    public interface IScenarioContentExecutor
    {
        /// <summary>
        /// 命令keycode
        /// </summary>
        string code { get; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="gameAction"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        ScenarioActionStatus Execute(IGameAction gameAction, IScenarioContent content,out string error);
        
        
    }
}