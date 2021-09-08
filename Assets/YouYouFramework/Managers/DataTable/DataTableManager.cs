using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YouYou
{
    public class DataTableManager : ManagerBase, IDisposable
    {
        public DataTableManager()
        {
            InitDBModel();
        }
        
        /// <summary>
        /// 总共需要加载的表格数量
        /// </summary>
        public int TotalTableCount = 0;

        /// <summary>
        /// 当前需要加载的表格数量
        /// </summary>
        public int CurrLoadTableCount = 0;
        
        /// <summary>
        /// 
        /// </summary>
        public Sys_CodeDBModel Sys_CodeDBModel { get; private set; }

        /// <summary>
        /// 系统特效表
        /// </summary>
        public Sys_EffectDBModel Sys_EffectDBModel { get; private set; }

        /// <summary>
        /// 系统预置体表
        /// </summary>
        public Sys_PrefabDBModel Sys_PrefabDBModel { get; private set; }

        /// <summary>
        /// 系统声音表
        /// </summary>
        public Sys_SoundDBModel Sys_SoundDBModel { get; private set; }

        /// <summary>
        /// 系统剧情声音表
        /// </summary>
        public Sys_StorySoundDBModel Sys_StorySoundDBModel { get; private set; }

        /// <summary>
        /// 系统UI预制表
        /// </summary>
        public Sys_UIFormDBModel Sys_UIFormDBModel { get; private set; }

        /// <summary>
        /// 系統本地化表
        /// </summary>
        public LocalizationDBModel LocalizationDBModel { get; private set; }

        /// <summary>
        /// 系统场景表
        /// </summary>
        public Sys_SceneDBModel Sys_SceneDBModel { get; private set; }

        /// <summary>
        /// 系统场景详情表
        /// </summary>
        public Sys_SceneDetailDBModel Sys_SceneDetailDBModel { get; private set; }

        /// <summary>
        /// 音效表
        /// </summary>
        public Sys_AudioDBModel Sys_AudioDBModel { get; private set; }

        /// <summary>
        /// 初始化DBModel
        /// </summary>
        private void InitDBModel()
        {
            //每个表都需要new一下
            Sys_CodeDBModel = new Sys_CodeDBModel();
            Sys_EffectDBModel = new Sys_EffectDBModel();
            LocalizationDBModel = new LocalizationDBModel();
            Sys_PrefabDBModel = new Sys_PrefabDBModel();
            Sys_SoundDBModel = new Sys_SoundDBModel();
            Sys_StorySoundDBModel = new Sys_StorySoundDBModel();
            Sys_UIFormDBModel = new Sys_UIFormDBModel();
            Sys_SceneDBModel = new Sys_SceneDBModel();
            Sys_SceneDetailDBModel = new Sys_SceneDetailDBModel();
            Sys_AudioDBModel = new Sys_AudioDBModel();
        }

       

        /// <summary>
        /// 加载表格
        /// </summary>
        public void LoadDataTable()
        {
            //每个表都需要LoadData
            Sys_CodeDBModel.LoadData();
            Sys_EffectDBModel.LoadData();
            LocalizationDBModel.LoadData();
            Sys_PrefabDBModel.LoadData();
            Sys_SoundDBModel.LoadData();
            Sys_StorySoundDBModel.LoadData();
            Sys_UIFormDBModel.LoadData();
            Sys_SceneDBModel.LoadData();
            Sys_SceneDetailDBModel.LoadData();
            Sys_AudioDBModel.LoadData();
        }

        /// <summary>
        /// 表格资源包
        /// </summary>
        private AssetBundle m_DataTableBundle;
        
        /// <summary>
        /// 异步加载表格
        /// </summary>
        public void LoadDataTableAsync()
        {
#if DISABLE_ASSETBUNDLE
            LoadDataTable();
#else
            GameEntry.Resource.ResourceLoaderManager.LoadAssetBundle("download/datatable.assetbundle",onComplete:(AssetBundle bundle) =>
                {
                    m_DataTableBundle = bundle;
                    Debug.LogError("LoadDataTableAsync 拿到了 bundle");
                    LoadDataTable();
                });
#endif
        }

        /// <summary>
        /// 获取表格的字节数组
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="onComplete"></param>
        public void GetDataTableBuffer(string tableName, BaseAction<byte[]> onComplete)
        {
#if DISABLE_ASSETBUNDLE
            GameEntry.Time.Yield(() =>
            {
                byte[] buffer = IOUtil.GetFileBuffer(string.Format("{0}/download/DataTable/{1}.bytes",
                    GameEntry.Resource.LocalFilePath, tableName));
                if (onComplete != null)
                {
                    onComplete(buffer);
                }
            });
#else
            GameEntry.Resource.ResourceLoaderManager.LoadAsset(GameEntry.Resource.GetLastPathName(tableName),
                m_DataTableBundle, onComplete:
                (UnityEngine.Object obj) =>
                {
                    TextAsset asset = obj as TextAsset;
                    if (onComplete != null)
                    {
                        onComplete(asset.bytes);
                    }
                });
#endif
        }


        public void Clear()
        {
            //每个表都Clear
            Sys_CodeDBModel.Clear();
            Sys_EffectDBModel.Clear();
            LocalizationDBModel.Clear();
            Sys_PrefabDBModel.Clear();
            Sys_SoundDBModel.Clear();
            Sys_StorySoundDBModel.Clear();
            Sys_UIFormDBModel.Clear();
        }

        public void Dispose()
        {

        }

        public override void Init()
        {

        }
    }
}