using System;
using UnityEngine;
using UnityEngine.UI;
using YouYou;

public class UITalkForm : UIFormBase
{
    public Image LeftRole;

    public Text TalkInfo;

    public bool isWrited;
    
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }

    protected override void OnOpen(object userData)
    {
        GameEntry.Event.CommonEvent.AddEventListener(SysEventId.UITalkClose, OnCloseByMessage);
        GameEntry.Event.CommonEvent.AddEventListener(SysEventId.UITalkStateUpdate,OnChangeWriteState);
        Debug.LogError("Open UITalkForm");
        base.OnOpen(userData);
        BaseParams baseParams = userData as BaseParams;
        
        TalkInfo.text = "位置" + baseParams.StringParam1 +"内容 : "+ baseParams.StringParam2 + "是否异步输出 :" + baseParams.BoolParam1;
    }

    private void OnChangeWriteState(object userdata)
    {
        BaseParams baseParams = GameEntry.Pool.DequeueClassObject<BaseParams>();
        baseParams.Reset();
        baseParams.BoolParam1 = true;
        GameEntry.Event.CommonEvent.Dispatch(SysEventId.UITalkWriteDown,baseParams);
    }

    private void OnCloseByMessage(object userdata)
    {
        Close();
    }

    protected override void OnClose()
    {
        base.OnClose();
    }

    protected override void OnBeforDestroy()
    {
        base.OnBeforDestroy();
    }
}

