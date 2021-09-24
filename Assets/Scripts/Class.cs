using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace Arycs_Fe.Models
{
    public class Class 
    {
        public Sys_ClassEntity info { get; private set; }

        public MoveConsumption moveConsumption
        {
            get
            {
                return GameEntry.Data.RoleDataManager.GetOrCreateMoveConsumption(info.ClassType);
            }
        }

        public Class(Sys_ClassEntity info)
        {
            this.info = info;
        }
    }
}