using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using UnityEngine.UI;

namespace YouYou
{
    public class GameEntry : MonoBehaviour
    {
        [FoldoutGroup("ParamsSettings")]
        [SerializeField]
        private ParamsSettings.DeviceGrade m_CurrDeviceGrade;

        [FoldoutGroup("ParamsSettings")]
        [SerializeField]
        private ParamsSettings m_ParamsSettings;

        [FoldoutGroup("GameObjectPool")]
        [Header("游戏物体对象池父物体")]
        public Transform PoolParent;

        /// <summary>
        /// 游戏物体对象池的分组
        /// </summary>
        [SerializeField]
        [FoldoutGroup("GameObjectPool")]
        public GameObjectPoolEntity[] GameObjectPoolGroups;

        /// <summary>
        /// 锁定的资源包（不会释放）
        /// </summary>
        [Header("锁定的资源包")]
        [FoldoutGroup("GameObjectPool")]
        public string[] LockedAssetBundle;

        [Header("标准分辨率的宽度")]
        [FoldoutGroup("UIGroup")]
        [SerializeField]
        public int m_StandardWidth = 1280;

        [Header("标准分辨率的高度")]
        [FoldoutGroup("UIGroup")]
        [SerializeField]
        public int m_StandardHeight = 720;

        [Header("UI摄像机")]
        [FoldoutGroup("UIGroup")]
        [SerializeField]
        public Camera UICamera;

        [Header("根画布")]
        [FoldoutGroup("UIGroup")]
        [SerializeField]
        public Canvas m_UIootCanvas;

        [Header("根画布的缩放")]
        [FoldoutGroup("UIGroup")]
        [SerializeField]
        public CanvasScaler UIRootCanvasScaler;

        [Header("UI分组")]
        [FoldoutGroup("UIGroup")]
        [SerializeField]
        public UIGroup[] UIGroups;


        public static GameEntry Instance;

        /// <summary>
        /// 全局参数设置
        /// </summary>
        public static ParamsSettings ParamsSettings
        {
            get;private set;
        }

        /// <summary>
        /// 全局参数设置
        /// </summary>
        public static ParamsSettings.DeviceGrade CurrDeviceGrade
        {
            get; private set;
        }
        #region 组件属性

        /// <summary>
        /// 事件组件
        /// </summary>
        public static EventManager Event
        {
            get; private set;
        }

        /// <summary>
        /// 时间组件
        /// </summary>
        public static TimeManager Time
        {
            get; private set;
        }

        /// <summary>
        /// 状态机组件
        /// </summary>
        public static FsmManager Fsm
        {
            get; private set;
        }

        /// <summary>
        /// 流程组件
        /// </summary>
        public static ProcedureManager Procedure
        {
            get; private set;
        }

        /// <summary>
        /// 数据表组件
        /// </summary>
        public static DataTableManager DataTable
        {
            get; private set;
        }

        /// <summary>
        /// Socket组件
        /// </summary>
        public static SocketManager Socket
        {
            get; private set;
        }

        /// <summary>
        /// Http组件
        /// </summary>
        public static HttpManager Http
        {
            get; private set;
        }

        /// <summary>
        /// 数据组件
        /// </summary>
        public static DataManager Data
        {
            get; private set;
        }

        /// <summary>
        /// 本地化组件
        /// </summary>
        public static LocalizationManager Localization
        {
            get; private set;
        }

        /// <summary>
        /// 对象池组件
        /// </summary>
        public static PoolManager Pool
        {
            get; private set;
        }

        /// <summary>
        /// 场景组件
        /// </summary>
        public static YouYouSceneManager Scene
        {
            get; private set;
        }

        /// <summary>
        /// 设置组件
        /// </summary>
        public static SettingManager Setting
        {
            get; private set;
        }

        /// <summary>
        /// 对象组件
        /// </summary>
        public static GameObjManager GameObj
        {
            get; private set;
        }

        /// <summary>
        /// 可寻址资源管理器
        /// </summary>
        public static AddressableManager Resource
        {
            get; private set;
        }

        /// <summary>
        /// 下载组件
        /// </summary>
        public static DownloadManager Download
        {
            get; private set;
        }

        /// <summary>
        /// UI组件
        /// </summary>
        public static YouYouUIManager UI
        {
            get; private set;
        }
        
        /// <summary>
        /// Audio组件
        /// </summary>
        public static AudioManager Audio
        {
            get; private set;
        }

        /// <summary>
        /// 日志管理器
        /// </summary>
        public static LoggerManager Logger
        {
            get;private set;
        }

        public static GameDirectorManager GameDirector
        {
            get; private set;
        }

        #endregion

        #region 基础组件管理

        /// <summary>
        /// 基础组件的列表
        /// </summary>
        private static readonly LinkedList<YouYouBaseComponent> m_BaseComponent = new LinkedList<YouYouBaseComponent>();

