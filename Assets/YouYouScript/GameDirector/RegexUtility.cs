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
        
        // 匹配 以a-z A-Z 中文字符 开始，以任意结尾的字符串
        private const string k_Variable = @"^[a-zA-Z_\u4e00-\u9fa5)][\w]*$";

        // 匹配数字
        private const string k_Number = @"[^a-zA-Z_\u4e00-\u9fa5)]+";
        
        /// <summary>
        /// 匹配 以a-z A-Z 中文字符 开始，以任意结尾的字符串
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static bool IsMatchVariable(string variable)
        {
            return Regex.IsMatch(variable, k_Variable);
        }

        public static int GetRichTextFormatString(string text, out Dictionary<int, int> richLengthDict, out Dictionary<int, KeyValuePair<string, string>> richTextDict)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 获取匹配的第一个数字
        /// </summary>
        /// <param name="varNum"></param>
        /// <param name="outNum"></param>
        public static void IsMatchNumber(string varNum, out int outNum)
        {
           Match temp = Regex.Match(varNum, k_Number);
           outNum = temp.Value.ToInt();
        }
    }
}