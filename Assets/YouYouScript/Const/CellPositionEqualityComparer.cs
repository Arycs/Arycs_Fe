using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using UnityEngine;

public class CellPositionEqualityComparer : IEqualityComparer<CellData>
{
    public bool Equals(CellData x, CellData y)
    {
        return x.position == y.position;
    }

    public int GetHashCode(CellData obj)
    {
        return obj.position.GetHashCode();
    }
}
