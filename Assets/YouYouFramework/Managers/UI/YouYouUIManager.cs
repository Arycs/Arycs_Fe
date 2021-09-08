using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YouYou
{
    /// <summary>
    /// UI组件
    /// </summary>
    public class YouYouUIManager : ManagerBase, IDisposable
    {
        private Dictionary<byte, UIGroup> m_UIGroupDic;
        
        /// <summary>
        /// 标准分辨率比值
        /// </summary>
        private float m_StandardScreen = 0;
        /// <summary>
        /// 当前分辨率比值
        /// </summary>
        private float m_CurrScreen = 0;

        private UILayer m_UILayer;

        private UIPool m_UIPool;

        public float UIClearInterval
        {
            get; private set;
        }

        /// <summary>
        /// UI回池后过期时间
        /// </summary>
        public float UIExpire
        {
            get;private set;
        }

        /// <summary>
        /// UI对象池中 最大的数量
        /// </summary>
        public int UIPoolMaxCount
        {
            get; private set;
        }

        /// <summary>
        /// 下次运行时间
        /// </summary>
        private float m_NextRunTime = 0f;
        
        public YouYouUIManager()
        {
            m_UILayer = new UILayer();
            m_UILayer.Init(GameEntry.Instance.UIGroups);
            m_OpenUIFormList = new LinkedList<UIFormBase>();
            m_UIPool = new UIPool();
            m_UIGroupDic = new Dictionary<byte, UIGroup>();
            m_ReverseChangeUIStack = new Stack<UIFormBase>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            UIPoolMaxCount = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.UI_PoolMaxCount, GameEntry.CurrDeviceGrade);
            UIExpire = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.UI_Exprie, GameEntry.CurrDeviceGrade);
            UIClearInterval = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.UI_ClearInterval, GameEntry.CurrDeviceGrade);

            m_StandardScreen = GameEntry.Instance.m_StandardWidth / (float)GameEntry.Instance.m_StandardHeight;
            m_CurrScreen = Screen.width / (float)Screen.height;
            NormalFormCanvasScaler();

            int len = GameEntry.Instance.UIGroups.Length;
            for (int i = 0; i < len; i++)
            {
                UIGroup group = GameEntry.Instance.UIGroups[i];
                m_UIGroupDic[group.Id] = group;
            }

            m_UILayer.Init(GameEntry.Instance.UIGroups);
        }
        #region UI适配
        /// <summary>
        /// LoadingForm适配缩放
        /// </summary>
        public void UILoadingFormCanvasScaler()
        {

            if (m_CurrScreen > m_StandardScreen)
            {
                //分辨率大于标准分辨率 则设置为0
                GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = 0;
            }
            else
            {
                //分辨率小于标准分辨率, 则用标准减去当前分辨率
                GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = m_CurrScreen - m_StandardScreen;
            }
        }

        /// <summary>
        /// FullForm适配缩放为1
        /// </summary>
        public void FullFormCanvasScaler()
        {
            GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = 1;
        }

        /// <summary>
        /// NormalForm适配缩放
        /// </summary>
        public void NormalFormCanvasScaler()
        {
            GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = m_CurrScreen >= m_StandardScreen ? 1 : 0;
        }
        #endregion

        #region GetUIGroup
        /// <summary>
        /// 根据UI分组编号,获取UI分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UIGroup GetUIGroup(byte id)
        {
            UIGroup group = null;
            m_UIGroupDic.TryGetValue(id, out group);
            return group;
        }
        #endregion

        
        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="formBase"></param>
        /// <param name="isAdd"></param>
        internal void SetSortingOrder(UIFormBase formBase, bool isAdd)
        {
            m_UILayer.SetSortingOrder(formBase,isAdd);
        }

        /// <summary>
        /// 从UI对象池中获取UI
        /// </summary>
        /// <param name="uiformId"></param>
        /// <returns></returns>
        internal UIFormBase Dequeue(int uiformId)
        {
            return m_UIPool.Dequeue(uiformId);
        }

        /// <summary>
        /// 回到UI对象池
        /// </summary>
        /// <param name="form"></param>
        internal void Enqueue(UIFormBase form)
        {
            m_UIPool.Enqueue(form);
        }

        /// <summary>
        /// 打开提示对话框
        /// </summary>
        /// <param name="sysCode"></param>
        /// <param name=""></param>
        /// <param name="onConfirm"></param>
        /// <param name="onCancel"></param>
        public void OpenDialogFormBySysCode(int sysCode, DialogFormType dialogFormType = DialogFormType.Normal, BaseAction onConfirm = null, BaseAction onCancel = null)
        {
            OpenDialForm(dialogFormType, GameEntry.Data.SysDataManager.GetSysCodeContent(sysCode), null, onConfirm, onCancel);
        }

        /// <summary>
        /// 打开提示框
        /// </summary>
        /// <param name="dialogFormType">提示框类型</param>
        /// <param name="content">内容</param>
        /// <param name="title">标题</param>
        /// <param name="onConfirm">确认回调</param>
        /// <param name="onCancel">取消回调</param>
        public void OpenDialForm(DialogFormType dialogFormType, string content, string title = null, BaseAction onConfirm = null, BaseAction onCancel = null)
        {
            BaseParams baseParams = GameEntry.Pool.DequeueClassObject<BaseParams>();
            baseParams.Reset();

            baseParams.IntParam1 = (int)dialogFormType;
            baseParams.StringParam1 = content;
            baseParams.StringParam2 = title;
            baseParams.ActionParam1 = onConfirm;
            baseParams.ActionParam2 = onCancel;

            OpenUIForm(2, baseParams);
        }

        public void Dispose()
        {
            //Debug.Log("UI组件关闭");
        }

        public void OnUpdate()
        {

            if (Time.time > m_NextRunTime + UIClearInterval)
            {
                m_NextRunTime = Time.time;
                //释放 UI对象池
                m_UIPool.CheckClear();
            }

        }

        /// <summary>
        /// 已经打开的UI链表
        /// </summary>
        private LinkedList<UIFormBase> m_OpenUIFormList;

        /// <summary>
        /// 反切UI栈
        /// </summary>
        private Stack<UIFormBase> m_ReverseChangeUIStack;

        #region  OpenUIForm 打开UI窗体

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormId">formId</param>
        /// <param name="userData">用户数据</param>
        public void OpenUIForm(int uiFormId, object userData = null, BaseAction<UIFormBase> onOpen = null)
        {
            //1. 读表
            Sys_UIFormEntity entity = GameEntry.DataTable.Sys_UIFormDBModel.Get(uiFormId);
            if (entity == null)
            {
                Debug.LogError("表格中没有对应UI窗体数据 : " + uiFormId);
                return;
            }

            if (!entity.CanMulit && IsExists(uiFormId))
            {
                return;
            }

            UIFormBase formBase = GameEntry.UI.Dequeue(uiFormId); //以后从对象池获取
            if (formBase == null)
            {
                //TODO : 异步加载UI需要时间 此处需要处理过滤加载中的UI

                string assetPath = string.Empty;
                switch (GameEntry.Localization.CurrLanguage)
                {
                    case YouYouLanguage.Chinese:
                        assetPath = entity.AssetPath_Chinese;
                        break;
                    case YouYouLanguage.English:
                        assetPath = entity.AssetPath_English;
                        break;
                }

                LoadUIAsset(assetPath, (ResourceEntity resourceEntity) =>
                {
                    GameObject uiObj = UnityEngine.Object.Instantiate((UnityEngine.Object)resourceEntity.Target) as GameObject;

                    //把克隆出的资源 加入实例资源池
                    GameEntry.Pool.RegisterInstanceResource(uiObj.GetInstanceID(), resourceEntity);

                    uiObj.transform.SetParent(GameEntry.UI.GetUIGroup(entity.UIGroupId).Group);
                    uiObj.transform.localPosition = Vector3.zero;
                    uiObj.transform.localScale = Vector3.one;

                    formBase = uiObj.GetComponent<UIFormBase>();
                    formBase.Init(uiFormId, entity.UIGroupId, entity.DisableUILayer == 1, entity.IsLock == 1, userData);
                    m_OpenUIFormList.AddLast(formBase);

                    OpenUI(entity, formBase, onOpen);
                });
            }
            else
            {
                formBase.Open(userData);
                m_OpenUIFormList.AddLast(formBase);
                ShowUI(formBase);

                OpenUI(entity, formBase, onOpen);
            }

            m_UIPool.CheckByOpenUI();
        }

        #endregion

        /// <summary>
        /// 打开UI 
        /// </summary>
        /// <param name="sys_UIFormEntity"></param>
        /// <param name="formBase"></param>
        /// <param name="onOpen"></param>
        private void OpenUI(Sys_UIFormEntity sys_UIFormEntity, UIFormBase formBase, BaseAction<UIFormBase> onOpen)
        {
            //判断反切UI
            UIFormShowMode uiFormShowMode = (UIFormShowMode)sys_UIFormEntity.ShowMode;
            if (uiFormShowMode == UIFormShowMode.ReverseChange)
            {
                //如果之前栈里边有UI
                if (m_ReverseChangeUIStack.Count > 0)
                {
                    //从栈顶拿到UI
                    UIFormBase topUIForm = m_ReverseChangeUIStack.Peek();

                    //禁用 冻结
                    HideUI(topUIForm);
                }
                m_ReverseChangeUIStack.Push(formBase);
            }
            onOpen?.Invoke(formBase);
        }

        #region LoadUIAsset 加载UI资源

        private void LoadUIAsset(string assetPath, BaseAction<ResourceEntity> onComplete)
        {
            GameEntry.Resource.ResourceLoaderManager.LoadMainAsset(AssetCategory.UIPrefab,
                string.Format("Assets/Download/UI/UIPrefab/{0}.prefab", assetPath),
                (ResourceEntity resourceEntity) =>
                {
                    if (onComplete != null)
                    {
                        onComplete(resourceEntity);
                    }
                });
        }

        #endregion

        /// <summary>
        /// 检查UI是否已经打开
        /// </summary>
        /// <returns></returns>
        public bool IsExists(int uiformId)
        {
            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.UIFormId == uiformId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 显示/激活一个UI
        /// </summary>
        /// <param name="uiFormBase"></param>
        public void ShowUI(UIFormBase uiFormBase)
        {
            uiFormBase.IsActive = true;
            uiFormBase.currCanvas.enabled = true;
            uiFormBase.gameObject.layer = 5;
        }

        /// <summary>
        /// 隐藏/冻结一个UI
        /// </summary>
        /// <param name="uiFormBase"></param>
        public void HideUI(UIFormBase uiFormBase)
        {
            uiFormBase.IsActive = false;
            uiFormBase.currCanvas.enabled = false;
            uiFormBase.gameObject.layer = 0;
        }

        /// <summary>
        /// 根据uiformId 关闭UI窗口
        /// </summary>
        /// <param name="uiformId"></param>
        internal void CloseUIForm(int uiformId)
        {
            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.UIFormId == uiformId)
                {
                    CloseUIForm(curr.Value);
                    break;
                }
            }
        }

        internal void CloseUIFormByInstanceID(int instanceID)
        {
            for (LinkedListNode<UIFormBase> curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.gameObject.GetInstanceID() == instanceID)
                {
                    CloseUIForm(curr.Value);
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="formBase"></param>
        internal void CloseUIForm(UIFormBase formBase)
        {
            m_OpenUIFormList.Remove(formBase);
            formBase.ToClose();
            
            //判断UI显示类型
            Sys_UIFormEntity entity = GameEntry.DataTable.Sys_UIFormDBModel.Get(formBase.UIFormId);
            if (entity == null)
            {
                GameEntry.LogError(formBase.UIFormId + "对应的UI窗体不存在");
            }
            HideUI(formBase);
            //判断反切UI
            UIFormShowMode uiFormShowMode = (UIFormShowMode)entity.ShowMode;
            if (uiFormShowMode == UIFormShowMode.ReverseChange)
            {
                m_ReverseChangeUIStack.Pop();
                if (m_ReverseChangeUIStack.Count > 0)
                {
                    UIFormBase topForms = m_ReverseChangeUIStack.Peek();
                    ShowUI(topForms);
                }
            }
        }
    }
}
