using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 进入游戏流程
    /// </summary>
    public class ProcedureEnterGame : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter ProcedureEnterGame");
            string name = GameEntry.Procedure.GetData<string>("Name");
            int code = GameEntry.Procedure.GetData<int>("code");
            Debug.Log("name : " + name);
            Debug.Log("code : " + code);
            
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLeave()
        {
            Debug.Log("OnLeave ProcedureEnterGame");
            base.OnLeave();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}