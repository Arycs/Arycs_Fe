using System;
using UnityEngine;
using UnityEngine.UI;
using YouYou;

public class UITalkForm : UIFormBase
{
    public Image LeftRole;

    public Image RightRole;

    public Text TalkInfo;
    
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }

    protected override void OnOpen(object userData)
    {
        Debug.LogError("Open UITalkForm");
        base.OnOpen(userData);
        BaseParams baseParams = userData as BaseParams;
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

