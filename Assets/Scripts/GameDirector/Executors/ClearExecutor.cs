using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class ClearExecutor : ScenarioContentExecutor<ClearExecutor.ClearArgs>
    {
        public struct ClearArgs
        {
            public string type;
            public string arg0;
            public string arg1;
            public string arg2;
            public string arg3;
        }

        public override string code
        {
            get { return "clear"; }
        }

        protected static readonly HashSet<String> s_DefaultSupportedTypes = new HashSet<string>()
        {
            "text"
        };

        protected virtual HashSet<string> GetSupportedTypes()
        {
            return s_DefaultSupportedTypes;
        }

        public override bool ParseArgs(IScenarioContent content, ref ClearArgs args, out string error)
        {
            if (content.length < 2)
            {
                error = GetLengthErrorString();
                return false;
            }

            args.type = content[1].ToLower();
            if (!GetSupportedTypes().Contains(args.type))
            {
                error = $"{typeName} ParseArgs -> the type '{args.type}' is not supported.";
                return false;
            }

            int length = content.length;
            int index = 2;
            if (index < length)
            {
                args.arg0 = content[index++];
            }

            if (index < length)
            {
                args.arg1 = content[index++];
            }

            if (index < length)
            {
                args.arg2 = content[index++];
            }

            if (index < length)
            {
                args.arg3 = content[index++];
            }

            return CheckArgsCorrect(content, ref args, out error);
        }

        private bool CheckArgsCorrect(IScenarioContent content, ref ClearArgs args, out string error)
        {
            switch (args.type)
            {
                case "text":
                    if (!string.IsNullOrEmpty(args.arg0))
                    {
                        args.arg0 = args.arg0.ToLower();
                        if (args.arg0 != "top" && args.arg0!= "bottom" && args.arg0 != "global")
                        {
                            error =
                                $"{typeName} ParseArgs error : position must be empty or one of [top, bottom, global].";
                            return false;
                        }
                    }

                    break;
                //TODO Other
                default:
                    break;
            }

            error = null;
            return true;
        }


        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, ClearArgs args, out string error)
        {
            switch (args.type)
            {
                case "text":
                    return ClearTextCmd(gameAction, args, out error);
                //TODO Other
                default:
                    error = $"{typeName} Run -> UnEspected error! the type '{args.type}' is not supported";
                    return ActionStatus.Error;
            }
        }

        private ActionStatus ClearTextCmd(IGameAction gameAction, ClearArgs args, out string error)
        {
            error = null;
            //TODO 判断界面是否打开

            //TODO 如果位置为空，默认全部关闭。
            string position = args.arg0;
            
            //TODO 否则关闭对应位置的窗口
            
            return ActionStatus.NextFrame;;
        }
    }
}