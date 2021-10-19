using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

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
        if (action.SelectedUnit == null)
        {
            return false;
        }

        Vector3Int position = new Vector3Int(x, y, 0);
        CellData cellData = action.Map.GetCellData(position);
        if (cellData == null || cellData.hasMapObject)
        {
            return false;
        }

        action.SelectedCell.mapObject = null;
        action.SelectedUnit.UpdatePosition(position);
        cellData.mapObject = action.SelectedUnit;
        return true;
    }
}
