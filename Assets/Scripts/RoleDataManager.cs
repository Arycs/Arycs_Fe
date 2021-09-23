using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;
using YouYou;

public class RoleDataManager : IDisposable
{
    public Dictionary<int, Role> m_RoleDic = new Dictionary<int, Role>();

    public Role CreateRoleById(int id)
    {
        Sys_CharacterEntity entity = GameEntry.DataTable.Sys_CharacterDBModel.Get(id);
        if (entity != null)
        {
            
        }

        return null;
    }

    public void Dispose()
    {
    }
}
