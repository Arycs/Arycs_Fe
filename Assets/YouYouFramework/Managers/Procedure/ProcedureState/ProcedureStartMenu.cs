using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public class ProcedureStartMenu : ProcedureBase
{
    public override void OnEnter()
    {
        GameEntry.Log(LogCategory.Procedure, "OnEnter ProcedureStartMenu");
        GameEntry.UI.OpenUIForm(UIFormId.UI_Start);
        GameEntry.Event.CommonEvent.AddEventListener(SysEventId.CloseStartMenu,OnCloseStartMenuForm);
        base.OnEnter();
    }

    public void OnCloseStartMenuForm(object userData)
    {
        GameEntry.Event.CommonEvent.RemoveEventListener(SysEventId.CloseStartMenu,OnCloseStartMenuForm);
        GameEntry.UI.CloseUIForm(UIFormId.UI_Start);
        GameEntry.Procedure.ChangeState(ProcedureState.Game);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnLeave()
    {
        GameEntry.Log(LogCategory.Procedure, "OnLeave ProcedureStartMenu");
        base.OnLeave();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

}
