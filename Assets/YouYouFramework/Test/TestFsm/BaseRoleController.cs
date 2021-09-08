using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public enum BaseRoleState {
   IdleState = 1,
   RunState = 2,
   AttackState = 3
}

public class BaseRoleController:MonoBehaviour
{
   public Fsm<BaseRoleController> curFsm;

   private void Awake()
   {
      
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.P))
      {
         FsmState<BaseRoleController>[] states = new FsmState<BaseRoleController>[3];
         states[0] = new IdleState();
         states[1] = new RunState();
         states[2] = new AttackState();
         curFsm = GameEntry.Fsm.Create(this, states);
      }
      if (Input.GetKeyDown(KeyCode.A))
      {
         curFsm.SetData("name","Arycs");
         curFsm.ChangeState(1);
      }

      if (Input.GetKeyDown(KeyCode.B))
      {
         curFsm.ChangeState(2);
      }

      if (Input.GetKeyDown(KeyCode.C))
      {
         curFsm.ChangeState(0);
      }
   }
}
