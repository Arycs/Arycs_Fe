using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本(脚本)，格式为Txt, 如果想解析其他类型脚本，则可以继续继承 Iscenario 并自行编写解析方法
    /// </summary>
    public class TxtScript : Iscenario
    {
        
        #region Conste/Static 定义好那些字符是特殊字符，遇到会进行特殊处理，比如 ; 代表命令分隔符，后续不够会进行添加

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
        protected class Command : IScenarioContent
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
            public int lineNo => m_LineNo;

            /// <summary>
            /// 类型
            /// </summary>
            public ScenarioContentType type => m_Type;

            /// <summary>
            /// 参数数量
            /// </summary>
            public int length => m_Arguments.Length;

            /// <summary>
            /// 索引器
            /// </summary>
            /// <param name="index"></param>
            public string this[int index] => m_Arguments[index];

            /// <summary>
            /// 关键字或剧情标识
            /// </summary>
            public string code => m_Arguments[0];

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

        /// <summary>
        /// 存储当前 脚本所有命令的列表
        /// </summary>
        private readonly List<Command> m_Commands = new List<Command>();

        #endregion

        #region Properties

        /// <summary>
        /// 剧本名(可能为null)
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// 剧本的原始副本
        /// </summary>
        public string buffer { get; private set; }

        /// <summary>
        /// 用作剧本标识的符号
        /// </summary>
        public string flagMark { get; set; } = k_DefaultFlagMark;

        /// <summary>
        /// 注释
        /// </summary>
        public string commentingPrefix { get; set; } = k_CommentingPrefix;

        /// <summary>
        /// 错误
        /// </summary>
        public string formatError { get; private set; } = string.Empty;

        /// <summary>
        /// 是否读取过剧本文本
        /// </summary>
        public bool isLoaded => !string.IsNullOrEmpty(buffer);

        /// <summary>
        /// 内容 (动作)
        /// </summary>
        protected List<Command> commands => m_Commands;

        /// <summary>
        /// 命令数量
        /// </summary>
        public int contentCount => m_Commands.Count;

        /// <summary>
        /// 获取剧本的一条命令
        /// </summary>
        /// <param name="index">命令索引</param>
        /// <returns></returns>
        public IScenarioContent GetContent(int index)
        {
            return m_Commands[index];
        }

        #endregion

        /// <summary>
        /// 重写 ToString ，如果加载过了，则返回buffer
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="flagMark">初始化剧情标识</param>
        /// <param name="commentingPrefix">初始化注释标识</param>
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
                this.flagMark = flagMark;
            }

            if (!string.IsNullOrEmpty(commentingPrefix))
            {
                this.commentingPrefix = commentingPrefix;
            }
        }

        #endregion

        public bool Load(string fileName, string scriptText)
        {
            // Regex.Unescape 转换输入字符串中的任意转义字符
            string script = Regex.Unescape(scriptText).Trim();

            if (string.IsNullOrEmpty(script))
            {
                //formatError = "TxtScript Load -> 'ScriptText' is null or empty";
                formatError = $"剧本加载 -> {fileName} 剧情文本为空";
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

        /// <summary>
        /// 解析脚本到命令
        /// </summary>
        /// <param name="script">文本内容</param>
        /// <returns></returns>
        protected virtual bool FormatScriptCommands(string script)
        {
            //1.以[";"] 分割文本，并删除空白
            string[] commandTexts = script.Split(new[] {k_CommandSeparator},
                StringSplitOptions.RemoveEmptyEntries);
            //2.在生成命令之前，我们来准备命令分隔符
            //分割剧本每个动作的分隔符：[" ","\t","\n"]
            string[] separators = {k_Space, k_Separator};
            string[] newLineSeparator = {k_NewLine};
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
            }
            return true;
        }

        /// <summary>
        /// 格式化命令
        /// </summary>
        /// <param name="index">索引，不是行号，是第N条命令的索引</param>
        /// <param name="commandText">命令的文本</param>
        /// <param name="separators">每行命令的分割符，一般为 "\t"," " 来分割当前行内容</param>
        /// <param name="newLineSeparator">根据换行符来进行 区分不同行</param>
        /// <param name="command">返回的命令</param>
        /// <returns></returns>
        protected virtual FormatContentResult FormatCommand(
            int index,
            string commandText,
            string[] separators,
            string[] newLineSeparator,
            out Command command)
        {
            ScenarioContentType type = ScenarioContentType.Action;
            List<string> arguments = new List<string>();

            // 1. 按["\n"]分割每一条内容
            string[] lines = commandText.Split(newLineSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string t1 in lines)
            {
                string line = t1.Trim();

                //删除每行注释
                int commentingIndex = line.IndexOf(commentingPrefix, StringComparison.Ordinal);
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
                foreach (string t in lineValues)
                {
                    string value = t.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        arguments.Add(value);
                    }
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
                //formatError = string.Format("TxtScript FormatError -> syntactic error: {0}", commandText);
                formatError = $"剧本错误 -> 语法错误: {commandText}";
                return FormatContentResult.Failure;
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