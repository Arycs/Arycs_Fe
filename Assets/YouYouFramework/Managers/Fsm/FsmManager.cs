using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 状态机管理器
    /// </summary>
    public class FsmManager : ManagerBase,IDisposable
    {
        /// <summary>
        /// 状态机的字典
        /// </summary>
        private Dictionary<int, FsmBase> m_FsmDic;

        /// <summary>
        /// 状态机的临时编号
        /// </summary>
        public int m_TempFsmId = 0;

        public FsmManager()
        {
            m_FsmDic = new Dictionary<int, FsmBase>();
        }

        public Fsm<T> Create<T>(T owner, FsmState<T>[] states) where T : class
        {
            return Create<T>(m_TempFsmId++, owner, states);
        }


        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns></returns>
        public Fsm<T> Create<T>(int fsmId,T owner, FsmState<T>[] states) where T : class
        {
            Fsm<T> fsm = new Fsm<T>(fsmId, owner, states);
            m_FsmDic[fsmId] = fsm;
            Debug.Log("创建状态机完成,拥有者为 : " + owner);
            return fsm;
        }

        public void DestroyFsm(int fsmId)
        {
            FsmBase fsm = null;
            if (m_FsmDic.TryGetValue(fsmId, out fsm))
            {
                fsm.ShutDown();
                m_FsmDic.Remove(fsmId);
            }
        }

        public void Dispose()
        {
            var enumerator = m_FsmDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.ShutDown();
            }
            m_FsmDic.Clear();
        }

        public override void Init()
        {
            
        }
    }
}

