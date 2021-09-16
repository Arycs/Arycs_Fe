using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 游戏动作接口
    /// </summary>
    public interface IGameAction : IDisposable
    {
        /// <summary>
        /// 是否打印信息
        /// </summary>
        bool debugInfo { get; set; }

        /// <summary>
        /// 上一个Action
        /// </summary>
        IGameAction previous { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        string error { get; }

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <param name="abortParams"></param>
        void Abort(params object[] abortParams);

        //输入操作，比如等待点击菜单选项等 
        //TODO 目前只有鼠标左右键，其余输入操作以后添加
        void OnMouseMove(Vector3 mousePosition);
        void OnMouseLButtonDown(Vector3 mousePosition);
        void OnMouseLButtonUp(Vector3 mousePosition);
        void OnMouseRButtonDown(Vector3 mousePosition);
        void OnMouseRButtonUp(Vector3 mousePosition);

        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();

        /// <summary>
        /// 重启
        /// </summary>
        void Resume();

        /// <summary>
        /// 美珍运行 ： true 继续运行，false终止运行
        /// </summary>
        /// <returns></returns>
        bool Update();
    }
}