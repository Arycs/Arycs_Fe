using System;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    public class PoolManager : ManagerBase, IDisposable
    {

        /// <summary>
        /// 类对象池
        /// </summary>
        public ClassObjectPool ClassObjectPool
        {
            get; private set;
        }
        /// <summary>
        /// 游戏物体对象池
        /// </summary>
        public GameObjectPool GameObjectPool
        {
            get; private set;
        }
        /// <summary>
        /// 资源包池
        /// </summary>
        public ResourcePool AssetBundlePool
        {
            get; private set;
        }
        /// <summary>
        /// 分类资源池
        /// </summary>
        public Dictionary<AssetCategory, ResourcePool> AssetPool
        {
            get; private set;
        }

        /// <summary>
        /// 锁定的资源包数组长度
        /// </summary>
        public int m_LockedAssetBundleLength;

        public PoolManager()
        {
            ClassObjectPool = new ClassObjectPool();
            GameObjectPool = new GameObjectPool();

            AssetBundlePool = new ResourcePool("AssetBundlePool");
            m_InstanceResourceDic = new Dictionary<int, ResourceEntity>();
            FontDic = new Dictionary<string, ResourceEntity>();
            AssetPool = new Dictionary<AssetCategory, ResourcePool>();
        }

        public override void Init()
        {
            ReleaseClassObjectInterval = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.Pool_ReleaseClassObjectInterval, GameEntry.CurrDeviceGrade);
            ReleaseAssetBundleInterval = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.Pool_ReleaseAssetBundleInterval, GameEntry.CurrDeviceGrade);
            ReleaseAssetInterval = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.Pool_ReleaseAssetInterval, GameEntry.CurrDeviceGrade);

            //确保游戏刚开始运行的时候,分类资源池已经初始化好了
            var enumerator = Enum.GetValues(typeof(AssetCategory)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssetCategory assetCategory = (AssetCategory)enumerator.Current;
                if (assetCategory == AssetCategory.None)
                {
                    continue;
                }
                AssetPool[assetCategory] = new ResourcePool(assetCategory.ToString());
            }

            ReleaseClassObjectNextRunTime = Time.time;
            ReleaseAssetBundleNextRunTime = Time.time;
            ReleaseAssetNextRunTime = Time.time;

            InitGameObjectPool();
            m_LockedAssetBundleLength = GameEntry.Instance.LockedAssetBundle.Length;
            InitClassReside();
        }

        /// <summary>
        /// 检查资源包是否锁定
        /// </summary>
        /// <param name="assetBundleName">资源包名称</param>
        /// <returns></returns>
        public bool CheckAssetBundleIsLock(string assetBundleName)
        {
            for (int i = 0; i < m_LockedAssetBundleLength; i++)
            {
                if (GameEntry.Instance.LockedAssetBundle[i].Equals(assetBundleName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 初始化常用类常驻数量
        /// </summary>
        private void InitClassReside()
        {
            SetClassObjectResideCount<HttpRoutine>(3);
            SetClassObjectResideCount<Dictionary<string, object>>(3);
            SetClassObjectResideCount<AssetBundleLoaderRoutine>(10);
            SetClassObjectResideCount<AssetLoaderRoutine>(10);
            SetClassObjectResideCount<MainAssetLoaderRoutine>(10);
        }

        #region  类对象池相关操作

        #region 设置类常驻数量

        /// <summary>
        /// 设置类的常驻数量
        /// </summary>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        public void SetClassObjectResideCount<T>(byte count) where T : class
        {
            ClassObjectPool.SetResideCount<T>(count);
        }

        #endregion


        #region DequeueClassObject取出一个对象

        /// <summary>
        /// 取出一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T DequeueClassObject<T>() where T : class, new()
        {
            return ClassObjectPool.DequeueClassObject<T>();
        }

        #endregion


        #region EnqueueClassObject对象回池

        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="obj"></param>
        public void EnqueueClassObject(object obj)
        {
            ClassObjectPool.EnqueueClassObject(obj);
        }

        #endregion

        #endregion


        #region 变量对象池

        /// <summary>
        /// 变量对象池锁
        /// </summary>
        private object m_VarObjectLock = new object();

#if UNITY_EDITOR
        /// <summary>
        /// 在监视面板显示的信息
        /// </summary>
        public Dictionary<Type, int> VarObjectInspectorDic = new Dictionary<Type, int>();

#endif

        /// <summary>
        /// 取出一个变量对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T DequeueVarObject<T>() where T : VariableBase, new()
        {
            lock (m_VarObjectLock)
            {
                T item = DequeueClassObject<T>();
#if UNITY_EDITOR
                Type t = item.GetType();
                if (VarObjectInspectorDic.ContainsKey(t))
                {
                    VarObjectInspectorDic[t]++;
                }
                else
                {
                    VarObjectInspectorDic[t] = 1;
                }
#endif
                return item;
            }
        }

        /// <summary>
        /// 变量对象回池
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        public void EnqueueVarObject<T>(T item) where T : VariableBase
        {
            lock (m_VarObjectLock)
            {
                EnqueueClassObject(item);
#if UNITY_EDITOR
                Type t = item.GetType();
                if (VarObjectInspectorDic.ContainsKey(t))
                {
                    VarObjectInspectorDic[t]--;
                    if (VarObjectInspectorDic[t] == 0)
                    {
                        VarObjectInspectorDic.Remove(t);
                    }
                }
#endif
            }
        }

        #endregion


        #region 释放对象时间相关

        /// <summary>
        /// 释放类对象池间隔
        /// </summary>
        public int ReleaseClassObjectInterval = 30;

        /// <summary>
        /// 下次释放类对象运行时间
        /// </summary>
        public float ReleaseClassObjectNextRunTime = 0f;

        /// <summary>
        /// 下次释放AB包运行时间
        /// </summary>
        public float ReleaseAssetBundleNextRunTime = 0f;

        /// <summary>
        /// 释放AssetBundle池间隔
        /// </summary>
        public int ReleaseAssetBundleInterval = 60;

        /// <summary>
        /// 下次释放AssetBundle池运行时间
        /// </summary> 
        private float ReleaseResourceNextRunTime = 0f;

        /// <summary>
        /// 释放Asset池间隔
        /// </summary>
        public int ReleaseAssetInterval = 120;

        /// <summary>
        /// 下次释放Asset池运行时间  
        /// </summary>
        public float ReleaseAssetNextRunTime = 0f;

        /// <summary>
        /// 显示分类资源池
        /// </summary>
        public bool ShowAssetPool = false;

        public void OnUpdate()
        {
            if (Time.time > ReleaseClassObjectNextRunTime + ReleaseClassObjectInterval)
            {
                ReleaseClassObjectNextRunTime = Time.time;
                ReleaseClassObjectPool();
                GameEntry.Log(LogCategory.Normal, "释放类对象池");
            }

            if (Time.time > ReleaseResourceNextRunTime + ReleaseAssetBundleInterval)
            {
                ReleaseResourceNextRunTime = Time.time;
#if !DISABLE_ASSETBUNDLE
                ReleaseAssetBundlePool();
                GameEntry.Log(LogCategory.Normal, "释放资源包池");
#endif
            }

            if (Time.time > ReleaseAssetNextRunTime + ReleaseAssetInterval)
            {
                ReleaseAssetNextRunTime = Time.time;
#if !DISABLE_ASSETBUNDLE
                ReleaseAssetPool();
                GameEntry.Log(LogCategory.Normal, "释放Asset池");
#endif
                //LuaManager.luaEnv.FullGc();
                Resources.UnloadUnusedAssets();
            }
        }

        #endregion


        #region 游戏物体对象池

        /// <summary>
        /// 对象池的分组
        /// </summary>
        [SerializeField] private GameObjectPoolEntity[] m_GameObjectPoolGroups;

        /// <summary>
        /// 初始化游戏物体对象池
        /// </summary>
        public void InitGameObjectPool()
        {
            GameEntry.Instance.StartCoroutine(GameObjectPool.Init(GameEntry.Instance.GameObjectPoolGroups,GameEntry.Instance.PoolParent));
        }

        #endregion


        #region GameObjectSpawn从游戏物体对象池中获取一个对象

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="onComplete"></param>
        public void GameObjectSpawn(byte poolId, BaseAction<Transform> onComplete)
        {
            GameObjectPool.Spawn(poolId, onComplete);
        }

        #endregion


        #region GameObjectDesspawn将游戏物体对象放回到对象池中

        /// <summary>
        /// 回池
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="instance"></param>
        public void GameObjectDespawn(byte poolId, Transform instance)
        {
            GameObjectPool.Despawn(poolId, instance);
        }

        #endregion


        #region 实例管理和分类资源池释放

        /// <summary>
        /// 克隆出来的实例资源字典
        /// </summary>
        private Dictionary<int, ResourceEntity> m_InstanceResourceDic;

        /// <summary>
        /// 字体字典
        /// </summary>
        private Dictionary<string, ResourceEntity> FontDic;

        /// <summary>
        /// 注册到实例字典
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="resourceEntity"></param>
        public void RegisterInstanceResource(int instanceId, ResourceEntity resourceEntity)
        {
            Debug.LogError("注册到实例字典instanceID = " + instanceId);
            m_InstanceResourceDic[instanceId] = resourceEntity;
        }

        /// <summary>
        /// 释放实例资源
        /// </summary>
        /// <param name="instanceId"></param>
        public void ReleaseInstanceResource(int instanceId)
        {
            Debug.LogError("释放实例资源instanceId = " + instanceId);
            ResourceEntity resourceEntity = null;
            if (m_InstanceResourceDic.TryGetValue(instanceId, out resourceEntity))
            {
#if DISABLE_ASSETBUNDLE
                resourceEntity.Target = null;
                EnqueueClassObject(resourceEntity);
#else
                UnspawnResourceEntity(resourceEntity);
#endif
                m_InstanceResourceDic.Remove(instanceId);
            }
        }

        /// <summary>
        /// 资源实体回池
        /// </summary>
        /// <param name="entity"></param>
        private void UnspawnResourceEntity(ResourceEntity entity)
        {
            var curr = entity.DependsResourceList.First;
            while (curr != null)
            {
                UnspawnResourceEntity(curr.Value);
                curr = curr.Next;
            }

            GameEntry.Pool.AssetPool[entity.Category].Unspawn(entity.ResourceName);
        }

        #endregion

        /// <summary>
        /// 释放类对象池
        /// </summary>
        public void ReleaseClassObjectPool()
        {
            ClassObjectPool.Release();
        }

        /// <summary>
        /// 释放资源包池
        /// </summary>
        public void ReleaseAssetBundlePool()
        {
            AssetBundlePool.Release();
        }

        /// <summary>
        /// 释放分类资源池中的所有资源
        /// </summary>
        public void ReleaseAssetPool()
        {
            var enumerator = Enum.GetValues(typeof(AssetCategory)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssetCategory assetCategory = (AssetCategory)enumerator.Current;
                if (assetCategory == AssetCategory.None)
                {
                    continue;
                }

                AssetPool[assetCategory].Release();
            }
        }


        public void Dispose()
        {
            ClassObjectPool.Dispose();
            GameObjectPool.Dispose();
        }

        
    }
}