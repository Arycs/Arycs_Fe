using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本接口
    /// </summary>
    public interface Iscenario
    {
        /// <summary>
        /// 剧本名称
        /// </summary>
        string name { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        string formatError { get; }

        /// <summary>
        /// 是否读取过剧本文本
        /// </summary>
        bool isLoaded { get; }

        /// <summary>
        /// 内容数量
        /// </summary>
        int contentCount { get; }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IScenarioContent GetContent(int index);

        /// <summary>
        /// 格式化剧本
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scriptText"></param>
        /// <returns></returns>
        bool Load(string fileName, string scriptText);
    }
}