using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Models
{
    [Serializable]
    public class ItemData : RuntimeData<ItemData>
    {
        public ulong guid;

        public int itemId;

        public Item.OwnerType ownerType;

        public int ownerId;
        
        /// <summary>
        /// 耐久度，物品数量或使用次数
        /// </summary>
        public int durability;

        public override ItemData Clone()
        {
            ItemData data = new ItemData()
            {
                itemId = itemId,
                durability = durability
            };
            return data;
        }

        public override void CopyTo(ItemData data)
        {
            if (data == null)
            {
                Debug.LogError("RuntimeData -> CopyTo : data is null");
                return;
            }

            if (data == this)
            {
                return;
            }

            data.itemId = itemId;
            data.durability = durability;
        }
    }
}