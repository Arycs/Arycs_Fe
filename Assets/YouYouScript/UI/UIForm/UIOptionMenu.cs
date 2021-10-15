using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using UnityEngine.UI;
using YouYou;

public class UIOptionMenu : UIFormBase
{
    [SerializeField]
    private GameObject btnPrefabParent;

    private int m_OptionNums;

    private string[] m_MenuInfos;

    private string m_OptionName;

    private Transform[] m_ButtonGroup;
    protected override void OnInit(object userData)
    {
        // GameEntry.Pool.GameObjectPool.Spawn();
    }

    protected override void OnOpen(object userData)
    {
        BaseParams baseParams = userData as BaseParams;
        m_OptionNums = baseParams.IntParam1;
        m_OptionName = baseParams.StringParam1;
        m_ButtonGroup = new Transform[m_OptionNums];
        for (int i = 1; i <= m_OptionNums; i++)
        {   
            GameEntry.Pool.GameObjectPool.Spawn(PrefabId.OptionBtn,(transform =>
            {
                transform.gameObject.name = "OptionBtn" + i;
                transform.GetComponent<Button>().onClick.AddListener(onBtnClick);
                transform.SetParent(btnPrefabParent.transform,false);
                Text tempText = transform.GetComponentInChildren<Text>();
                if (i == 1)
                {
                    tempText.text = baseParams.StringParam2;
                }else if (i == 2)
                {
                    tempText.text = baseParams.StringParam3;
                }else if (i == 3)
                {
                    tempText.text = baseParams.StringParam4;
                }else if (i == 4)
                {
                    tempText.text = baseParams.StringParam5;
                }

                m_ButtonGroup[i - 1] = transform;
            }));
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
            baseParams.StringParam1 = m_OptionName;
            GameEntry.Event.CommonEvent.Dispatch(SysEventId.UIMenuOptionDown,baseParams);
            Close();
        }
        Debug.LogError("关闭 OptionMenu 界面" + buttonSelf.name);
    }


    private void CreateBtn()
    {
        
    }

    protected override void OnClose()
    {
        for (int i = 0; i < m_OptionNums; i++)
        {
            m_ButtonGroup[i].GetComponent<Button>().onClick.RemoveListener(onBtnClick);
            GameEntry.Pool.GameObjectPool.Despawn(2,m_ButtonGroup[i]);
        }

        // Array.Clear(m_ButtonGroup,0,m_OptionNums);
    }

    protected override void OnBeforDestroy()
    {
        
    }
}
