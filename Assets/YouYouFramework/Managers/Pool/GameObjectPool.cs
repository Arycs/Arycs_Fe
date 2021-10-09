using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YouYou
{
    /// <summary>
    /// 游戏物体对象池
    /// </summary>
    public class GameObjectPool : IDisposable
    {
        /// <summary>
        /// 游戏物体对象池字典
        /// </summary>
        private Dictionary<byte, GameObjectPoolEntity> m_SpawnPoolDic;

        /// <summary>
        /// 实例ID对应对象池ID
        /// </summary>
        private Dictionary<int, byte> m_InstanceIdPoolDic;

        /// <summary>
        /// 空闲预设池队列, 相当于对这个预设池再加了一层池
        /// </summary>
        private Queue<PrefabPool> m_PrefabPoolQueue;

        public GameObjectPool()
        {
            m_SpawnPoolDic = new Dictionary<byte, GameObjectPoolEntity>();
            m_InstanceIdPoolDic = new Dictionary<int, byte>();
            m_PrefabPoolQueue = new Queue<PrefabPool>();

            InstanceHandler.InstantiateDelegates += InstantiateDelegate;
            InstanceHandler.DestroyDelegates += DestroyDelegate;

        }

        public void Dispose()
        {
            m_SpawnPoolDic.Clear();
        }

        /// <summary>
        /// 当对象池物体创建时候
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public GameObject InstantiateDelegate(GameObject prefab, Vector3 pos, Quaternion rot, object userData)
        {
            ResourceEntity resourceEntity = userData as ResourceEntity;

            if (resourceEntity != null)
            {
                Debug.LogError("resourceEntity= " + resourceEntity.ResourceName);
            }

            GameObject obj = Object.Instantiate(prefab, pos, rot);
            Debug.LogError("实例编号000=" + obj.GetInstanceID());

            //注册
            GameEntry.Pool.RegisterInstanceResource(obj.GetInstanceID(), resourceEntity);
            return obj;
        }

        /// <summary>
        /// 当对象池物体销毁的时候
        /// </summary>
        /// <param name="instance"></param>
        public void DestroyDelegate(GameObject instance)
        {
            Object.Destroy(instance);
            GameEntry.Resource.ResourceLoaderManager.UnLoadGameObject(instance);
        }

        /// <summary>
        /// 切换场景的时候 销毁需要时间,因此使用协程
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerator Init(GameObjectPoolEntity[] arr, Transform parent)
        {
            int len = arr.Length;
            for (int i = 0; i < len; i++)
            {
                GameObjectPoolEntity entity = arr[i];
                if (entity.Pool != null)
                {
                    Object.Destroy(entity.Pool.gameObject);
                    yield return null;
                    entity.Pool = null;
                }

                //创建对象池
                SpawnPool pool = PathologicalGames.PoolManager.Pools.Create(entity.PoolName);
                pool.group.parent = parent;
                pool.group.localPosition = Vector3.zero;
                entity.Pool = pool;

                m_SpawnPoolDic[entity.PoolId] = entity;
            }
        }

        /// <summary>
        /// 加载中的预设池字典
        /// </summary>
        private Dictionary<int, HashSet<BaseAction<SpawnPool, Transform, ResourceEntity>>> m_LoadingPrefabPoolDic = new Dictionary<int, HashSet<BaseAction<SpawnPool, Transform, ResourceEntity>>>();

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="prefab"></param>
        /// <param name="onComplete"></param>
        public void Spawn(int prefabId, BaseAction<Transform> onComplete)
        {
            lock (m_PrefabPoolQueue)
            {
                //拿到预设表数据
                Sys_PrefabEntity entity = GameEntry.DataTable.Sys_PrefabDBModel.Get(prefabId);
                if (entity == null)
                {
                    Debug.LogError("预设数据不存在");
                    return;
                }

                //拿到对象池
                GameObjectPoolEntity gameObjectPoolEntity = m_SpawnPoolDic[entity.PoolId];

                //使用预设编号 当做池ID
                PrefabPool prefabPool = gameObjectPoolEntity.Pool.GetPrefabPool(entity.Id);

                if (prefabPool != null)
                {
                    //拿到一个实例
                    Transform retTrans = prefabPool.TrySpawnInstance();
                    if (retTrans != null)
                    {
                        int instanceID = retTrans.gameObject.GetInstanceID();
                        m_InstanceIdPoolDic[instanceID] = entity.PoolId;
                        onComplete?.Invoke(retTrans);
                        return;
                    }
                }

                HashSet<BaseAction<SpawnPool, Transform, ResourceEntity>> lst = null;
                if (m_LoadingPrefabPoolDic.TryGetValue(prefabId,out lst))
                {
                    //进行拦截
                    //如果存在加载中的asset 把委托加入对应的链表 然后直接返回
                    lst.Add((_SpawnPool, _Transform, _ResourceEntity) =>
                    {
                        //拿到一个实例
                        Transform retTrans = _SpawnPool.Spawn(_Transform, _ResourceEntity);
                        int instanceID = retTrans.gameObject.GetInstanceID();
                        m_InstanceIdPoolDic[instanceID] = entity.PoolId;
                        onComplete?.Invoke(retTrans);
                    });
                    return;
                }
                //这里说明是加载在第一个
                lst = GameEntry.Pool.DequeueClassObject<HashSet<BaseAction<SpawnPool, Transform, ResourceEntity>>>();
                lst.Add((_SpawnPool, _Transform, _ResourceEntity) =>
                {
                    //拿到一个实例
                    Transform retTrans = _SpawnPool.Spawn(_Transform, _ResourceEntity);
                    int instanceID = retTrans.gameObject.GetInstanceID();
                    m_InstanceIdPoolDic[instanceID] = entity.PoolId;
                    onComplete?.Invoke(retTrans);
                });
                m_LoadingPrefabPoolDic[prefabId] = lst;

                GameEntry.Resource.ResourceLoaderManager.LoadMainAsset((AssetCategory)entity.AssetCategory, entity.AssetPath,
                    resourceEntity =>
                    {
                        Transform prefab = ((GameObject)resourceEntity.Target).transform;

                        if (prefabPool == null)
                        {
                            //先去队列里找 空闲的池
                            if (m_PrefabPoolQueue.Count > 0)
                            {
                                Debug.LogError("从队列里取");
                                prefabPool = m_PrefabPoolQueue.Dequeue();

                                prefabPool.PrefabPoolId = entity.Id;//设置预设池编号
                                gameObjectPoolEntity.Pool.AddPrefabPool(prefabPool);

                                prefabPool.prefab = prefab;
                                prefabPool.prefabGO = prefab.gameObject;
                                prefabPool.AddPrefabToDic(prefab.name, prefab);
                            }
                            else
                            {
                                prefabPool = new PrefabPool(prefab, entity.Id);
                                gameObjectPoolEntity.Pool.CreatePrefabPool(prefabPool, resourceEntity);
                            }

                            prefabPool.OnPrefabPoolClear = pool =>
                            {
                                //预设池加入队列
                                pool.PrefabPoolId = 0;
                                gameObjectPoolEntity.Pool.RemovePrefabPool(pool);
                                m_PrefabPoolQueue.Enqueue(pool);
                            };

                            //这些属性要从表格中读取
                            prefabPool.cullDespawned = entity.CullDespawned == 1;
                            prefabPool.cullAbove = entity.CullAbove;
                            prefabPool.cullDelay = entity.CullDelay;
                            prefabPool.cullMaxPerPass = entity.CullMaxPerPass;
                        }

                        var enumerator = lst.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current?.Invoke(gameObjectPoolEntity.Pool, prefab, resourceEntity);
                        }
                        m_LoadingPrefabPoolDic.Remove(prefabId);
                        lst.Clear(); // 一定要清空
                        GameEntry.Pool.EnqueueClassObject(lst);
                    });
            }
        }

        #region Despawn 对象回池
        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="instance"></param>
        public void Despawn(byte poolId, Transform instance)
        {
            GameObjectPoolEntity entity = m_SpawnPoolDic[poolId];
            entity.Pool.Despawn(instance);
        }

        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="instance"></param>
        public void Despawn(Transform instance)
        {
            int instanceID = instance.gameObject.GetInstanceID();
            byte poolId = m_InstanceIdPoolDic[instanceID];
            m_InstanceIdPoolDic.Remove(instanceID);
            Despawn(poolId, instance);
        }

        #endregion

    }
}