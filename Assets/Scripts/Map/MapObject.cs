using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    //TODO 该对象应该使用对象池来进行实例化与收回
    [DisallowMultipleComponent]
    public abstract class MapObject : MonoBehaviour
    {
        #region Field

        [SerializeField] private SpriteRenderer m_Render;

        private MapGraph m_Map;
        private Vector3Int m_CellPosition;
        #endregion

        #region Property

        public new SpriteRenderer renderer
        {
            get { return m_Render; }
            set { m_Render = value; }
        }

        /// <summary>
        /// 地图网络中的位置
        /// </summary>
        public Vector3Int cellPosition
        {
            get { return m_CellPosition; }
        }

        /// <summary>
        /// 所属地图
        /// </summary>
        public MapGraph map
        {
            get { return m_Map; }
            internal set { m_Map = value; }
        }

        /// <summary>
        /// 地图对象类型
        /// </summary>
        public abstract MapObjectType mapObjectType { get; }
        #endregion

        #region Method

        public void InitMapObject(MapGraph map)
        {
            m_Map = map;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <param name="world"></param>
        /// <param name="center"></param>
        public void UpdatePosition(Vector3Int cellPosition, bool world = true, bool center = false)
        {
            if (m_Map == null)
            {
                Debug.LogError(name + "Map is null.");
                return;
            }

            Vector3 pos = m_Map.GetCellPosition(cellPosition, world, center);
            if (world)
            {
                transform.position = pos;
            }
            else
            {
                transform.localPosition = pos;
            }

            m_CellPosition = cellPosition;
        }
        #endregion
    }
}