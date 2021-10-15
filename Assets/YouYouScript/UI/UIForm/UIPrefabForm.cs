using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YouYou;

public class UIPrefabForm : UIFormBase
{
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        Debug.Log("检查要打开的UI的脚本是否未替换");
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        Debug.Log("检查要打开的UI的脚本是否未替换");
    }

    protected override void OnClose()
    {
        base.OnClose();
        Debug.Log("UITaskForm Close");
    }

    protected override void OnBeforDestroy()
    {
        base.OnBeforDestroy();
        Debug.Log("UITaskForm OnBeforDestory");
    }
}
