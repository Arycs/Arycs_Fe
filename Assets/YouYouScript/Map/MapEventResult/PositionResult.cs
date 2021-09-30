using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

[Serializable]
public class PositionResult : Result
{
    public int x;
    public int y;

    public override MapEventResultType type
    {
        get { return MapEventResultType.PositionResult; }
    }

    public override bool Trigger(MapAction action)
    {
        if (action.selectedUnit == null)
        {
            return false;
        }

        Vector3Int position = new Vector3Int(x, y, 0);
        CellData cellData = action.map.GetCellData(position);
        if (cellData == null || cellData.hasMapObject)
        {
            return false;
        }

        action.selectedCell.mapObject = null;
        action.selectedUnit.UpdatePosition(position);
        cellData.mapObject = action.selectedUnit;
        return true;
    }
}
