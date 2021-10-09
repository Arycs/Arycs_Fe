using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

[Serializable]
public class TurnCondition : Condition
{
    //最小回合
    public int minTurn;
    //最大回合
    public int maxTurn;
    //回合内的阵营
    public bool allAttitudeTowards;

    public AttitudeTowards attitudeTowards;

    public override MapEventConditionType type
    {
        get { return MapEventConditionType.TurnCondition; }
    }

    public sealed override bool GetResult(MapAction action)
    {
        if (!allAttitudeTowards && action.Turn != attitudeTowards)
        {
            return false;
        }

        int turn = action.TurnToken;
        //如果最大回合数小于 0 则只取最小回合
        if (maxTurn < 0)
        {
            return turn >= minTurn;
        }

        return turn >= minTurn && turn <= maxTurn;
    }
}
