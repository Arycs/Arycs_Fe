using System.Collections.Generic;
using Arycs_Fe.FindPath;
using Arycs_Fe.Models;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
using SceneView = UnityEditor.SceneView;

#endif

namespace Arycs_Fe.Maps
{
    [RequireComponent(typeof(Grid))]
    public class MapGraph : MonoBehaviour
    {
#if UNITY_EDITOR

        #region Gizmos

        [Header("Editor Gizmos")] public bool m_EditorDrawGizmos = true;
        public Color m_EditorBorderColor = Color.white;
        public Color m_EditorCellColor = Color.green;
        public Color m_EditorErrorColor = Color.red;

        private void OnDrawGizmos()
        {
            if (m_EditorDrawGizmos)
            {
                EditorDrawBorderGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_EditorDrawGizmos)
            {
                EditorDrawCellGizmos();
            }
        }

        /// <summary>
        /// 绘制Border的Gizmos
        /// </summary>
        protected void EditorDrawBorderGizmos()
        {
            Color old = Gizmos.color;

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = m_EditorBorderColor;

            // 获取边框左下角与右上角的世界坐标
            Vector3 leftDown = grid.GetCellCenterWorld(leftDownPosition) - halfCellSize;
            Vector3 rightUp = grid.GetCellCenterWorld(rightUpPosition) + halfCellSize;

            // 绘制左下角Cell与右上角Cell的Position
            Handles.Label(leftDown, (new Vector2Int(leftDownPosition.x, leftDownPosition.y)).ToString(), textStyle);
            Handles.Label(rightUp, (new Vector2Int(rightUpPosition.x, rightUpPosition.y)).ToString(), textStyle);

            if (mapRect.width > 0 && mapRect.height > 0)
            {
                Gizmos.color = m_EditorBorderColor;

                // 边框的长与宽
                Vector3 size = Vector3.Scale(new Vector3(width, height), grid.cellSize);

                // 边框的中心坐标
                Vector3 center = leftDown + size / 2f;

                // 绘制边框
                Gizmos.DrawWireCube(center, size);
            }

            Gizmos.color = old;
        }

        /// <summary>
        /// 绘制Cell的Gizmos
        /// </summary>
        protected void EditorDrawCellGizmos()
        {
            // 由于是在Scene场景，并且游戏没有运行，因此使用Event.current.mousePosition用于获取鼠标位置
            Event e = Event.current;
            if (e.type != EventType.Repaint)
            {
                return;
            }

            // 获取当前操作Scene面板
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null)
            {
                return;
            }

            Color old = Gizmos.color;

            /// 获取鼠标世界坐标：
            /// Event是从左上角(Left Up)开始，
            /// 而Camera是从左下角(Left Down)，
            /// 需要转换才能使用Camera的ScreenToWorldPoint方法。
            Vector2 screenPosition = new Vector2(e.mousePosition.x, sceneView.camera.pixelHeight - e.mousePosition.y);
            Vector2 worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);

            // 当前鼠标所在Cell的Position
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            // 当前鼠标所在Cell的Center坐标
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);

            /// 绘制当前鼠标下的Cell边框与Position
            /// 如果包含Cell，正常绘制
            /// 如果不包含Cell，改变颜色，并多绘制一个叉
            if (Contains(cellPosition))
            {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = m_EditorCellColor;
                Gizmos.color = m_EditorCellColor;

                Handles.Label(cellCenter - halfCellSize, (new Vector2Int(cellPosition.x, cellPosition.y)).ToString(),
                    textStyle);
                Gizmos.DrawWireCube(cellCenter, grid.cellSize);
            }
            else
            {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = m_EditorErrorColor;
                Gizmos.color = m_EditorErrorColor;

                Handles.Label(cellCenter - halfCellSize, (new Vector2Int(cellPosition.x, cellPosition.y)).ToString(),
                    textStyle);
                Gizmos.DrawWireCube(cellCenter, grid.cellSize);

                // 绘制Cell对角线
                Vector3 from = cellCenter - halfCellSize;
                Vector3 to = cellCenter + halfCellSize;
                Gizmos.DrawLine(from, to);
                float tmpX = from.x;
                from.x = to.x;
                to.x = tmpX;
                Gizmos.DrawLine(from, to);
            }

            Gizmos.color = old;
        }

        #endregion

