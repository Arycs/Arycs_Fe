using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本动作状态
    /// </summary>
    public enum ScenarioActionStatus : int
    {
        /// <summary>
        /// 错误
        /// </summary>
        Error = -1,
        
        /// <summary>
        /// 继续
        /// </summary>
        Continue = 0,
        
        /// <summary>
        /// 下一帧
        /// </summary>
        NextFrame,
        
        /// <summary>
        /// 等待输入
        /// </summary>
        WaitInput,
        
        /// <summary>
        /// 等待文本写入完成
        /// </summary>
        WaitWriteTextDone,
        
        /// <summary>
        /// 等待计时器结束
        /// </summary>
        WaitTimerTimeOut,
        
        /// <summary>
        /// 等待菜单选择
        /// </summary>
        WaitMenuOption,
        
        //Others
    }
}