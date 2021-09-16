using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class CalcExecutor : ScenarioContentExecutor<CalcExecutor.CalcArgs>
    {
        public struct CalcArgs
        {
            public string name;
            public string equalOp;
            public int value1;
            public string binaryOp;
            public int value2;
        }

        public override string code
        {
            get { return "calc"; }
        }

        protected bool IsMatchBinaryOperator(string opStr, ref string binaryOp, out string error)
        {
            switch (opStr)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "&":
                case "|":
                case "^":
                    binaryOp = opStr;
                    error = null;
                    return true;
                default:
                    error = GetMatchOperatorErrorString(
                        opStr, "+", "-", "*", "/", "&", "|", "^");
                    return false;
            }
        }

        protected bool IsMatchEqualOperator(string opStr, ref string equalOp, out string error)
        {
            if (opStr == "=")
            {
                equalOp = opStr;
            }
            else
            {
                if (!IsMatchBinaryOperator(opStr.Substring(0, 1), ref equalOp, out error))
                {
                    error = GetMatchOperatorErrorString(
                        opStr,
                        "=", "+", "-=", "*=", "/=", "&=", "|=", "^=");
                    return false;
                }
            }

            error = null;
            return true;
        }

        public override bool ParseArgs(IScenarioContent content, ref CalcArgs args, out string error)
        {
            //calc var += 10;
            //calc var = var + 10;
            if (content.length != 4 && content.length != 6)
            {
                error = GetLengthErrorString(4, 6);
                return false;
            }

            //在计算之前，变量必须存在
            //处理等号左值
            if (!IsMatchVar(content[1], true, ref args.name, out error))
            {
                return false;
            }

            if (!IsMatchEqualOperator(content[2], ref args.equalOp, out error))
            {
                return false;
            }

            // 处理等号右值
            if (!ParseOrGetVarValue(content[3], ref args.value1, out error))
            {
                return false;
            }

            if (content.length == 4)
            {
                args.binaryOp = "+";
                args.value2 = 0;
            }
            else
            {
                if (!IsMatchBinaryOperator(content[4], ref args.binaryOp, out error))
                {
                    return false;
                }

                if (!ParseOrGetVarValue(content[5], ref args.value2, out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        protected bool CalculateBinaryResult(string binaryOp, int value1, int value2, out int binaryResult,
            out string error)
        {
            binaryResult = 0;
            switch (binaryOp)
            {
                case "+":
                    binaryResult = value1 + value2;
                    break;
                case "-":
                    binaryResult = value1 - value2;
                    break;
                case "*":
                    binaryResult = value1 * value2;
                    break;
                case "/":
                    if (value2 == 0)
                    {
                        error = "CalcExecutor -> the dividend can not be zero";
                        return false;
                    }

                    binaryResult = value1 / value2;
                    break;
                case "&":
                    binaryResult = value1 & value2;
                    break;
                case "|":
                    binaryResult = value1 | value2;
                    break;
                case "^":
                    binaryResult = value1 ^ value2;
                    break;
            }

            error = null;
            return true;
        }

        protected override ScenarioActionStatus Run(IGameAction gameAction, IScenarioContent content, CalcArgs args,
            out string error)
        {
            int binaryResult;
            if (!CalculateBinaryResult(args.binaryOp,args.value1,args.value2,out binaryResult, out error))
            {
                return ScenarioActionStatus.Error;
            }

            int equalResult = 0;
            if (args.equalOp == "=")
            {
                equalResult = binaryResult;
            }
            else
            {
                int oldValue = ScenarioBlackboard.Get(args.name);
                if (!CalculateBinaryResult(args.equalOp,oldValue,binaryResult,out binaryResult,out error))
                {
                    return ScenarioActionStatus.Error;
                }
            }
            ScenarioBlackboard.Set(args.name,equalResult);
            return ScenarioActionStatus.Continue;
        }
    }
}