using System;

namespace YouYou
{
    /// <summary>
    /// Http 请求的回调数据
    /// </summary>
    public class HttpCallBackArgs : EventArgs
    {
        /// <summary>
        /// 是否有错 
        /// </summary>
        public bool HasError;

        /// <summary>
        /// 返回值
        /// </summary>
        public string Value;

        /// <summary>
        /// 字节数组
        /// </summary>
        public byte[] Data;
    }
}