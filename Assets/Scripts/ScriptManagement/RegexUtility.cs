using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 正则表达式帮助类
    /// </summary>
    public class RegexUtility
    {
        //正则表达式
        // ^ 和 $ 开始与结束的符号
        // [] : 需要匹配单字的内容
        // * :将前一个匹配内容重复零到多次，这里是[\w]
        // \w : 包含下划线的所有单字
        // a-z,A-Z : 小/大写字母
        // _ : 下划线
        // \u4e00 - \u9fa5 : 中文Unicode字符开始与结束
        public const string k_Variable = @"^[a-zA-Z_\u4e00-\u9fa5)][\w]*$";

        public static bool IsMatchVariable(string variable)
        {
            return Regex.IsMatch(variable, k_Variable);
        }

        

        
    }
}