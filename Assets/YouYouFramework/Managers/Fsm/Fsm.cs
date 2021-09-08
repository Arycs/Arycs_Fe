using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 状态机
    /// </summary>
    /// <typeparam name="T">拥有者</typeparam>
    public class Fsm<T>:FsmBase where T:class
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        private FsmState<T> m_CurrState;

        /// <summary>
        /// 状态字典
        /// </summary>
        private Dictionary<sbyte, FsmState<T>> m_StateDic;

        /// <summary>
        /// 参数字典
        /// </summary>
        private Dictionary<string, VariableBase> m_ParamDic;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        public Fsm(int fsmId, T owner,FsmState<T>[] states) : base(fsmId)
        {
            m_StateDic = new Dictionary<sbyte, FsmState<T>>();
            m_ParamDic = new Dictionary<string, VariableBase>();
            //把状态加入字典
            int len = states.Length;
            for (int i = 0; i < len; i++)
            {
                FsmState<T> state = states[i];
                state.curFsm = this;
                m_StateDic[(sbyte)i] = state;
            }
            
            CurrStateType = -1;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        public FsmState<T> GetState(sbyte stateType)
        {
            FsmState<T> state = null;
            m_StateDic.TryGetValue(stateType, out state);
            return state;
        }

        /// <summary>
        /// 执行当前状态,由状态机的拥有者来执行Update,不用FsmComponent
        /// </summary>
        public void OnUpdate()
        {
            if (m_CurrState != null)
            {
                m_CurrState.OnUpdate();
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(sbyte newState)
        {
            //两个状态一样不重复进入
            if (CurrStateType == newState)
            {
                return;
            }
            
            if (m_CurrState != null)
            {
                m_CurrState.OnLeave();
            }

            CurrStateType = newState;
            m_CurrState = m_StateDic[CurrStateType];
            
            //进入新状态
            m_CurrState.OnEnter();
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TData">泛型类型</typeparam>
        public void SetData<TData>(string key, TData value)
        {
            VariableBase itemBase = null;
            if (m_ParamDic.TryGetValue(key,out itemBase))
            {
                Debug.Log("修改已有值");
                Variable<TData> item = itemBase as Variable<TData>;
                item.Value = value;
                m_ParamDic[key] = item;
            }
            else
            {
                Debug.Log("参数原本不存在,实例化新对象");
                //参数原本不存在
                Variable<TData> item = new Variable<TData>();
                item.Value = value;
                m_ParamDic[key] = item;
            }
        }

        /// <summary>
        /// 取得参数
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData GetData<TData>(string key)
        {
            VariableBase itemBase = null;
           if (m_ParamDic.TryGetValue(key,out itemBase))
           {
               Variable<TData> item = itemBase as Variable<TData>;
               return item.Value;
           }

           return default(TData);
        }

        /// <summary>
        /// 关闭状态机
        /// </summary>
        public override void ShutDown()
        {
            if (m_CurrState != null)
            {
                m_CurrState.OnLeave();
            }

            foreach (KeyValuePair<sbyte,FsmState<T>> state in m_StateDic)
            {
                state.Value.OnDestroy();    
            }

            m_StateDic.Clear();
            m_ParamDic.Clear();
        }
    }
}