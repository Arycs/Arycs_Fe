using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public abstract class YouYouBaseComponent : YouYouComponent
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            //注册组件,将自己加入基础组件列表
            GameEntry.RegisterBaseComponent(this);
        }

        /// <summary>
        /// 关闭方法
        /// </summary>
        public abstract void Shutdown();
    }
}