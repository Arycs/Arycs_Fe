using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouYou;

namespace YouYou
{
    /// <summary>
    /// int变量
    /// </summary>
    public class VarInt : Variable<int>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarInt Alloc()
        {
            VarInt var = GameEntry.Pool.DequeueVarObject<VarInt>();  //要从对象池获取
            var.Value = 0; //对其进行初始化,防止其他对象数据回池没清空
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象, Alloc 在同步情况下 与Release 是成对出现的 ,Alloc 要最开始生命,Release要在结束声明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VarInt Alloc(int value)
        {
            VarInt var = Alloc();
            var.Value = value;
            return var;
        }
        
        /// <summary>
        /// 重写运算符 VarInt -> int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator int(VarInt value)
        {
            return value.Value;
        }
    }
}
