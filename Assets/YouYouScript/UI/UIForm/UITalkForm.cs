using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YouYou;

public class UITalkForm : UIFormBase
{
    [SerializeField]
    private Image imgLeftRole;

    [SerializeField]
    private Text textTalkInfo;

    private bool m_IsAsync = false;

    private string m_TalkPosition;

    private string[] m_TalkInfoList;

    private int m_TalkInfoIndex;

    protected override void OnInit(object userData)
    {
        m_IsAsync = false;
        m_TalkPosition = "";
        m_TalkInfoIndex = 0;
    }

    protected override void OnOpen(object userData)
    {
        Debug.LogError("Open UITalkForm");

        GameEntry.Event.CommonEvent.AddEventListener(CommonEventId.UITalkClose, OnCloseByMessage);
        GameEntry.Event.CommonEvent.AddEventListener(CommonEventId.UITalkStateUpdate, OnChangeWriteState);

        BaseParams baseParams = userData as BaseParams;
        m_IsAsync = baseParams.BoolParam1;
        m_TalkPosition = baseParams.StringParam1;
        m_TalkInfoList = baseParams.StringParam2.TrimEnd((char[])"\n\r".ToCharArray()).Split('\n');

        // textTalkInfo.text = "位置" + baseParams.StringParam1 + "内容 : " + baseParams.StringParam2 + "是否异步输出 :" +
        //                      baseParams.BoolParam1;
        if (m_IsAsync)
        {
            WriteInfo();
        }
        else
        {
            WriteInfoAsync();
        }
    }

    public void WriteInfo()
    {
        textTalkInfo.text = m_TalkInfoList[m_TalkInfoIndex];
        m_TalkInfoIndex++;
    }

    public void WriteInfoAsync()
    {
        textTalkInfo.DOText(m_TalkInfoList[m_TalkInfoIndex], 10f);
        m_TalkInfoIndex++;
    }


    /// <summary>
    /// 检测写入状态改变
    /// </summary>
    /// <param name="userdata"></param>
    private void OnChangeWriteState(object userdata)
    {
        if (m_TalkInfoIndex < m_TalkInfoList.Length)
        {
            if (m_IsAsync)
            {
                textTalkInfo.DOComplete();
                WriteInfoAsync();                
            }
            else
            {
                WriteInfo();
            }
        }
        else
        {
            BaseParams baseParams = GameEntry.Pool.DequeueClassObject<BaseParams>();
            baseParams.Reset();
            baseParams.BoolParam1 = true;
            GameEntry.Event.CommonEvent.Dispatch(CommonEventId.UITalkWriteDown, baseParams);
        }
    }

    private void OnCloseByMessage(object userdata)
    {
        Close();
    }

    protected override void OnClose()
    {
        m_IsAsync = false;
        m_TalkPosition = "";
        m_TalkInfoIndex = 0;
        GameEntry.Event.CommonEvent.RemoveEventListener(CommonEventId.UITalkClose, OnCloseByMessage);
        GameEntry.Event.CommonEvent.RemoveEventListener(CommonEventId.UITalkStateUpdate, OnChangeWriteState);
    }

    protected override void OnBeforDestroy()
    {
    }
}