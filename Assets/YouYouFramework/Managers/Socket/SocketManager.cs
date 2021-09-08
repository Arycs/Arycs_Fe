using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class SocketManager : ManagerBase,IDisposable
    {
        [Header("每帧最大发送数量")] public int MaxSendCount = 5;

        [Header("每次发包最大的字节")] public int MaxSendByteCount = 1024;

        [Header("每帧最大处理包数量")] public int MaxReceiveCount = 5;

        [Header("心跳间隔")] public int HeartbeatInterval = 10;

        /// <summary>
        /// 上次的心跳时间
        /// </summary>
        private float m_PrevHeartbeatTime = 0;

        /// <summary>
        /// PING值(毫秒)
        /// </summary>
        [HideInInspector] public int PingValue;

        /// <summary>
        /// 游戏服务器时间
        /// </summary>
        [HideInInspector] public long GameServerTime;

        /// <summary>
        /// 和服务器对表的时刻
        /// </summary>
        [HideInInspector] public float CheckServerTime;

        /// <summary>
        /// 是否已经连接到了主Socket
        /// </summary>
        private bool m_IsConnectToMainSocket = false;

        /// <summary>
        /// 发送用的MS
        /// </summary>
        public MMO_MemoryStream SocketSendMS
        {
            get;
            private set;
        }

        /// <summary>
        /// 接收用的MS
        /// </summary>
        public MMO_MemoryStream SocketReceiveMS
        {
            get;
            private set;
        }

        /// <summary>
        /// SocketTcp访问器的链表
        /// </summary>
        private LinkedList<SocketTcpRoutine> m_SocketTcpRoutineList;
        public SocketManager()
        {
            m_SocketTcpRoutineList = new LinkedList<SocketTcpRoutine>();
            SocketSendMS = new MMO_MemoryStream();
            SocketReceiveMS = new MMO_MemoryStream();
            m_MainSocket = CreateSocketTcpRoutine();
            m_MainSocket.OnConnectOK = () =>
            {
                //已经建立了链接
                m_IsConnectToMainSocket = true;
            };
            SocketProtoListener.AddProtoListener();
        }

        /// <summary>
        /// 创建SocketTcp访问器
        /// </summary>
        /// <returns></returns>
        public SocketTcpRoutine CreateSocketTcpRoutine()
        {
            //从池中获取(什么时候回池),断开没必要回池,因为可能还会需要
            return GameEntry.Pool.DequeueClassObject<SocketTcpRoutine>();
        }

        /// <summary>
        /// 注册SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RegisterSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketTcpRoutineList.AddFirst(routine);
        }
        
        /// <summary>
        /// 移除SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RemoveSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketTcpRoutineList.Remove(routine);
        }

        internal void OnUpdate()
        {
            for (LinkedListNode<SocketTcpRoutine> curr = m_SocketTcpRoutineList.First; curr != null; curr = curr.Next) 
            {
                curr.Value.OnUpdate();
            }
            if (m_IsConnectToMainSocket)
            {
                if (Time.realtimeSinceStartup > m_PrevHeartbeatTime + HeartbeatInterval)
                {
                    //发送心跳包
                    m_PrevHeartbeatTime = Time.realtimeSinceStartup;
                    //以下是链接服务器使用,目前没链接服务器先注释掉
                    //System_HeartbeatProto proto = new System_HeartbeatProto();
                    //proto.LocalTime = Time.realtimeSinceStartup * 1000;
                    //CheckServerTime = Time.realtimeSinceStartup; // 和服务器对表的时刻
                    //SendMsg(proto);

                    //接收心跳包, 这里改放到消息的Handler中
                    //float localTime = proto.LocalTime;
                    //long serverTime = proto.ServerTime;

                    //GameEntry.Socket.PingValue = (int) ((Time.realtimeSinceStartup * 1000 - localTime) * 0.5f); //Ping值
                    //GameEntry.Socket.GameServerTime = serverTime - GameEntry.Socket.PingValue; // 客户端计算出来的服务器时间
                }
            }
        }
        //============业务逻辑=====================
        /// <summary>
        /// 主Socket
        /// </summary>
        private SocketTcpRoutine m_MainSocket;

        /// <summary>
        /// 链接主Socket 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void ConnectToMainSocket(string ip, int port)
        {
            m_MainSocket.Connect(ip, port);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        public void SendMainMsg(IProto proto)
        {
#if DEBUG_LOG_PROTO
            Debug.Log("<color=#ffa200>发送消息:</color><color=#FFFB80>" + proto.ProtoEnName + "" + proto.ProtoId +
                      "</color>");
            Debug.Log("<color=#ffdeb3>==>>" + JsonUtility.ToJson(proto) + "</color>");
#endif
            m_MainSocket.SendMsg(proto);
        }

        /// <summary>
        /// Lua中发送消息
        /// </summary>
        /// <param name="protoId"></param>
        /// <param name="category"></param>
        /// <param name="buffer"></param>
        public void SendMainMsgForLua(ushort protoId, byte category, byte[] buffer)
        {
            m_MainSocket.SendMsg(protoId, category, buffer);
        }

        public void Dispose()
        {
            m_SocketTcpRoutineList.Clear();

            m_IsConnectToMainSocket = false;
            GameEntry.Pool.EnqueueClassObject(m_MainSocket);
            SocketProtoListener.RemoveProtoListener();

            SocketSendMS.Dispose();
            SocketSendMS.Close();
            SocketReceiveMS.Dispose();
            SocketReceiveMS.Close();
        }

        public override void Init()
        {

        }
    }
}