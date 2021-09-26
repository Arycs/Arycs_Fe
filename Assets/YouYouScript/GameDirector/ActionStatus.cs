using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本动作状态
    /// </summary>
    public enum ActionStatus : int
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
        
        /// <summary>
        /// 等待地图加载
        /// </summary>
        WaitMapDone,
        
        /// <summary>
        /// 返回上一个Action
        /// </summary>
        BackAction,
        
        /// <summary>
        /// 在地图中等待剧情
        /// </summary>
        WaitScenarioDone,
        
        //Others
    }

    /// <summary>
    /// 地图状态
    /// </summary>
    public enum MapStatus
    {
        /// <summary>
        /// 正常输入
        /// </summary>
        Normal,
        
        /// <summary>
        /// 菜单
        /// </summary>
        Menu,
        
        /// <summary>
        /// 移动后菜单
        /// </summary>
        SubMenu,
        
        /// <summary>
        /// 显示移动范围
        /// </summary>
        MoveCursor,
        
        /// <summary>
        /// 显示攻击范围
        /// </summary>
        AttackCursor,
        
        /// <summary>
        /// 动画
        /// </summary>
        Animation,
        
        /// <summary>
        /// 事件
        /// </summary>
        Event,
    }
}