using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arycs_Fe.Models;
using UnityEngine;
using YouYou;

public class ItemDataManager 
{
   /// <summary>
   /// 生成的下一个物品的GUID
   /// </summary>
   public static ulong nextItemGUID { get; private set; }

   private Dictionary<ulong, Item> m_Items;
   private Dictionary<int, ItemData> m_ItemTempLates;

   protected void OnLoad()
   {
      nextItemGUID = 1UL;
      m_Items = new Dictionary<ulong, Item>();
      m_ItemTempLates = new Dictionary<int, ItemData>();
   }

   /// <summary>
   /// 获取物品信息
   /// </summary>
   /// <param name="itemId"></param>
   /// <returns></returns>
   private Sys_ItemEntity GetItemInfo(int itemId)
   {
      Sys_ItemEntity entity = GameEntry.DataTable.Sys_ItemDBModel.Get(itemId);
      if (entity == null)
      {
         Debug.LogErrorFormat("ItemModel -> ItemInfo key '{0}' is not fount", itemId.ToString());
         return null;
      }

      return entity;
   }

   /// <summary>
   /// 获取物品数据模板，如果不存在，则创建后获取
   /// </summary>
   /// <param name="itemId"></param>
   /// <returns></returns>
   public ItemData GetOrCreateItemTemplate(int itemId)
   {
      ItemData data;
      if (!m_ItemTempLates.TryGetValue(itemId,out data))
      {
         Sys_ItemEntity entity = GetItemInfo(itemId);
         if (entity == null)
         {
            return null;
         }

         data = new ItemData()
         {
            itemId = entity.ItemId
         };

         switch (entity.ItemType)
         {
            case ItemType.Weapon:
               data.durability = entity.Durability;
               break;
            case ItemType.Ornament:
               break;
            case ItemType.Consumable:
               data.durability = entity.StackingNumber == 1 ? entity.AmountUsed : entity.StackingNumber;
               break;
            default:
               break;
         }

         m_ItemTempLates.Add(itemId, data);
      }

      return data;
   }

   /// <summary>
   /// 创建物品
   /// </summary>
   /// <param name="info"></param>
   /// <param name="data"></param>
   /// <param name="isTemplate"></param>
   /// <returns></returns>
   private Item CreateItem(Sys_ItemEntity info, ItemData data, bool isTemplate)
   {
      Item item;
      switch (info.ItemType)
      {
         case ItemType.Weapon:
            item = new Weapon(info, isTemplate ? nextItemGUID++ : data.guid);
            break;
         case ItemType.Ornament:
            item = new Consumable(info, isTemplate ? nextItemGUID++ : data.guid);
            break;
         case ItemType.Consumable:
            item = new Consumable(info, isTemplate ? nextItemGUID++ : data.guid);
            break;
         default:
            Debug.LogError("ItemDataManager -> Create item : unKnow type");
            item = null;
            break;
      }

      if (item != null)
      {
         item.Load(data);
      }

      return item;
   }
   
   /// <summary>
   /// 创建物品
   /// </summary>
   public Item CreateItem(int itemId)
   {
      Sys_ItemEntity entity = GetItemInfo(itemId);
      if (entity == null)
      {
         return null;
      }

      ItemData template = GetOrCreateItemTemplate(itemId);
      if (template == null)
      {
         return null;
      }

      Item item = CreateItem(entity, template, true);
      m_Items.Add(item.guid,item);
      return item;
   }

   protected void OnDispose()
   {
      ulong[] keys = m_Items.Keys.ToArray();
      for (int i = 0; i < keys.Length; i++)
      {
         m_Items[keys[i]].Dispose();
      }

      m_Items = null;
      m_ItemTempLates = null;
   }
}
