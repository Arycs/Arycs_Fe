using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public class ProcedureGame : ProcedureBase
{
        public override void OnEnter()
        {
            Debug.Log("进入到游戏流程");
            base.OnEnter();
            GameEntry.GameDirector.RunScenario(1);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    
        public override void OnLeave()
        {
            base.OnLeave();
        }
    
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

}