        #region RegisterBaseComponent 注册基础组件

        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="component"></param>
        internal static void RegisterBaseComponent(YouYouBaseComponent component)
        {
            //获取到组件类型
            Type type = component.GetType();

            LinkedListNode<YouYouBaseComponent> curr = m_BaseComponent.First;
            while (curr != null)
            {
                if (curr.Value.GetType() == type)
                {
                    return;
                }

                curr = curr.Next;
            }

            //把组件加入最后一个节点
            m_BaseComponent.AddLast(component);
        }

        #endregion

        #region GetBaseComponent 获取基础组件

        public static T GetBaseComponent<T>() where T : YouYouBaseComponent
        {
            return (T)GetBaseComponent(typeof(T));
        }

        internal static YouYouBaseComponent GetBaseComponent(Type type)
        {
            LinkedListNode<YouYouBaseComponent> curr = m_BaseComponent.First;
            while (curr != null)
            {
                if (curr.Value.GetType() == type)
                {
                    return curr.Value;
                }

                curr = curr.Next;
            }

            return null;
        }

        #endregion

        #endregion

        private void Awake()
        {
            Instance = this;
            CurrDeviceGrade = m_CurrDeviceGrade;
            ParamsSettings = m_ParamsSettings;
        }

        #region InitManagers 初始化管理器
        /// <summary>
        /// 初始化管理器
        /// </summary>
        private static void InitManagers()
        {
            Event = new EventManager();
            Time = new TimeManager();
            Fsm = new FsmManager();
            Procedure = new ProcedureManager();
            DataTable = new DataTableManager();
            Pool = new PoolManager();
            Socket = new SocketManager();
            Http = new HttpManager();
            Data = new DataManager();
            Localization = new LocalizationManager();
            Scene = new YouYouSceneManager();
            Resource = new AddressableManager();
            Download = new DownloadManager();
            UI = new YouYouUIManager();
            Audio = new AudioManager();
            Logger = new LoggerManager();
            GameDirector = new GameDirectorManager();
            
            Logger.Init();
            Event.Init();
            Time.Init();
            Fsm.Init();
            Procedure.Init();
            DataTable.Init();
            Socket.Init();
            Http.Init();
            Data.Init();
            Localization.Init();
            Pool.Init();
            Scene.Init();
            Resource.Init();
            Download.Init();
            UI.Init();
            Audio.Init();
            GameDirector.Init();
            Procedure.ChangeState(ProcedureState.Launch);
        }
        #endregion

        void Start()
        {
            InitManagers();
        }

        void Update()
        {
            Time.OnUpdate();
            Procedure.OnUpdate();
            Socket.OnUpdate();
            Pool.OnUpdate();
            Scene.OnUpdate();
            Resource.OnUpdate();
            Download.OnUpdate();
            UI.OnUpdate();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        private void OnDestroy()
        {
            Event.Dispose();
            Time.Dispose();
            Fsm.Dispose();
            Procedure.Dispose();
            DataTable.Dispose();
            Socket.Dispose();
            Http.Dispose();
            Data.Dispose();
            Localization.Dispose();
            Pool.Dispose();
            Scene.Dispose();
            Resource.Dispose();
            Download.Dispose();
            UI.Init();
            Audio.Dispose();
        }

        /// <summary>
        /// 打印日志 
        /// </summary>
        /// <param name="catetory"></param>
        /// <param name="message"></param>
        public static void Log(LogCategory catetory, string message, params object[] args)
        {
            switch (catetory)
            {
                default:
                case LogCategory.Normal:
#if DEBUG_LOG_NORMAL && DEBUG_MODEL
                    Debug.Log("[YouYou]" + (args.Length == 0 ? message : string.Format(message, args)));
#endif
                    break;
                case LogCategory.Procedure:
#if DEBUG_LOG_PROCEDURE && DEBUG_MODEL
                    Debug.Log("[YouYou]" + (string.Format("<color=#ffffff>{0}</color>", args.Length == 0 ? message : string.Format(message, args))));
#endif
                    break;
                case LogCategory.Resource:
#if DEBUG_LOG_PROCEDURE && DEBUG_MODEL
                    Debug.Log("[YouYou]" + (string.Format("<color=#ACE44A>{0}</color>", args.Length == 0 ? message : string.Format(message, args))));
#endif
                    break;
                case LogCategory.Proto:
#if DEBUG_LOG_PROTO && DEBUG_MODEL
                    Debug.Log("[YouYou]" + (args.Length == 0 ? message : string.Format(message, args)));
#endif
                    break;
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(string message, params object[] args)
        {
#if DEBUG_LOG_ERROR && DEBUG_MODEL
            Debug.LogError("[YouYou]" + (args.Length == 0 ? message : string.Format(message, args)));
#endif
        }
    }
}