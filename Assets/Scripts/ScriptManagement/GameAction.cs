using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    [Serializable]
    public class GameAction : IGameAction
    {
        #region Fields

        private bool m_DebugInfo = false;

        private IGameAction m_Previous = null;
        protected string m_Error = null;

        #endregion

        #region Properties

        /// <summary>
        /// 是否输出Debug信息
        /// </summary>
        public bool debugInfo
        {
            get { return m_DebugInfo; }
            set { m_DebugInfo = value; }
        }

        public IGameAction previous
        {
            get { return m_Previous; }
            set { m_Previous = value; }
        }

        public string error
        {
            get { return m_Error; }
            set { m_Error = value; }
        }

        public delegate void OnGameActionDelegate(IGameAction action, params object[] actionParams);

        public event OnGameActionDelegate onAbort;
        
        public virtual void Abort(params object[] abortParams)
        {
            InvokeOnAbort(abortParams);
        }

        protected void InvokeOnAbort(params object[] abortParams)
        {
            if (onAbort != null)
            {
                onAbort(this, abortParams);
            }
        }

        public virtual void OnMouseMove(Vector3 mousePosition)
        {
            throw new NotImplementedException();
        }

        public virtual void OnMouseLButtonDown(Vector3 mousePosition)
        {
            throw new NotImplementedException();
        }

        public virtual void OnMouseLButtonUp(Vector3 mousePosition)
        {
            throw new NotImplementedException();
        }

        public virtual void OnMouseRButtonDown(Vector3 mousePosition)
        {
            throw new NotImplementedException();
        }

        public virtual void OnMouseRButtonUp(Vector3 mousePosition)
        {
            throw new NotImplementedException();
        }

        public virtual void Pause()
        {
            throw new NotImplementedException();
        }

        public virtual void Resume()
        {
            throw new NotImplementedException();
        }

        public virtual bool Update()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Constructor

        public GameAction()
        {
        }

        public GameAction(IGameAction previous)
        {
            m_Previous = previous;
        }

        #endregion

        public virtual void Dispose()
        {
            m_DebugInfo = false;
            m_Previous = null;
            m_Error = null;
            onAbort = null;
        }
    }
}