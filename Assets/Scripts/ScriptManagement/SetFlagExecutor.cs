using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class SetFlagExecutor : ScenarioContentExecutor<SetFlagExecutor.SetFlagArgs>
    {
        public struct SetFlagArgs
        {
            public string flag;
        }

        public override string code
        {
            get { return string.Empty; }
        }
        
        //TODO ParseArgs and Run
        public override bool ParseArgs(IScenarioContent content, ref SetFlagArgs args, out string error)
        {
            // #flag0
            //剧情标识符只能有一个参数
            if (content.length != 1)
            {
                error = GetLengthErrorString(1);
                return false;
            }

            args.flag = content.code;
            error = null;
            return true;
        }

        protected override ScenarioActionStatus Run(IGameAction gameAction, IScenarioContent content, SetFlagArgs args, out string error)
        {
            ScenarioAction action;
            if (!ParseAction<ScenarioAction>(gameAction,out action,out error))
            {
                return ScenarioActionStatus.Error;
            }

            return action.SetFlagCommand(args.flag, out error);
        }
    }
}