#endif

        #region Field

        [Header("Map Setting")] [SerializeField]
        private string m_MapName;

        [SerializeField] private RectInt m_MapRect = new RectInt(0, 0, 10, 10);

        private Grid m_Grid;

        #endregion

        #region Property

        /// <summary>
        /// 地图的名称
        /// </summary>
        public string mapName
        {
            get { return m_MapName; }
            set { m_MapName = value; }
        }

        /// <summary>
        /// 地图的矩形框
        /// </summary>
        public RectInt mapRect
        {
            get { return m_MapRect; }
            set { m_MapRect = value; }
        }

        /// <summary>
        /// 地图左下角Position
        /// </summary>
        public Vector3Int leftDownPosition
        {
            get { return new Vector3Int(m_MapRect.xMin, m_MapRect.yMin, 0); }
        }

        /// <summary>
        /// 地图右上角Position
        /// </summary>
        public Vector3Int rightUpPosition
        {
            get { return new Vector3Int(m_MapRect.xMax - 1, m_MapRect.yMax - 1, 0); }
        }

        /// <summary>
        /// 地图宽
        /// </summary>
        public int width
        {
            get { return m_MapRect.width; }
        }

        /// <summary>
        /// 地图高
        /// </summary>
        public int height
        {
            get { return m_MapRect.height; }
        }

        /// <summary>
        /// Grid组件
        /// </summary>
        public Grid grid
        {
            get
            {
                if (m_Grid == null)
                {
                    m_Grid = GetComponent<Grid>();
                }

                return m_Grid;
            }
        }

        /// <summary>
        /// 地图每个cell尺寸的一半
        /// </summary>
        public Vector3 halfCellSize
        {
            get { return grid.cellSize / 2f; }
        }


        [SerializeField] private Tilemap m_TerrainTilemap;

        /// <summary>
        /// 计算的Tilemap
        /// </summary>
        public Tilemap terrainTilemap
        {
            get { return m_TerrainTilemap; }
            set { m_TerrainTilemap = value; }
        }

        /// <summary>
        /// 地图每个格子的信息
        /// </summary>
        private Dictionary<Vector3Int, CellData> m_DataDict = new Dictionary<Vector3Int, CellData>();

        #endregion

        #region Path Finding Field

        /// <summary>
        /// 寻路核心
        /// </summary>
        private PathFinding m_SearchPath;

        [Header("Path Finding")] [SerializeField]
        private FindRange m_FindAttackRange;

        [SerializeField] private FindRange m_FindMoveRange;

        [SerializeField] private FindRange m_FindPathDirect;

        #endregion

        #region  Path Finding Property

        /// <summary>
        /// 寻路核心
        /// </summary>
        public PathFinding searchPath => m_SearchPath;

        /// <summary>
        /// 寻找攻击范围
        /// </summary>
        public FindRange findAttackRange
        {
            get => m_FindAttackRange;
            set => m_FindAttackRange = value;
        }

        /// <summary>
        /// 寻找移动范围
        /// </summary>
        public FindRange findmoverange
        {
            get => m_FindMoveRange;
            set => m_FindMoveRange = value;
        }

        /// <summary>
        /// 无视移动力，直接寻找路径
        /// </summary>
        public FindRange findPathDirect
        {
            get => m_FindPathDirect;
            set => m_FindPathDirect = value;
        }

        public void InitMap(bool reinitCellDatas = false)
        {
            if (reinitCellDatas)
            {
                ClearCellDatas();
            }

            CreateCleeDatas();

            if (m_SearchPath == null)
            {
                m_SearchPath = new PathFinding(this);
            }

            //TODO Other Init
        }

        /// <summary>
        /// 建立CellData
        /// </summary>
        private void CreateCleeDatas()
        {
            if (m_DataDict.Count != 0)
            {
                return;
            }

            for (int y = mapRect.yMin; y < mapRect.yMax; y++)
            {
                for (int x = mapRect.xMin; x < mapRect.xMax; x++)
                {
                    CellData cell = new CellData(x, y);
                    m_DataDict.Add(cell.position, cell);
                }
            }

            foreach (CellData cell in m_DataDict.Values)
            {
                cell.hasTile = GetTile(cell.position) != null;
                FindAjacents(cell);
            }
        }

        /// <summary>
        /// 添加邻居
        /// </summary>
        /// <param name="cell"></param>
        private void FindAjacents(CellData cell)
        {
            cell.adjacents.Clear();
            Vector3Int position = cell.position;
            Vector3Int pos;

            //up
            pos = new Vector3Int(position.x, position.y + 1, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }

            //right
            pos = new Vector3Int(position.x + 1, position.y, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }

            //down
            pos = new Vector3Int(position.x, position.y - 1, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }

            //left
            pos = new Vector3Int(position.x - 1, position.y, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }
        }

        /// <summary>
        /// 获取Terrain层的Tile
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public SrpgTile GetTile(Vector3Int position)
        {
            return terrainTilemap.GetTile<SrpgTile>(position);
        }

        /// <summary>
        /// 删除已有的CellData
        /// </summary>
        private void ClearCellDatas()
        {
            if (m_DataDict.Count > 0)
            {
                foreach (CellData cell in m_DataDict.Values)
                {
                    if (cell.hasMapObject)
                    {
                        //TODO destory/despawn map object
                    }

                    cell.Dispose();
                }

                m_DataDict.Clear();
            }
        }

        /// <summary>
        /// 搜寻移动范围
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="movePoint"></param>
        /// <returns></returns>
        public List<CellData> SearchMoveRange(CellData cell, float movePoint,MoveConsumption moveConsumption)
        {
            if (findmoverange == null)
            {
                Debug.LogError("Error: Find move range is null");
                return null;
            }

            if (!m_SearchPath.SearchMoveRange(findmoverange, cell, movePoint,moveConsumption))
            {
                Debug.LogErrorFormat("Error: Move Range({0}) is Not Found", 5f);
            }

            return m_SearchPath.result;
        }

        /// <summary>
        /// 搜寻攻击范围
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <returns></returns>
        public List<CellData> SearchAttackRange(CellData cell, int minRange, int maxRange,bool useEndCell = false)
        {
            if (findAttackRange == null)
            {
                Debug.LogError("Error :Find attack range is null");
                return null;
            }

            if (!m_SearchPath.SearchAttackRange(findAttackRange, cell, minRange, maxRange,useEndCell))
            {
                Debug.LogErrorFormat("Error : Attack Range{0} - {1} is Not Found", 2, 3);
            }

            return m_SearchPath.result;
        }

        /// <summary>
        /// 搜寻路径
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        /// <returns></returns>
        public List<CellData> SearchPath(CellData startCell, CellData endCell,MoveConsumption consumption)
        {
            if (findPathDirect == null)
            {
                Debug.LogError("Error : Find path is null");
                return null;
            }

            if (!m_SearchPath.SearchPath(findPathDirect,startCell,endCell,consumption))
            {
                Debug.LogErrorFormat("Error : Search Path Error. Maybe some cells are out of range.");
                return null;
            }

            return m_SearchPath.result;
        }


        #endregion

        #region Helper Method

        /// <summary>
        /// 地图是否包含Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector3Int position)
        {
            return mapRect.Contains(new Vector2Int(position.x, position.y));
        }

        /// <summary>
        /// 获取格子信息
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CellData GetCellData(Vector3Int position)
        {
            if (!Contains(position))
            {
                return null;
            }

            return m_DataDict[position];
        }

        /// <summary>
        /// 获取Cell的位置
        /// </summary>
        /// <param name="cellPosition">网络坐标</param>
        /// <param name="world">事否是世界坐标</param>
        /// <param name="center">是否是中心坐标</param>
        /// <returns></returns>
        public Vector3 GetCellPosition(Vector3Int cellPosition, bool world = true, bool center = false)
        {
            Vector3 pos;
            if (world)
            {
                pos = grid.GetCellCenterWorld(cellPosition);
            }
            else
            {
                pos = grid.GetCellCenterLocal(cellPosition);
            }

            if (!center)
            {
                pos.y -= halfCellSize.y;
            }

            return pos;
        }

        #endregion
    }
}