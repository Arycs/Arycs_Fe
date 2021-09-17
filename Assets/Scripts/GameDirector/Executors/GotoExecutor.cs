using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class GotoExecutor : ScenarioContentExecutor<GotoExecutor.GotoArgs>
    {
        public struct GotoArgs
        {
            public string flag;
        }

        public override string code
        {
            get { return "goto"; }
        }
        public override bool ParseArgs(IScenarioContent content, ref GotoArgs args, out string error)
        {
            //goto #flag
            if (content.length != 2)
            {
                error = GetLengthErrorString(2);
                return false;
            }

            args.flag = content[1];
            error = null;
            return true;
        }

        protected override ScenarioActionStatus Run(IGameAction gameAction, IScenarioContent content, GotoArgs args, out string error)
        {
            ScenarioAction action;
            if (!ParseAction<ScenarioAction>(gameAction,out action,out error))
            {
                return ScenarioActionStatus.Error;
            }

            return action.GotoCommand(args.flag, out error);
        }
    }
}