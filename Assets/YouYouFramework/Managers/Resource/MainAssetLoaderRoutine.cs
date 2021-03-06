using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 主资源加载器
    /// </summary>
    public class MainAssetLoaderRoutine
    {
        /// <summary>
        /// 当前的资源信息实体
        /// </summary>
        private AssetEntity m_CurrAssetEntity;

        /// <summary>
        /// 当前的资源实体
        /// </summary>
        private ResourceEntity m_CurrResourceEntity;

        /// <summary>
        /// 当前资源的依赖资源实体链表(临时存储)
        /// </summary>
        private LinkedList<ResourceEntity> m_DependsResourceList = new LinkedList<ResourceEntity>();

        /// <summary>
        /// 需要加载的依赖资源数量
        /// </summary>
        private int m_NeedLoadAssetDependCount = 0;

        /// <summary>
        /// 当前已经加载的依赖资源数量
        /// </summary>
        private int m_CurrLoadAssetDependCount = 0;

        /// <summary>
        /// 著资源加载完毕
        /// </summary>
        /// <returns></returns>
        private BaseAction<ResourceEntity> m_OnComplete;

        /// <summary>
        /// 加载主资源
        /// </summary>
        /// <param name="assetCategory"></param>
        /// <param name="assetFullName"></param>
        /// <param name="onComplete"></param>
        public void Load(AssetCategory assetCategory, string assetFullName,
            BaseAction<ResourceEntity> onComplete = null)
        {
#if DISABLE_ASSETBUNDLE && UNITY_EDITOR
            Debug.LogError("取池");
            m_CurrResourceEntity = GameEntry.Pool.DequeueClassObject<ResourceEntity>();
            m_CurrResourceEntity.Category = assetCategory;
            m_CurrResourceEntity.IsAssetBundle = false;
            m_CurrResourceEntity.ResourceName = assetFullName;
            if (assetCategory == AssetCategory.Scenario)
            {
                m_CurrResourceEntity.Target = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(assetFullName);
            }
            else if(assetCategory == AssetCategory.MapEventInfo)
            {
                m_CurrResourceEntity.Target = UnityEditor.AssetDatabase.LoadAssetAtPath<MapEventInfo>(assetFullName);
            }
            else if(assetCategory == AssetCategory.MapObject)
            {
                m_CurrResourceEntity.Target = UnityEditor.AssetDatabase.LoadAssetAtPath<MapObject>(assetFullName);
            }
            else
            {
                m_CurrResourceEntity.Target = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetFullName);
            }

            if (m_CurrResourceEntity.Target == null)
            {
                Debug.LogError($"资源加载出现错误,请检查是否路径正确或者加载格式正确,{assetFullName}");
            }
            
            if (onComplete != null)
            {
                onComplete(m_CurrResourceEntity);
            }
#else
            m_OnComplete = onComplete;
            m_CurrAssetEntity = GameEntry.Resource.ResourceLoaderManager.GetAssetEntity(assetCategory, assetFullName);
            LoadDependsAsset();
#endif

        }

        /// <summary>
        /// 真正的加载主资源
        /// </summary>
        private void LoadMainAsset()
        {
            //1. 从分类资源池(AssetPool)中查找
            ResourceEntity assetEntity = GameEntry.Pool.AssetPool[m_CurrAssetEntity.Category]
                .Spawn(m_CurrAssetEntity.AssetFullName);
            if (assetEntity != null)
            {
                Debug.LogError("从分类资源池加载" + assetEntity.ResourceName);
                //说明资源在分类资源池中存在
                if (m_OnComplete != null)
                {
                    m_OnComplete(assetEntity);
                }

                return;
            }

            //2. 找资源包
            GameEntry.Resource.ResourceLoaderManager.LoadAssetBundle(m_CurrAssetEntity.AssetBundleName,
                onComplete: (AssetBundle bundle) =>
                {
                    //3. 加载资源
                    GameEntry.Resource.ResourceLoaderManager.LoadAsset(m_CurrAssetEntity.AssetFullName, bundle,
                        onComplete: (
                            UnityEngine.Object obj) =>
                        {
                            //4. 再次检查 很重要 不检查引用计数会出错
                            m_CurrResourceEntity = GameEntry.Pool.AssetPool[m_CurrAssetEntity.Category]
                                .Spawn(m_CurrAssetEntity.AssetFullName);
                            if (m_CurrResourceEntity != null)
                            {
                                if (m_OnComplete != null)
                                {
                                    m_OnComplete(m_CurrResourceEntity);
                                }
                                return;
                            }

                            m_CurrResourceEntity = GameEntry.Pool.DequeueClassObject<ResourceEntity>();
                            m_CurrResourceEntity.Category = m_CurrAssetEntity.Category;
                            m_CurrResourceEntity.IsAssetBundle = false;
                            m_CurrResourceEntity.ResourceName = m_CurrAssetEntity.AssetFullName;
                            m_CurrResourceEntity.Target = obj;

                            GameEntry.LogError("注册" + m_CurrResourceEntity.ResourceName);
                            GameEntry.Pool.AssetPool[m_CurrAssetEntity.Category].Register(m_CurrResourceEntity);

                            //加入到这个资源的依赖资源链表里
                            var CurrDependsResource = m_DependsResourceList.First;
                            while (CurrDependsResource != null)
                            {
                                var next = CurrDependsResource.Next;
                                m_DependsResourceList.Remove(CurrDependsResource);
                                m_CurrResourceEntity.DependsResourceList.AddLast(CurrDependsResource);
                                CurrDependsResource = next;
                            }

                            if (m_OnComplete != null)
                            {
                                m_OnComplete(m_CurrResourceEntity);
                            }

                            Reset();
                        });
                });
        }


        /// <summary>
        /// 加载依赖资源
        /// </summary>
        private void LoadDependsAsset()
        {
            List<AssetDependsEntity> lst = m_CurrAssetEntity.DependsAssetList;
            if (lst != null)
            {
                int len = lst.Count;
                m_NeedLoadAssetDependCount = len;
                for (int i = 0; i < len; i++)
                {
                    AssetDependsEntity entity = lst[i];
                    MainAssetLoaderRoutine routine = GameEntry.Pool.DequeueClassObject<MainAssetLoaderRoutine>();
                    routine.Load(entity.Category, entity.AssetFullName, OnLoadDependsAssetComplete);
                }
            }
            else
            {
                //这个资源没有依赖 直接加载主资源
                LoadMainAsset();
            }
        }

        /// <summary>
        /// 加载某个依赖资源完毕
        /// </summary>
        /// <param name="res"></param>
        private void OnLoadDependsAssetComplete(ResourceEntity res)
        {
            //把这个主资源依赖的资源实体,加入临时链表
            m_DependsResourceList.AddLast(res);

            //把加载出来的资源 加入到池 需要做
            m_CurrLoadAssetDependCount++;

            //这个主资源的依赖加载完毕了
            if (m_NeedLoadAssetDependCount == m_CurrLoadAssetDependCount)
            {
                //加载这个资源的主资源
                LoadMainAsset();
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            m_OnComplete = null;
            m_CurrAssetEntity = null;
            m_CurrResourceEntity = null;
            m_NeedLoadAssetDependCount = 0;
            m_CurrLoadAssetDependCount = 0;
            m_DependsResourceList.Clear();
            GameEntry.Pool.EnqueueClassObject(this);
        }
    }
}