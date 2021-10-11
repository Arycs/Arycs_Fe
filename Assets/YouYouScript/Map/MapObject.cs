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
            set
            {
                m_CellPosition = value;
                if (renderer != null)
                {
                    renderer.sortingOrder = MapObject.CalcSortingOrder(map, value);
                }
            }
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
        /// <param name="world"></param>
        /// <param name="center"></param>
        public void UpdatePosition(bool world = true, bool center = false)
        {
            if (map == null)
            {
                Debug.LogError(name + " Map is null");
                return;
            }

            Vector3 pos = map.GetCellPosition(cellPosition, world, center);
            if (world)
            {
                transform.position = pos;
            }
            else
            {
                transform.localPosition = pos;
            }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <param name="world"></param>
        /// <param name="center"></param>
        public void UpdatePosition(Vector3Int cellPosition ,bool world = true, bool center = false)
        {
            this.cellPosition = cellPosition;
            UpdatePosition(world, center);
        }

        #endregion

        /// <summary>
        /// 计算sortingOrder，防止遮挡
        /// </summary>
        /// <param name="map"></param>
        /// <param name="cellPosition"></param>
        /// <returns></returns>
        public static int CalcSortingOrder(MapGraph map, Vector3Int cellPosition)
        {
            if (map == null)
            {
                return 0;
            }
            
            //相对零点坐标
            Vector3Int relative = cellPosition - map.leftDownPosition;
            // 计算从右到左，从下到上渲染顺序， (sortingOrder越大越后渲染)
            // 前y行的格子总数 = map.width * relative.y
            // 当前行(第 y+1 行) 从右向左的格子数 = map.width - relative.x
            // 这样计算后是递增， 范围[1, map.width * map.height]
            // 加上负号后 是递减，范围[-(map.width * map.height), -1]
            // 举例 ： 地图尺寸 20 * 20
            // 右下角相对坐标，第20列第1行：
            //          cellPosition = (19,0,0), sortingOrder = -(20 * 0 + (20 - 19)) = -1
            // 左上角相对坐标, 第1列第10行：
            //          cellPosition = (0,9,0), sortingOrder = -(20 * 9 + (20 - 0)) = -200
            //测试相对坐标 第12列第6行：
            //          cellPosition = (11,5,0), sortingOrder = -(20 * 5 + (20 - 11)) = -109
            return -(map.width * relative.y + (map.width - relative.x));
        }
    }
}