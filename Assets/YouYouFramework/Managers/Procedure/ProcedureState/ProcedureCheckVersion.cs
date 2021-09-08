using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
    /// <summary>
    /// 检查版本流程
    /// </summary>
    public class ProcedureCheckVersion : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter ProcedureCheckVersion");

#if DISABLE_ASSETBUNDLE
            GameEntry.Procedure.ChangeState(ProcedureState.Preload);
#else
            GameEntry.Resource.InitStreamingAssetsBundleInfo();
#endif
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLeave()
        {
            base.OnLeave();
            Debug.Log("OnLeave ProcedureCheckVersion");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}