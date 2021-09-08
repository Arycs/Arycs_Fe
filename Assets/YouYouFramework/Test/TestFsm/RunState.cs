using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public class RunState : FsmState<BaseRoleController>
{
    public override void OnEnter()
    {
        base.OnEnter();
        string name = curFsm.GetData<string>("name");
        
        Debug.Log("角色 : "+name +"进入到了奔跑状态");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Debug.Log("角色进入到了奔跑状态");
    }

    public override void OnLeave()
    {
        base.OnLeave();
        Debug.Log("角色离开奔跑状态");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("角色已经被销毁");
    }
}
