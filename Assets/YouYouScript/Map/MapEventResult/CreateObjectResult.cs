using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

public class CreateObjectResult : Result
{
   public MapObjectInfo[] objects;

   [HideInInspector]
   public string error;

   public override MapEventResultType type
   {
      get { return MapEventResultType.CreateObjectResult; }
   }

   public override bool Trigger(MapAction action)
   {
      if (objects == null || objects.Length == 0)
      {
         return true;
      }

      ActionStatus status = ActionStatus.Continue;

      for (int i = 0; i < objects.Length; i++)
      {
         if (objects[i].objectType == MapObjectType.Obstacle)
         {
            ObstacleInfo info = objects[i] as ObstacleInfo;
            status = action.ObjectCommandCreateObstacle(info.prefab, new Vector3Int(info.x, info.y, 0), out error);
         }else if (objects[i].objectType == MapObjectType.Class)
         {
            ClassInfo info = objects[i] as ClassInfo;
            if (info.roleType == RoleType.Unique)
            {
               status = action.ObjectCommandCreateClassUnique(info.attitudeTowards, info.id,
                  new Vector3Int(info.x, info.y, 0), out error);
            }
            else
            {
               status = action.ObjectCommandCreateClassFollowing(info.attitudeTowards, info.id,
                  new Vector3Int(info.x, info.y, 0),
                  (info as ClassFollowingInfo).level, (info as ClassFollowingInfo)?.items, out error);
            }
         }
         else
         {
            error = "CreateObjectResult -> object type can not be `Cursor` or `MouseCursor`.";
         }

         if (status == ActionStatus.Error)
         {
            return false;
         }
      }

      return true;
   }
}
