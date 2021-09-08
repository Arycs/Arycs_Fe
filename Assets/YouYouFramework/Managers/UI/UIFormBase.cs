using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class UIFormBase : MonoBehaviour
    {
        /// <summary>
        /// 是否活跃
        /// </summary>
        protected internal bool IsActive;

        /// <summary>
        /// UI窗体的编号
        /// </summary>
        public int UIFormId { get; private set; }

        /// <summary>
        /// 分组编号
        /// </summary>
        public byte GroupId { get; private set; }

        /// <summary>
        /// 当前的画布
        /// </summary>
        public Canvas currCanvas { get; private set; }
        
        /// <summary>
        /// 关闭时间
        /// </summary>
        public float CloseTime { get; private set; }
        
        /// <summary>
        /// 禁用层级管理
        /// </summary>
        public bool DisableUILayer { get; private set; }
        
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLock { get; private set; }
        
        /// <summary>
        /// 用户数据
        /// </summary>
        public object UserData { get; private set; }
        

        private void Awake()
        {
            currCanvas = GetComponent<Canvas>();
        }

        internal void Init(int uiFormId, byte groupId, bool disableUILayer, bool isLock, object userData)
        {
            UIFormId = uiFormId;
            GroupId = groupId;
            DisableUILayer = disableUILayer;
            IsLock = isLock;
            UserData = userData;
        }

        private void Start()
        {
            OnInit(UserData);
            Open(UserData,true);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="userData">用户信息</param>
        /// <param name="isFromInit">是否从Init调用</param>
        internal void Open(object userData, bool isFromInit = false)
        {
            GameEntry.Audio.PlayAudio(ConstDefine.Audio_ButtonClick);
            if (!isFromInit)
            {
                UserData = userData;
            }

            if (!DisableUILayer)
            {
                //进行层级管理 增加层级
                GameEntry.UI.SetSortingOrder(this,true);

            }
            OnOpen(userData);
        }

        public void Close()
        {
            GameEntry.UI.CloseUIForm(this);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void ToClose()
        {
            GameEntry.Audio.PlayAudio(ConstDefine.Audio_UIClose);
            if (!DisableUILayer)
            {
                //进行层级管理 减少层级
                GameEntry.UI.SetSortingOrder(this,false);
            }
            OnClose();
            CloseTime = Time.time;
            GameEntry.UI.Enqueue(this);
        }

        private void OnDestroy()
        {
            OnBeforDestroy();
        }

        protected virtual void OnInit(object userData)
        {
        }

        protected virtual void OnOpen(object userData)
        {
        }
        
        protected virtual void OnClose()
        {
        }

        protected virtual void OnBeforDestroy()
        {
            
        }
    }
}