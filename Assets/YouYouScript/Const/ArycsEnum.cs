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

namespace Arycs_Fe.ScriptManagement
{
   public enum MenuTextID
   {
      //Main 
      Unit = 100,
      Item = 101,
      Data = 102,
      Skill = 103,
      Config = 104,
      Save = 105,
      TurnEnd = 106,
      
      //Common
      Close = 110,
      
      //Player
      Move = 120,
      Holding = 121,
      Talk = 122,
      Attack = 123,
      Status = 124,
   }
}
