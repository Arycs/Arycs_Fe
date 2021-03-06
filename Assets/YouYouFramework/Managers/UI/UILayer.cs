using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouYou
{
   /// <summary>
   /// UI层级管理
   /// </summary>
   public class UILayer
   {
      private Dictionary<byte, ushort> m_UILayerDic;

      public UILayer()
      {
         m_UILayerDic = new Dictionary<byte, ushort>();
      }
      
      /// <summary>
      /// 初始化基础排序
      /// </summary>
      /// <param name="groups"></param>
      internal void Init(UIGroup[] groups)
      {
         int len = groups.Length;
         for (int i = 0; i < len; i++)
         {
            UIGroup group = groups[i];
            m_UILayerDic[group.Id] = group.BaseOrder;
         }
      }

      /// <summary>
      /// 设置层级
      /// </summary>
      /// <param name="formBase"></param>
      /// <param name="isAdd"></param>
      internal void SetSortingOrder(UIFormBase formBase,bool isAdd)
      {
         if (isAdd)
         {
            m_UILayerDic[formBase.GroupId] += 10;
         }
         else
         {
            m_UILayerDic[formBase.GroupId] -= 10;
         }

         formBase.currCanvas.sortingOrder = m_UILayerDic[formBase.GroupId];
      }
   }
}