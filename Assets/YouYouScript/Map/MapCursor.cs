using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    public class MapCursor : MapObject
    {
    
        /// <summary>
        /// 光标类型
        /// </summary>
        public enum CursorType : int
        {
            Mouse = 0,
            Move = 1,
            Attack = 2,
        }
        
    
        public Animator m_Animator;
        [SerializeField]
        public Sprite[] m_CursorSprites;

        public virtual CursorType cursorType
        {
            set
            {
                if (renderer == null)
                {
                    Debug.LogError("Cursor : SpriteRender was not found.");
                    return;
                }

                if (m_CursorSprites == null || m_CursorSprites.Length == 0)
                {
                    Debug.LogError("Cursor : there is no sprite");
                    return;
                }

                int index = (int) value;
                if (index < 0 || index >= m_CursorSprites.Length)
                {
                    Debug.LogError("Cursor : index is out of range");
                    return;
                }

                renderer.sprite = m_CursorSprites[index];
            }
        }

        public override MapObjectType mapObjectType
        {
            get { return MapObjectType.Cursor; }
        }

        //TODO 对象池相关方法，后续从框架上的对象池组件上进行重写调用
        public void OnSpawn()
        {
            if (renderer == null)
            {
                renderer = gameObject.GetComponentInChildren<SpriteRenderer>(true);
                if (renderer == null)
                {
                    Debug.LogError("Cursor : SpriteRenderer was not fount");
                    return;
                }
            }
        }
    }
}