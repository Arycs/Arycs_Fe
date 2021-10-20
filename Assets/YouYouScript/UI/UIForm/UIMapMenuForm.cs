using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using UnityEngine.UI;
using YouYou;

public class UIMapMenuForm : UIFormBase
{
    [SerializeField]
    private GameObject btnPrefabParent;

    private int m_OptionNums;

    private string[] m_MenuInfos;

    private List<Transform> m_ButtonGroup;

    protected override void OnInit(object userData)
    {
        m_ButtonGroup = new List<Transform>();
    }

    protected override void OnOpen(object userData)
    {
        if (userData is HashSet<MenuTextID> menu)
        {
            m_OptionNums = menu.Count;
            foreach (MenuTextID menuItem in menu)
            {
                GameEntry.Pool.GameObjectPool.Spawn(PrefabId.OptionBtn, (transform =>
                {
                    transform.SetParent(btnPrefabParent.transform, false);
                    transform.name = menuItem.ToString() + (int) menuItem;
                    transform.GetComponent<Button>().onClick.AddListener(onBtnClick);
                    transform.SetParent(btnPrefabParent.transform, false);
                    Text tempText = transform.GetComponentInChildren<Text>();
                    tempText.text = menuItem.ToString();
                    m_ButtonGroup.Add(transform);
                }));
            }
        }
    }

    private void onBtnClick()
    {
        var buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (buttonSelf != null)
        {
            BaseParams baseParams = GameEntry.Pool.DequeueClassObject<BaseParams>();
            baseParams.Reset();
            RegexUtility.IsMatchNumber(buttonSelf.name, out baseParams.IntParam1); 
            GameEntry.Event.CommonEvent.Dispatch(CommonEventId.UIMapMenuOnClick,baseParams);
            GameEntry.Pool.EnqueueClassObject(baseParams);
            Close();
        }
        Debug.LogError("关闭 OptionMenu 界面" + buttonSelf.name);
    }
    
    protected override void OnClose()
    {
        for (int i = 0; i < m_OptionNums; i++)
        {
            m_ButtonGroup[i].GetComponent<Button>().onClick.RemoveListener(onBtnClick);
            GameEntry.Pool.GameObjectPool.Despawn(2,m_ButtonGroup[i]);
        }
        m_ButtonGroup.Clear();
    }

    protected override void OnBeforDestroy()
    {
        base.OnBeforDestroy();
    }
}
