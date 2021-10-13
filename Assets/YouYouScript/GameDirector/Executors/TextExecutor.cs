using System.Collections;
using System.Collections.Generic;
using System.Text;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using YouYou;

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
            StringBuilder builder = new StringBuilder();

            int index = 2;
            while (index < content.length)
            {
                string line;
                if (content[index].StartsWith("\""))
                {
                    if (!ScenarioUtility.ParseContentString(content,ref index,out line ,out error))
                    {
                        return false;
                    }
                }
                else
                {
                    // 可能是个变量
                    int id = -1;
                    if (!ParseOrGetVarValue(content[index],ref id ,out error))
                    {
                        return false;
                    }
                    //TODO  这里根据ID 获取语言包
                    line = "AAA";
                    if (string.IsNullOrEmpty(line))
                    {
                        error = $"{typeName} ParseArgs error : text id '{content[index]}' was not fount";
                        return false;
                    }

                    index++;
                }

                builder.AppendLine(line);
            }

            args.text = builder.ToString();
            
            //从游戏设置中读取
            // 最常见的就是类似J-AVG快进的形式
            args.async = true;
            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, TextArgs args, out string error)
        {
            BaseParams baseParams = GameEntry.Pool.DequeueClassObject<BaseParams>();
            baseParams.Reset();
            baseParams.StringParam1 = args.position;
            baseParams.StringParam2 = args.text;
            baseParams.BoolParam1 = args.async;
            GameEntry.UI.OpenUIForm(UIFormId.UI_Talk,baseParams);
            error = null;
            // GameEntry.Event.CommonEvent.AddEventListener(SysEventId.UITalkWriteDown,((ScenarioAction) gameAction).WriteTextDone);
            
            
            //如果是快进模式，要等待一帧，防止看不到界面，闪屏都没有
            return args.async ? ActionStatus.WaitWriteTextDone : ActionStatus.NextFrame;
        }
    }
}