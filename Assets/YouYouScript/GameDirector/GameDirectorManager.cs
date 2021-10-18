using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 游戏剧情管理器
    /// </summary>
    public class GameDirectorManager : ManagerBase, IDisposable
    {
        private IGameAction m_GameAction;

        private bool isLoaded;
        public IGameAction CurrentAction
        {
            get { return m_GameAction; }
            protected set { m_GameAction = value; }
        }

        private Coroutine m_Coroutine = null;
        public override void Init()
        {
            ScenarioAction action = new ScenarioAction(m_GameAction);
            Type[] executorTypes = GameAction.GetDefaultExecutorTypesForScenarioAction().ToArray();
            action.LoadExecutors(executorTypes);
            CurrentAction = action;
        }
        
        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userData"></param>
        /// <param name="onOpen"></param>
        public void RunScenario(int id, object userData = null, BaseAction<TextAsset> onOpen = null)
        {
            //1. 读表 , 如果没有则直接返回
            //TODO 这里的路径为临时写入
            LoadScenarioAsset("test",(resourceEntity =>
            {
                TextAsset textAsset = resourceEntity.Target as TextAsset;
                Debug.Log("加载出来的目标为" + textAsset.text);
                TxtScript txt = new TxtScript();
                txt.Load("序章", textAsset.text);
                if (((ScenarioAction) CurrentAction).LoadScenario(txt))
                {
                    isLoaded = true;
                }
                else
                {
                    isLoaded = false;
                    Debug.LogError("剧本读取失败,请查看剧本,剧本名称为" + "//Todo 剧本名称");
                    return;
                }
                RunGameAction();
            }));
        }

        public void LoadScenarioAsset(string assetPath, BaseAction<ResourceEntity> onComplete)
        {
            GameEntry.Resource.ResourceLoaderManager.LoadMainAsset(AssetCategory.Scenario,
                string.Format("Assets/Download/Scenario/{0}.txt",assetPath),
                (resourceEntity =>
                {
                    if (onComplete != null)
                    {
                        onComplete(resourceEntity);
                    }
                }));
        }

        public void Update()
        {
            if (isLoaded)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentAction.OnMouseLButtonDown(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    CurrentAction.OnMouseLButtonUp(Input.mousePosition);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    CurrentAction.OnMouseRButtonDown(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    CurrentAction.OnMouseRButtonUp(Input.mousePosition);
                }
            }
        }

        public void Dispose()
        {
            m_GameAction?.Dispose();
        }
        /// <summary>
        /// 执行当前脚本
        /// </summary>
        public void RunGameAction()
        {
            m_Coroutine = GameEntry.Instance.StartCoroutine(RunningGameAction());
        }
        
        /// <summary>
        /// 停止当前脚本
        /// </summary>
        public void StopGameAction()
        {
            if (m_Coroutine == null)
            {
                return;
            }
            GameEntry.Instance.StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
        
        /// <summary>
        /// 运行中
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunningGameAction()
        {
            yield return null;

            while (true)
            {
                if (CurrentAction == null)
                {
                    yield return null;
                    continue;
                }

                if (CurrentAction.Update())
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            m_Coroutine = null;
        }
        
        /// <summary>
        /// 读取地图
        /// </summary>
        /// <param name="argsSceneName"></param>
        /// <returns></returns>
        public bool LoadMap(string argsSceneName)
        {
            int sceneId =  GameEntry.DataTable.Sys_SceneDBModel.GetSceneIdByName(argsSceneName);
            if (sceneId == 0)
            {
                Debug.LogError("场景名不存在,请检查,场景名为 : " + argsSceneName);
                return false;
            }
            GameEntry.Scene.LoadScene(sceneId,onComplete:LoadMap_OnSceneLoaded);
            return true;
        }

        private void LoadMap_OnSceneLoaded()
        {
            //暂停之前的游戏剧本
            StopGameAction();
            
            MapAction action = new MapAction(CurrentAction);
            if (!action.Load(ScenarioBlackboard.mapScript))
            {
                Debug.LogError(action.error);
                action.Dispose();
                return;
            }
            
            CurrentAction.Pause();
            CurrentAction = action;
        }

        public void BackGameAction()
        {
            IGameAction old = CurrentAction;
            CurrentAction = CurrentAction.previous;
            old.Dispose();

            if (CurrentAction == null)
            {
                //TODO 返回游戏的开始场景
            }
        }
    }
}