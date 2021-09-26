using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    [AddComponentMenu("SRPG/Map Object/Map Mouse Cursor")]
    public class MapMouseCursor : MapCursor
    {
        public override MapObjectType mapObjectType
        {
            get { return MapObjectType.MouseCursor; }
        }

        public override CursorType cursorType
        {
            set
            {
                if (value != CursorType.Mouse)
                {
                    return;
                }

                base.cursorType = value;
            }
        }

        public void OnSpawn()
        {
            Debug.LogError("MapMouseCursor 使用框架对象池内容处理。");
        }

        public void DisPlayCursor(bool show)
        {
            if (renderer != null)
            {
                renderer.enabled = show;
            }
        }
    }
}