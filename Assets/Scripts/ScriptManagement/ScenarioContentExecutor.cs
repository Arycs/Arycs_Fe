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

        public abstract ScenarioActionStatus
            Execute(IGameAction gameAction, IScenarioContent content, out string error);

        #region Methods
        /// <summary>
        /// 判断变量格式是否正确，并赋值给  ’variable‘
        /// </summary>
        /// <param name="varStr"></param>
        /// <param name="isExist">指定变量在使用之前，是否存在</param>
        /// <param name="variable"></param>
        /// <param name="error"></param>
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
        /// 如果是数字，直接赋值，如果是变量，就获取变量
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

        #region Error String Methodsd

        protected string GetLengthErrorString(params int[] correctLength)
        {
            if (correctLength == null || correctLength.Length == 0)
            {
                return string.Format(
                    "{0} ParseArgs error : length of 'content' is incorrect.", typeName);
            }
            else
            {
                return string.Format(
                    "{0} ParseArgs error : length of 'content' must be one of [{1}]", typeName,
                    string.Join(", ", correctLength.Select(length => length.ToString()).ToArray()));
            }
        }

        protected string GetMatchVariableErrorString(string variable)
        {
            return string.Format(
                "{0} ParseArgs error : variable '{1} match error'", typeName, variable);
        }

        protected string GetVariableExistErrorString(string variable, bool exist)
        {
            if (exist)
            {
                return string.Format(
                    "{0} ParseArgs error: variable `{1}` is already exist.",
                    typeName,
                    variable);
            }
            else
            {
                return string.Format(
                    "{0} ParseArgs error: variable `{1}` was not found.",
                    typeName,
                    variable);
            }
        }
        
        protected string GetMatchOperatorErrorString(string op, params string[] operators)
        {
            if (operators == null || operators.Length == 0)
            {
                return string.Format(
                    "{0} ParseArgs error: operator `{1}` is not supported.",
                    typeName,
                    op);
            }
            else
            {
                return string.Format(
                    "{0} ParseArgs error: operator `{1}` is not in [{2}]",
                    typeName,
                    op,
                    string.Join(", ", operators));
            }
        }

        protected string GetActionTypeErrorString(string currentActionType, string correctActionType = null)
        {
            if (string.IsNullOrEmpty(correctActionType))
            {
                return string.Format(
                    "{0} Execute error: action type `{1}` is incorrect.",
                    typeName,
                    currentActionType);
            }
            else
            {
                return string.Format(
                    "{0} Execute error: action type `{1}` does not inhert `{2}`.",
                    typeName,
                    currentActionType,
                    correctActionType);
            }
        }
        #endregion
        
        
    }
    
    public abstract class ScenarioContentExecutor<T> : ScenarioContentExecutor
    {
        public abstract bool ParseArgs(IScenarioContent content, ref T args, out string error);

        public sealed override ScenarioActionStatus Execute(IGameAction gameAction, IScenarioContent content,
            out string error)
        {
            T args = default(T);
            if (!ParseArgs(content,ref args,out error))
            {
                return ScenarioActionStatus.Error;
            }

            return Run(gameAction, content, args, out error);
        }

        protected abstract ScenarioActionStatus Run(IGameAction gameAction, IScenarioContent content, T args,
            out string error);
    }
}