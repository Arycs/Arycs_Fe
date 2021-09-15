using System.Collections;
using System.Collections.Generic;
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

            public Command(int lineNo, ScenarioContentType type, string[] arguments):this(type,arguments)
            {
                m_LineNo = lineNo;
            }
            #endregion

            public override string ToString()
            {
                return string.Join(k_Space, m_Arguments) + k_CommandSeparator;
            }

        }
        
        //TODO 剧本
        public string name { get; }
        public string formatError { get; }
        public bool isLoaded { get; }
        public int contentCount { get; }
        public IScenarioContent GetContent(int index)
        {
            throw new System.NotImplementedException();
        }

        public bool Load(string fileName, string scriptText)
        {
            throw new System.NotImplementedException();
        }
    }
}