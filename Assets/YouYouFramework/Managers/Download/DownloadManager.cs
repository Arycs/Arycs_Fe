using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 下载管理器
    /// </summary>
    public class DownloadManager : ManagerBase, IDisposable
    {
        /// <summary>
        /// 写入磁盘的缓存大小(k)
        /// </summary>
        public int FlushSize
        {
            get; private set;
        }

        /// <summary>
        /// 多文件下载器中的下载器的数量
        /// </summary>
        public int DownloadRoutineCount
        {
            get; private set;
        }

        /// <summary>
        /// 下载失败的连接重试
        /// </summary>
        public int Retry
        {
            get; private set;
        }

        /// <summary>
        /// 下载器链表
        /// </summary>
        private LinkedList<DownloadRoutine> m_DownloadSingleRoutineList;

        /// <summary>
        /// 多文件下载器链表
        /// </summary>
        private LinkedList<DownloadMulitRoutine> m_DownloadMulitRoutineList;

        public DownloadManager()
        {
            m_DownloadSingleRoutineList = new LinkedList<DownloadRoutine>();
            m_DownloadMulitRoutineList = new LinkedList<DownloadMulitRoutine>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            Retry = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.Download_Retry, GameEntry.CurrDeviceGrade);
            DownloadRoutineCount = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.Download_RoutineCount, GameEntry.CurrDeviceGrade);
            FlushSize = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.Download_FlushSize, GameEntry.CurrDeviceGrade);
        }

        /// <summary>
        /// 下载单一文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onUpdate"></param>
        /// <param name="onComplete"></param>
        public void BeginDownloadSingle(string url, BaseAction<string, ulong, float> onUpdate = null, BaseAction<string> onComplete = null)
        {
            AssetBundleInfoEntity entity = GameEntry.Resource.ResourceManager.GetAssetBundleInfo(url);
            if (entity == null)
            {
                GameEntry.LogError("无效的资源包=>" + url);
                return;
            }

            DownloadRoutine routine = GameEntry.Pool.DequeueClassObject<DownloadRoutine>();
            routine.BeginDownload(url, entity, onUpdate, onComplete: (string fileUrl, DownloadRoutine r) =>
             {
                 m_DownloadSingleRoutineList.Remove(r);
                 GameEntry.Pool.EnqueueClassObject(routine);
                 if (onComplete != null)
                 {
                     onComplete(fileUrl);
                 }
             });
            m_DownloadSingleRoutineList.AddLast(routine);
        }

        /// <summary>
        /// 下载多个文件
        /// </summary>
        /// <param name="lstUrl"></param>
        /// <param name="onDownloadMulitUpdate"></param>
        /// <param name="onDownloadMulitComplete"></param>
        public void BeginDownloadMulit(LinkedList<string> lstUrl,
            BaseAction<int, int, ulong, ulong> onDownloadMulitUpdate = null, BaseAction onDownloadMulitComplete = null)
        {
            DownloadMulitRoutine mulitRoutine = GameEntry.Pool.DequeueClassObject<DownloadMulitRoutine>();
            mulitRoutine.BeginDownloadMulit(lstUrl, onDownloadMulitUpdate, onDownloadMulitComplete: (DownloadMulitRoutine r) =>
            {
                m_DownloadMulitRoutineList.Remove(r);
                GameEntry.Pool.EnqueueClassObject(r);
                onDownloadMulitComplete?.Invoke();
            });
            m_DownloadMulitRoutineList.AddLast(mulitRoutine);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void OnUpdate()
        {
            LinkedListNode<DownloadMulitRoutine> mulitRoutine = m_DownloadMulitRoutineList.First;
            while (mulitRoutine != null)
            {
                mulitRoutine.Value.OnUpdate();
                mulitRoutine = mulitRoutine.Next;
            }

            //循环单文件下载器
            LinkedListNode<DownloadRoutine> singleRoutine = m_DownloadSingleRoutineList.First;
            while (singleRoutine != null)
            {
                singleRoutine.Value.OnUpdate();
                singleRoutine = singleRoutine.Next;
            }
        }

        public void Dispose()
        {
            m_DownloadSingleRoutineList.Clear();

            //循环多文件下载器
            LinkedListNode<DownloadMulitRoutine> mulitRoutine = m_DownloadMulitRoutineList.First;
            while (mulitRoutine != null)
            {
                mulitRoutine.Value.Dispose();
                mulitRoutine = mulitRoutine.Next;
            }
            m_DownloadMulitRoutineList.Clear();
        }
    }
}