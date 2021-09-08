using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 通用事件 
    /// </summary>
    public class CommonEvent:IDisposable
    {
        public delegate void OnActionHandler(object userData);
        public Dictionary<ushort, LinkedList<OnActionHandler>> dic = new Dictionary<ushort, LinkedList<OnActionHandler>>();

        #region AddEventListener添加监听
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddEventListener(ushort key, OnActionHandler handler)
        {
            LinkedList<OnActionHandler> lstHandler = null;
            dic.TryGetValue(key, out lstHandler);

            if (lstHandler == null)
            {
                lstHandler = new LinkedList<OnActionHandler>();
                dic[key] = lstHandler;
            }
            lstHandler.AddLast(handler);
        }
        #endregion

        #region RemoveEventListener移除监听
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(ushort key, OnActionHandler handler)
        {
            LinkedList<OnActionHandler> lstHandler = null;
            dic.TryGetValue(key, out lstHandler);

            if (lstHandler != null)
            {
                lstHandler.Remove(handler);
                if (lstHandler.Count == 0)
                {
                    dic.Remove(key);
                }
            }
        }
        #endregion

        #region Dispatch派发
        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userData"></param>
        public void Dispatch(ushort key, object userData)
        {
            LinkedList<OnActionHandler> lstHandler = null;
            dic.TryGetValue(key, out lstHandler);

            if (lstHandler != null)
            {
                int lstCount = lstHandler.Count; //获取集合数量 只调用一次, 针对for循环优化,如果用lstHandler.Count效果一样,但是每次访问都需要有额外开销
                for (LinkedListNode<OnActionHandler> curr = lstHandler.First; curr != null; curr = curr.Next)
                {
                    //获取索引数据 只调用一次
                    OnActionHandler handler = curr.Value;
                    if (handler != null)
                    {
                        handler(userData);
                    }
                }
            }
        }

        public void Dispatch(ushort key)
        {
            Dispatch(key,null);
        }
        #endregion

        public void Dispose()
        {
            //Debug.Log("释放CommonEvent字典");
            dic.Clear();
        }
    }
}
