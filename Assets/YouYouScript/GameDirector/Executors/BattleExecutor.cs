using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace Arycs_Fe.ScriptManagement
{
    public class BattleExecutor : ScenarioContentExecutor<BattleExecutor.BattleArgs>
    {
        public struct BattleArgs
        {
            public string sceneName;
            public string scriptName;
        }

        public override string code
        {
            get { return "battle"; }
        }
        public override bool ParseArgs(IScenarioContent content, ref BattleArgs args, out string error)
        {
            //标准格式 battle Stage0Scene Stage0Script
            if (content.length != 3)
            {
                error = GetLengthErrorString();
                return false;
            }

            args.sceneName = content[1];
            args.scriptName = content[2];
            
            error = null;
            return true;
        }
        
        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, BattleArgs args, out string error)
        {
            error = null;
            
            //保存地图场景与脚本名 
            ScenarioBlackboard.battleMapScene = args.sceneName;
            ScenarioBlackboard.mapScript = args.scriptName;
            
            // 读取场景
            if (!GameEntry.GameDirector.LoadMap(args.sceneName))
            {
                return ActionStatus.Error;
            }
            
            //使用场景名称初始化地图状态， 0为战斗失败，1为战斗胜利
            ScenarioBlackboard.Set(args.sceneName,0);
            return ActionStatus.WaitMapDone;
        }
    }
}