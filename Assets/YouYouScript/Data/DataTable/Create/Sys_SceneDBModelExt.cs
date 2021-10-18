using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

public partial class Sys_SceneDBModel : DataTableDBModelBase<Sys_SceneDBModel, Sys_SceneEntity>
{
    public int GetSceneIdByName(string sceneName)
    {
        List<Sys_SceneEntity> entities = GetList();
        int len = entities.Count;
        for (int i = 0; i < len; i++)
        {
            if (entities[i].SceneName == sceneName)
            {
                return entities[i].Id;
            }
        }
        return 0;
    }
}
