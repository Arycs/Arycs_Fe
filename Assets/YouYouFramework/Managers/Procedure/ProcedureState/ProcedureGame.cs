using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureGame : ProcedureBase
{
        public override void OnEnter()
        {
            Debug.Log("进入到游戏流程");
            base.OnEnter();
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
