using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class TextExecutor : ScenarioContentExecutor<TextExecutor.TextArgs>
    {
        public struct TextArgs
        {
            public string position;
            public string text;
            public bool async;
        }

        public override string code
        {
            get { return "text"; }
        }
        public override bool ParseArgs(IScenarioContent content, ref TextArgs args, out string error)
        {
            //标准格式 text top text1 text2 
            if (content.length < 3 )
            {
                error = GetLengthErrorString();   
                return false;
            }
            // 处理位置
            string position = content[1].ToLower();
            if (position != "top" && position !="bottom" && position != "global")
            {
                // error = $"{typeName} ParseArgs error : position must be one of [top,bottom,global]";
                error = $"{typeName} 参数错误 : 位置必须为 [top,bottom,global]之一";
                return false;
            }

            args.position = position;

            args.text = "AAA";
            args.async = true;
            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, TextArgs args, out string error)
        {
            throw new System.NotImplementedException();
        }
    }
}