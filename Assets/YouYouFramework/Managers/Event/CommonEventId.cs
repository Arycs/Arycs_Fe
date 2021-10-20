using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class CommonEventId
    {
        /// <summary>
        /// 注册成功事件
        /// </summary>
        public const ushort RegComplete = 10001;


        #region UITalk 相关事件

        /// <summary>
        /// 监测对话写入状态
        /// </summary>
        public const ushort UITalkWriteDown = 10011;


        /// <summary>
        /// 关闭对话界面
        /// </summary>
        public const ushort UITalkClose = 10012;

        /// <summary>
        /// 对话界面更新状态事件
        /// </summary>
        public const ushort UITalkStateUpdate = 10013;

        #endregion

        #region UIMenuOption 相关事件

        /// <summary>
        /// 等待选择界面做选择
        /// </summary>
        public const ushort UIMenuOptionDown = 10021;

        #endregion

        #region MapMenu相关事件

        /// <summary>
        /// 地图菜单点击事项
        /// </summary>
        public const ushort UIMapMenuOnClick = 10031;

        #endregion
    }
}