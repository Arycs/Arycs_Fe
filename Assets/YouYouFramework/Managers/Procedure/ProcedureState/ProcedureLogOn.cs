using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 登录流程
    /// </summary>
    public class ProcedureLogOn : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.Log(LogCategory.Procedure,"OnEnter ProcedureLogOn");

            GameEntry.UI.OpenUIForm(UIFormId.UI_LogonBG, onOpen: OnLogonBGOpen);
        }

        private void OnLogonBGOpen(UIFormBase uiFormBase)
        {
            GameEntry.UI.OpenUIForm(UIFormId.UI_Login);

            GameEntry.Event.CommonEvent.Dispatch(SysEventId.CloseCheckVersionUI);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLeave()
        {
            base.OnLeave();
            GameEntry.Log(LogCategory.Procedure, "OnLeave ProcedureLogOn");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
