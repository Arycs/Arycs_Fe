using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class IfGotoExecutor : ScenarioContentExecutor<IfGotoExecutor.IfGotoArgs>
    {
        public struct IfGotoArgs
        {
            public int left;
            public string condition;
            public int right;
            public string flag;
        }

        public override string code
        {
            get { return "if"; }
        }
        public override bool ParseArgs(IScenarioContent content, ref IfGotoArgs args, out string error)
        {
            //if var goto #flag
            //if !var goto #flag
            //if var >= num goto #flag
            if (content.length != 4 && content.length != 6)
            {
                error = GetLengthErrorString(4, 6);
                return false;
            }

            string gotoStr;
            if (content.length == 4)
            {
                string varName;
                if (content[1][0] == '!')
                {
                    varName = content[1].Remove(0, 1);
                    args.condition = "==";
                }
                else
                {
                    varName = content[1];
                    args.condition = "!=";
                }

                if (!ParseOrGetVarValue(varName,ref args.left,out error))
                {
                    return false;
                }

                args.right = 0;
                gotoStr = content[2];
                args.flag = content[3];
            }
            else
            {
                if (!ParseOrGetVarValue(content[1],ref args.left,out error))
                {
                    return false;
                }

                if (!IsMatchConditionOperator(content[2],ref args.condition,out error))
                {
                    return false;
                }

                if (!ParseOrGetVarValue(content[3],ref args.right,out error))
                {
                    return false;
                }

                gotoStr = content[4];
                args.flag = content[5];
            }

            if (gotoStr != "goto")
            {
                error = string.Format(
                    "{0} ParseArgs error: keycode '{1}' is not equal to 'goto'", GetType().Name, gotoStr);
                return false;
            }

            error = null;
            return true;
        }

        protected override ScenarioActionStatus Run(IGameAction gameAction, IScenarioContent content, IfGotoArgs args, out string error)
        {
            ScenarioAction action;
            if (!ParseAction<ScenarioAction>(gameAction,out action,out error))
            {
                return ScenarioActionStatus.Error;
            }

            if (CompareResult(args.condition,args.left,args.right))
            {
                return action.GotoCommand(args.flag, out error);
            }

            error = null;
            return ScenarioActionStatus.Continue;
        }
        
        protected bool IsMatchConditionOperator(string opStr, ref string condition, out string error)
        {
            switch (opStr)
            {
                case "==":
                case "!=":
                case ">":
                case ">=":
                case "<":
                case "<=":
                    condition = opStr;
                    error = null;
                    return true;
                default:
                    error = GetMatchOperatorErrorString(
                        opStr,
                        "==", "!=",
                        ">", ">=",
                        "<", "<=");
                    return false;
            }
        }
        
        protected bool CompareResult(string condition, int left, int right)
        {
            bool result = false;
            switch (condition)
            {
                case "==":
                    result = left == right;
                    break;
                case "!=":
                    result = left != right;
                    break;
                case ">":
                    result = left > right;
                    break;
                case ">=":
                    result = left >= right;
                    break;
                case "<":
                    result = left < right;
                    break;
                case "<=":
                    result = left <= right;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}