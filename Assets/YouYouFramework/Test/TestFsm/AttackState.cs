using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public class AttackState : FsmState<BaseRoleController>
{
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("角色进入到了攻击状态");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Debug.Log("角色进入到了攻击状态");
    }

    public override void OnLeave()
    {
        base.OnLeave();
        Debug.Log("角色离开攻击状态");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("角色已经被销毁");
    }
}
