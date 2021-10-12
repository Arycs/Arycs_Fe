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

            GameEntry.UI.OpenUIForm(UIFormId.UI_CheckVersion, onOpen: OnStartUIForm);
        }

        private void OnStartUIForm(UIFormBase uiFormBase)
        {
            GameEntry.Event.CommonEvent.Dispatch(SysEventId.CloseCheckVersionUI);

            GameEntry.UI.OpenUIForm(UIFormId.UI_Start);

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
