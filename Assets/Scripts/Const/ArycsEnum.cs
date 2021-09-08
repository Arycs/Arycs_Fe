using System;

namespace Arycs_Fe.Maps
{
   /// <summary>
   /// 地形类型
   /// </summary>
   public enum TerrainType : byte
   {
      /// <summary>
      /// 平地
      /// </summary>
      Plain,
      
      /// <summary>
      /// 湿地(沼泽)
      /// </summary>
      [Obsolete("Game Not Use", true)] 
      Swamp,
      
      /// <summary>
      /// 道路
      /// </summary>
      Road,
      
      MaxLength
   }

   /// <summary>
   /// 地图类型
   /// </summary>
   public enum MapObjectType
   {
      MouseCursor,
      /// <summary>
      /// 光标
      /// </summary>
      Cursor,
      /// <summary>
      /// 障碍物
      /// </summary>
      Obstacle,
      /// <summary>
      /// 角色
      /// </summary>
      Class
   }

   /// <summary>
   /// 光标类型
   /// </summary>
   public enum CursorType : int
   {
      Mouse = 0,
      Move = 1,
      Attack = 2,
   }
   
   /// <summary>
   /// 移动方向
   /// </summary>
   [Serializable]
   [Flags]
   public enum Direction : byte
   {
      Down = 0x01,
      Right = 0x02,
      Up = 0x04,
      Left = 0x08,
      Cross = Down | Right | Up | Left
   }
}

namespace Arycs_Fe.Models
{
   [Serializable]
   public enum WeaponType : int
   {
      /// <summary>
      /// 剑
      /// </summary>
      Sword = 0,

      /// <summary>
      /// 枪
      /// </summary>
      [Obsolete("No animation", true)]
      Lance = 1,

      /// <summary>
      /// 斧
      /// </summary>
      [Obsolete("No animation", true)]
      Axe = 2,

      /// <summary>
      /// 弓
      /// </summary>
      [Obsolete("No animation", true)]
      Bow = 3,

      /// <summary>
      /// 杖
      /// </summary>
      [Obsolete("No animation", true)]
      Staff = 4,
   }
}