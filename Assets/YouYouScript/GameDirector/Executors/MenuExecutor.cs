using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace Arycs_Fe.ScriptManagement
{
    public class MenuExecutor : ScenarioContentExecutor<MenuExecutor.MenuArgs>
    {
        public struct MenuArgs
        {
            public string menuName;
            public string[] options;
        }

        public override string code
        {
            get { return "menu"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref MenuArgs args, out string error)
        {
            // menu option
            //      option0
            //      option1
            //      ...;
            if (content.length < 3)
            {
                error = GetLengthErrorString();
                return false;
            }

            // 获取使用的变量
            if (!RegexUtility.IsMatchVariable(content[1]))
            {
                error = GetMatchVariableErrorString(content[1]);
                return false;
            }

            args.menuName = content[1];
            List<string> options = new List<string>();
            int index = 2;
            while (index < content.length)
            {
                string line;
                if (content[index].StartsWith("\""))
                {
                    if (!ScenarioUtility.ParseContentString(content, ref index, out line, out error))
                    {
                        return false;
                    }
                }
                else
                {
                    //可能是个变量
                    int id = -1;
                    if (!ParseOrGetVarValue(content[index], ref id, out error))
                    {
                        return false;
                    }

                    //TODO
                    line = "语言包获取";
                    if (string.IsNullOrEmpty(line))
                    {
                        error = $"{typeName} ParseArgs error : text id '{content[index]}' was not fount";
                        return false;
                    }

                    index++;
                }

                options.Add(line);
            }

            args.options = options.ToArray();
            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, MenuArgs args,
            out string error)
        {
            ScenarioAction action;
            if (!ParseAction(gameAction, out action, out error))
            {
                return ActionStatus.Error;
            }

            // 重置变为 -1
            ScenarioBlackboard.Set(args.menuName, -1);
            BaseParams baseParams = GameEntry.Pool.DequeueClassObject<BaseParams>();
            baseParams.Reset();
            baseParams.IntParam1 = args.options.Length;
            baseParams.StringParam1 = args.menuName;
            int i = 0;
            while (i < args.options.Length )
            {
                if ( i == 0)
                {
                    baseParams.StringParam2 = args.options[i];
                }else if (i == 1)
                {
                    baseParams.StringParam3 = args.options[i];
                }else if (i == 2)
                {
                    baseParams.StringParam4 = args.options[i];
                }else if (i == 3)
                {
                    baseParams.StringParam5 = args.options[i];
                }
            }
            GameEntry.UI.OpenUIForm(UIFormId.UI_OptionMenu,baseParams);
            //TODO 传入对应参数

            error = null;
            return ActionStatus.WaitMenuOption;
        }
    }
}