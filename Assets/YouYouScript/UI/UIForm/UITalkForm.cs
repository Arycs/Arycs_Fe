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
        Debug.LogError("Open UITalkForm");
        base.OnOpen(userData);
        BaseParams baseParams = userData as BaseParams;
        
        TalkInfo.text = "位置" + baseParams.StringParam1 +"内容 : "+ baseParams.StringParam2 + "是否异步输出 :" + baseParams.BoolParam1;
        isWrited = true;
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

