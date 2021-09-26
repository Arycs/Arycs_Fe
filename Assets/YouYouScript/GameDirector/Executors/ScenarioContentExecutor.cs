using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public abstract class ScenarioContentExecutor : IScenarioContentExecutor
    {
        public string typeName
        {
            get { return GetType().Name; }
        }
        
        public abstract string code { get; }

        public abstract ActionStatus Execute(IGameAction gameAction, IScenarioContent content, out string error);

        #region Methods
        /// <summary>
        /// 判断变量格式是否正确，并赋值给  ’variable‘
        /// </summary>
        /// <param name="varStr">要判断的变量名</param>
        /// <param name="isExist">指定变量在使用之前，是否存在</param>
        /// <param name="variable">返回的变量名</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        protected bool IsMatchVar(string varStr, bool isExist, ref string variable, out string error)
        {
            if (!RegexUtility.IsMatchVariable(varStr))
            {
                error = GetMatchVariableErrorString(varStr);
                return false;
            }

            if (ScenarioBlackboard.Contains(varStr) != isExist)
            {
                error = GetVariableExistErrorString(varStr, !isExist);
                return false;
            }

            variable = varStr;
            error = null;
            return true;
        }

        /// <summary>
        /// 解析或获取变量 ，如果是数字，直接赋值，如果是变量，就获取变量
        /// </summary>
        /// <param name="numOrVar"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected bool ParseOrGetVarValue(string numOrVar, ref int value, out string error)
        {
            if (!int.TryParse(numOrVar,out value))
            {
                if (!RegexUtility.IsMatchVariable(numOrVar))
                {
                    error = GetMatchVariableErrorString(numOrVar);
                    return false;
                }

                if (!ScenarioBlackboard.TryGet(numOrVar,out value))
                {
                    error = GetVariableExistErrorString(numOrVar, false);
                    return false;
                }
            }

            error = null;
            return true;
        }

        /// <summary>
        /// 转换Action
        /// </summary>
        /// <param name="inputAction"></param>
        /// <param name="outputAction"></param>
        /// <param name="error"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected bool ParseAction<T>(IGameAction inputAction, out T outputAction, out string error)
            where T : class, IGameAction
        {
            if (inputAction is T)
            {
                outputAction = inputAction as T;
                error = null;
                return true;
            }

            outputAction = null;
            error = GetActionTypeErrorString(inputAction.GetType().Name, typeof(T).Name);
            return false;
        }
        #endregion

        #region Error String Methods 匹配错误的相关方法 
        /// <summary>
        /// 字符串长度错误
        /// </summary>
        /// <param name="correctLength"></param>
        /// <returns></returns>
        protected string GetLengthErrorString(params int[] correctLength)
        {
            if (correctLength == null || correctLength.Length == 0)
            {
                //return $"{typeName} ParseArgs error : length of 'content' is incorrect.";
                return $"{typeName} 解析参数 错误 : 参数长度不正确.";
            }
            else
            {
                //return $"{typeName} ParseArgs error : length of 'content' must be one of [{string.Join(", ", correctLength.Select(length => length.ToString()).ToArray())}]";
                return $"{typeName} 解析参数 错误 : 内容长度必须为以下值 [{string.Join(", ", correctLength.Select(length => length.ToString()).ToArray())}]";
            }
        }
        /// <summary>
        /// 变量不符合规定，匹配错误
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        protected string GetMatchVariableErrorString(string variable)
        {
            // return $"{typeName} ParseArgs error : variable '{variable} match error'";
            return $"{typeName} 解析参数 错误 : 变量 '{variable} 匹配 错误'";
        }
        /// <summary>
        /// 变量已经存在或未被找到
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="exist"></param>
        /// <returns></returns>
        protected string GetVariableExistErrorString(string variable, bool exist)
        {
            if (exist)
            {
                // return $"{typeName} ParseArgs error: variable `{variable}` is already exist.";
                return $"{typeName} 解析参数 错误: 变量 `{variable}` 已经存在.";
            }
            else
            {
                //return $"{typeName} ParseArgs error: variable `{variable}` was not found.";
                return $"{typeName} 解析参数 错误: 变量 `{variable}` 未被找到.";
            }
        }
        /// <summary>
        /// 获取匹配运算符错误
        /// </summary>
        /// <param name="op"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        protected string GetMatchOperatorErrorString(string op, params string[] operators)
        {
            if (operators == null || operators.Length == 0)
            {
                // return $"{typeName} ParseArgs error: operator `{op}` is not supported.";
                return $"{typeName} 解析参数 错误: 运算符 `{op}` 不支持.";
            }
            else
            {
                // return $"{typeName} ParseArgs error: operator `{op}` is not in [{string.Join(", ", operators)}]";
                return $"{typeName} 解析参数 错误: 运算符 `{op}` 不在 [{string.Join(", ", operators)}] 之中";
            }
        }
        /// <summary>
        /// 获取动作类型错误
        /// </summary>
        /// <param name="currentActionType"></param>
        /// <param name="correctActionType"></param>
        /// <returns></returns>
        protected string GetActionTypeErrorString(string currentActionType, string correctActionType = null)
        {
            if (string.IsNullOrEmpty(correctActionType))
            {
                // return $"{typeName} Execute error: action type `{currentActionType}` is incorrect.";
                return $"{typeName} 执行 错误: 动作 类型 `{currentActionType}` 不正确.";
            }
            else
            {
                // return $"{typeName} Execute error: action type `{currentActionType}` does not inhert `{correctActionType}`.";
                return $"{typeName} 执行 错误: 动作 类型 `{currentActionType}` 不是 `{correctActionType}`.";
            }
        }
        #endregion
    }
    
    public abstract class ScenarioContentExecutor<T> : ScenarioContentExecutor
    {
        public abstract bool ParseArgs(IScenarioContent content, ref T args, out string error);

        public sealed override ActionStatus Execute(IGameAction gameAction, IScenarioContent content,
            out string error)
        {
            T args = default(T);
            if (!ParseArgs(content,ref args,out error))
            {
                return ActionStatus.Error;
            }

            return Run(gameAction, content, args, out error);
        }

        protected abstract ActionStatus Run(IGameAction gameAction, IScenarioContent content, T args,
            out string error);
    }
}