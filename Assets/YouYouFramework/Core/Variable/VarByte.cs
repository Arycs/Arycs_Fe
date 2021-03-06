using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace YouYou
{
    /// <summary>
    /// byte变量
    /// </summary>
    public class VarByte : Variable<byte>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarByte Alloc()
        {
            VarByte var = GameEntry.Pool.DequeueVarObject<VarByte>();  //要从对象池获取
            var.Value = 0; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc 在同步情况下 与Release 是成对出现的 ,Alloc 要最开始生命,Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarByte Alloc(byte value)
        {
            VarByte var = Alloc();
            var.Value = value;
            return var;
        }
        
        /// <summary>
        /// 重写运算符 VarByte -> byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator byte(VarByte value)
        {
            return value.Value;
        }
    }
}