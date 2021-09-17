using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本内容类型
    /// </summary>
    public enum ScenarioContentType
    {
        /// <summary>
        /// 剧本动作
        /// </summary>
        Action,
        
        /// <summary>
        /// 剧本标识
        /// </summary>
        Flag,
    }

    /// <summary>
    /// 格式化结果
    /// </summary>
    public enum FormatContentResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,
        
        /// <summary>
        /// 失败
        /// </summary>
        Failure,
        
        /// <summary>
        /// 只有注释
        /// </summary>
        Commenting
    }

    /// <summary>
    /// 剧本中的一条内容相关信息
    /// </summary>
    public interface IScenarioContent 
    {
        /// <summary>
        /// 剧本内容类型
        /// </summary>
        ScenarioContentType type { get; }

        /// <summary>
        /// 关键字或剧情标识
        /// </summary>
        string code { get; }

        /// <summary>
        /// 参数数量
        /// </summary>
        int length { get; }

        /// <summary>
        /// 参数索引器
        /// </summary>
        /// <param name="index"></param>
        string this[int index] { get; }

        /// <summary>
        /// 行号，可能会用到
        /// </summary>
        int lineNo { get; }
    }
}