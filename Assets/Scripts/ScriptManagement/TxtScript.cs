using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本(脚本)
    /// </summary>
    public class TxtScript : Iscenario
    {
        #region Conste/Static

        /// <summary>
        /// 用于命令的分隔符
        /// </summary>
        public const string k_CommandSeparator = ";";

        /// <summary>
        /// 空格
        /// </summary>
        public const string k_Space = " ";

        /// <summary>
        /// 分隔符
        /// </summary>
        public const string k_Separator = "\t";

        /// <summary>
        /// 换行符
        /// </summary>
        public const string k_NewLine = "\n";

        /// <summary>
        /// 注释前缀
        /// </summary>
        public const string k_CommentingPrefix = "//";

        /// <summary>
        /// 默认剧本标识前缀
        /// </summary>
        public const string k_DefaultFlagMark = "#";

        #endregion

        /// <summary>
        /// 剧本的一条命令
        /// </summary>
        public class Command : IScenarioContent
        {
            //TODO 命令

            #region Fields

            private readonly int m_LineNo;
            private readonly ScenarioContentType m_Type;
            private readonly string[] m_Arguments;

            #endregion

            #region Properties

            /// <summary>
            /// 行号，若命令在脚本中是多行，这里指最后一行的行号
            /// </summary>
            public int lineNo
            {
                get { return m_LineNo; }
            }

            /// <summary>
            /// 类型
            /// </summary>
            public ScenarioContentType type
            {
                get { return m_Type; }
            }

            /// <summary>
            /// 参数数量
            /// </summary>
            public int length
            {
                get { return m_Arguments.Length; }
            }

            /// <summary>
            /// 索引器
            /// </summary>
            /// <param name="index"></param>
            public string this[int index]
            {
                get { return m_Arguments[index]; }
            }

            /// <summary>
            /// 关键字或剧情标识
            /// </summary>
            public string code
            {
                get { return m_Arguments[0]; }
            }

            #endregion

            #region Constructor

            public Command(ScenarioContentType type, string[] arguments)
            {
                m_Type = type;
                m_Arguments = arguments;
            }

            public Command(int lineNo, ScenarioContentType type, string[] arguments) : this(type, arguments)
            {
                m_LineNo = lineNo;
            }

            #endregion

            public override string ToString()
            {
                return string.Join(k_Space, m_Arguments) + k_CommandSeparator;
            }
        }

        #region Fields

        private string m_Name;
        private string m_Buffer;
        private string m_FlagMark = k_DefaultFlagMark;
        private string m_CommentingPrefix = k_CommentingPrefix;
        private string m_Error = string.Empty;
        private readonly List<Command> m_Commands = new List<Command>();

        #endregion

        #region Properties

        /// <summary>
        /// 剧本名(可能为null)
        /// </summary>
        public string name
        {
            get { return m_Name; }
            private set { m_Name = value; }
        }

        /// <summary>
        /// 剧本的原始副本
        /// </summary>
        public string buffer
        {
            get { return m_Buffer; }
            private set { m_Buffer = value; }
        }

        /// <summary>
        /// 用作剧本标识的符号
        /// </summary>
        public string flagMark
        {
            get { return m_FlagMark; }
            set { m_FlagMark = value; }
        }

        /// <summary>
        /// 注释
        /// </summary>
        public string commentingPrefix
        {
            get { return m_CommentingPrefix; }
            set { m_CommentingPrefix = value; }
        }

        /// <summary>
        /// 错误
        /// </summary>
        public string formatError
        {
            get { return m_Error; }
            protected set { m_Error = value; }
        }

        /// <summary>
        /// 是否读取过剧本文本
        /// </summary>
        public bool isLoaded
        {
            get { return !string.IsNullOrEmpty(m_Buffer); }
        }

        /// <summary>
        /// 内容 (动作)
        /// </summary>
        protected List<Command> commands
        {
            get { return m_Commands; }
        }

        /// <summary>
        /// 命令数量
        /// </summary>
        public int contentCount
        {
            get { return m_Commands.Count; }
        }

        public IScenarioContent GetContent(int index)
        {
            return m_Commands[index];
        }

        #endregion

        public override string ToString()
        {
            if (!isLoaded)
            {
                return base.ToString();
            }

            return buffer;
        }

        #region Constructor

        public TxtScript()
        {
        }

        public TxtScript(string flagMark, string commentingPrefix)
        {
            //防止flagMark有空格
            //它可以有特殊字符，但不推荐含有特殊符号
            //可以使用Trim() 去除两边的特殊字符
            //或者使用Regex.Replace(flagMark,@"\s","") 去除所有特殊字符
            // \s 是正则表达式的匹配夫，包含任何空白的字符(["","\f","\n","\t","\v"...)
            //这些限定不是必须的，也许你就喜欢有空格也说不定
            //在你创建语言规则前应该考虑这些
            if (flagMark != null)
            {
                flagMark = flagMark.Replace(" ", "");
            }

            //防止commentingPrefix有空格
            if (commentingPrefix != null)
            {
                commentingPrefix = commentingPrefix.Replace(" ", "");
            }

            if (!string.IsNullOrEmpty(flagMark))
            {
                m_FlagMark = flagMark;
            }

            if (!string.IsNullOrEmpty(commentingPrefix))
            {
                m_CommentingPrefix = commentingPrefix;
            }
        }

        #endregion

        public bool Load(string fileName, string scriptText)
        {
            string script = Regex.Unescape(scriptText).Trim();

            if (string.IsNullOrEmpty(script))
            {
                formatError = "TxtScript Load -> 'ScriptText' is null or empty";
                return false;
            }

            name = string.Empty;
            buffer = string.Empty;
            formatError = null;
            commands.Clear();

            bool loaded = FormatScriptCommands(script);
            if (loaded)
            {
                name = fileName;
                buffer = script;
            }

            return loaded;
        }

        protected virtual bool FormatScriptCommands(string script)
        {
            //1.以[";"] 分割文本，并删除空白
            string[] commandTexts = script.Split(new string[] {k_CommandSeparator},
                StringSplitOptions.RemoveEmptyEntries);
            //2.在生成命令之前，我们来准备命令分隔符
            //分割剧本每个动作的分隔符：[" ","\t","\n"]
            string[] separators = new string[] {k_Space, k_Separator};
            string[] newLineSeparator = new string[] {k_NewLine};
            for (int i = 0; i < commandTexts.Length; i++)
            {
                //删除左右空格和左右各种特殊转义符
                string commandText = commandTexts[i].Trim();

                //如果为空 下一个动作
                if (string.IsNullOrEmpty(commandText))
                {
                    continue;
                }

                //格式化每一次动作，生成命令
                Command command;
                FormatContentResult formatResult = FormatCommand(
                    i, //不是行号，是下标
                    commandText,
                    separators,
                    newLineSeparator,
                    out command);
                //成功添加
                if (formatResult == FormatContentResult.Succeed)
                {
                    commands.Add(command);
                }
                //失败返回
                else if (formatResult == FormatContentResult.Failure)
                {
                    return false;
                }
                else
                {
                    continue;
                }
            }

            return true;
        }

        protected virtual FormatContentResult FormatCommand(
            int index,
            string commandText,
            string[] separators,
            string[] newLineSeparator,
            out Command command)
        {
            ScenarioContentType type = ScenarioContentType.Action;
            List<string> arguments = new List<string>();

            //TODO 具体实现
            // 1. 按["\n"]分割每一条内容
            string[] lines = commandText.Split(newLineSeparator, StringSplitOptions.RemoveEmptyEntries);
            for (int li = 0; li < lines.Length; li++)
            {
                string line = lines[li].Trim();

                //删除每行注释
                int commentingIndex = line.IndexOf(commentingPrefix);
                if (commentingIndex != -1)
                {
                    line = line.Substring(0, commentingIndex).TrimEnd();
                }

                //如果 每行为空，则下一行
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                //2. 按[" ", "\t"] 分割每行
                string[] lineValues = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                //如果是标识，如果arguments.Count 不是0，则不是第一个参数
                if (lineValues[0].StartsWith(flagMark) && arguments.Count == 0)
                {
                    type = ScenarioContentType.Flag;
                }

                //添加内容
                for (int vi = 0; vi < lineValues.Length; vi++)
                {
                    string value = lineValues[vi].Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        arguments.Add(value);
                    }
                }

                //只有注释
                if (arguments.Count == 0)
                {
                    command = null;
                    return FormatContentResult.Commenting;
                }

                //如果标识符参数大于1，则语法错误，检查语法
                if (type == ScenarioContentType.Flag && arguments.Count > 1)
                {
                    command = null;
                    formatError = string.Format("TxtScript FormatError -> syntactic error: {0}", commandText);
                    return FormatContentResult.Failure;
                }

                command = new Command(index, type, arguments.ToArray());
                return FormatContentResult.Succeed;
            }

            command = new Command(index, type, arguments.ToArray());
            return FormatContentResult.Succeed;
        }

        /// <summary>
        /// 重新建立文本
        /// if 'commandSeparator' == null : Environment.NewLine.
        /// 如果你需要每条命令贴在一起，传入String.Empty
        /// </summary>
        /// <param name="commandSeparator"></param>
        /// <returns></returns>
        public string RecreateText(string commandSeparator)
        {
            if (commandSeparator == null)
            {
                commandSeparator = Environment.NewLine;
            }

            string[] texts = commands.Select(cmd => cmd.ToString()).ToArray();
            return string.Join(commandSeparator, texts);
        }
    }
}