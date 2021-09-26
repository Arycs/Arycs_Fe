using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    /// <summary>
    /// 格子状态
    /// </summary>
    public enum CellStatus : byte
    {
        /// <summary>
        /// 没有任何东西， 0000 0000
        /// </summary>
        None = 0,
        
        /// <summary>
        /// 有TerrainTile, 0000 0001
        /// </summary>
        TerrainTile = 0x01,
        
        /// <summary>
        /// 移动光标， 0000 0010
        /// </summary>
        MoveCursor = 0x02,
        
        /// <summary>
        /// 攻击光标， 0000 0100
        /// </summary>
        AttackCursor = 0x04,
        
        /// <summary>
        /// 地图对象， 0000 1000
        /// </summary>
        MapObject = 0x08,
        
        // 如果有其他需求，在这里添加其余4个开关属性， 使用二进制的每一位来记录，十分的巧妙，接下来的是
        //  0x16 0x32 0x64 0x128
        // 8个 数位分别代表对应的位置， 即0000 1010 则说明 有地图对象，并且有移动光标在此处
        
        /// <summary>
        /// 全部8个开关，1111 1111
        /// </summary>
        All = byte.MaxValue,
    }
}