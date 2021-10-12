using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 开始流程
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter ProcedureLaunch");
            
#if DISABLE_ASSETBUNDLE
            GameEntry.Procedure.ChangeState(ProcedureState.Preload);
#else
            GameEntry.Resource.InitStreamingAssetsBundleInfo();
#endif
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //Debug.Log("OnUpdate ProcedureLaunch");
        }

        public override void OnLeave()
        {
            base.OnLeave();
            Debug.Log("OnLeave ProcedureLaunch");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}