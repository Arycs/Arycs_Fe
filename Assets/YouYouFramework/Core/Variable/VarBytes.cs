using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace YouYou
{
    /// <summary>
    /// byte[]变量
    /// </summary>
    public class VarBytes : Variable<byte[]>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarBytes Alloc()
        {
            VarBytes var = GameEntry.Pool.DequeueVarObject<VarBytes>();  //要从对象池获取
            var.Value = null; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc 在同步情况下 与Release 是成对出现的 ,Alloc 要最开始生命,Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarBytes Alloc(byte[] value)
        {
            VarBytes var = Alloc();
            var.Value = value;
            return var;
        }
        
        /// <summary>
        /// 重写运算符 VarBytes -> byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator byte[](VarBytes value)
        {
            return value.Value;
        }
    }
}