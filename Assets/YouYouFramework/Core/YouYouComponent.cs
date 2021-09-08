using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// YouYou的组件基类
    /// </summary>
    public class YouYouComponent : MonoBehaviour
    {
        /// <summary>
        /// 组件实例编号
        /// </summary>
        private int m_InstanceId;
        private void Awake()
        {
            m_InstanceId = GetInstanceID();
            
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            
        }

        private void OnDestroy()
        {
            BeforOnDestroy();
        }

        protected virtual void BeforOnDestroy()
        {
            
        }

        public int InstanceId
        {
            get { return m_InstanceId; }
        }
    }
}

