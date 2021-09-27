using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

[Serializable]
public class PositionCondition : Condition
{
    public int x;
    public int y;

    public override MapEventConditionType type
    {
        get { return MapEventConditionType.PositionCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        if (action.selectedUnit == null)
        {
            return false;
        }

        Vector3Int position = action.selectedUnit.cellPosition;
        return position.x == x && position.y == y;
    }
}
