using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    
public class VarExecutor : ScenarioContentExecutor<VarExecutor.VarArgs>
{
    public struct VarArgs
    {
        public string name;
        public int value;
    }

    public override string code
    {
        get { return "var"; }
    }

    public override bool ParseArgs(IScenarioContent content, ref VarArgs args, out string error)
    {
        // var a;
        // var b = 10;
        // var c = b;
        if (content.length != 2 && content.length != 4)
        {
            error = GetLengthErrorString(2, 4);
            return false;
        }
        //变量只能包含数字，字母(中文) ，下划线，且不能以数字开头
        if (!RegexUtility.IsMatchVariable(content[1]))
        {
            error = GetMatchVariableErrorString(content[1]);
            return false;
        }

        args.name = content[1];
        args.value = 0;
        if (content.length == 4)
        {
            if (content[2] != "=")
            {
                error = GetMatchOperatorErrorString(content[2], "=");
                return false;
            }

            if (!ParseOrGetVarValue(content[3],ref args.value,out error))
            {
                return false;
            }
        }
        error = null;
        return true;
    }

    protected override ScenarioActionStatus Run(IGameAction gameAction, IScenarioContent content, VarArgs args, out string error)
    {
        ScenarioBlackboard.Set(args.name,args.value);
        error = null;
        return ScenarioActionStatus.Continue;
    }
}
}
