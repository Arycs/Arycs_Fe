using System.Collections.Generic;
using System.Linq;
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

            // 获取鼠标世界坐标：
            // Event是从左上角(Left Up)开始，
            // 而Camera是从左下角(Left Down)，
            // 需要转换才能使用Camera的ScreenToWorldPoint方法。
            Vector2 screenPosition = new Vector2(e.mousePosition.x, sceneView.camera.pixelHeight - e.mousePosition.y);
            Vector2 worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);

            // 当前鼠标所在Cell的Position
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            // 当前鼠标所在Cell的Center坐标
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);

            // 绘制当前鼠标下的Cell边框与Position
            // 如果包含Cell，正常绘制
            // 如果不包含Cell，改变颜色，并多绘制一个叉
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

        #region Map Object Field

        [Header("Map Object Setting")] [SerializeField]
        private MapMouseCursor m_MouseCursorPrefab;

        [SerializeField] private MapCursor m_CursorPrefab;

        /// <summary>
        /// 生成的MapMouseCursor
        /// </summary>
        private MapMouseCursor m_MouseCursor;

        /// <summary>
        /// 运行时，MapCursor的预制体
        /// </summary>
        private MapCursor m_RuntimeCursorPrefab;

        /// <summary>
        /// 光标集合
        /// </summary>
        private HashSet<MapCursor> m_Cursors = new HashSet<MapCursor>();

        /// <summary>
        /// 职业集合
        /// </summary>
        private List<MapClass> m_Classes = new List<MapClass>();

        private CellPositionEqualityComparer m_CellPositionEqualityComparer = new CellPositionEqualityComparer();

        public CellPositionEqualityComparer CellPositionEqualityComparer
        {
            get
            {
                if (m_CellPositionEqualityComparer == null)
                {
                    m_CellPositionEqualityComparer = new CellPositionEqualityComparer();
                }

                return m_CellPositionEqualityComparer;
            }
        }

        #endregion

        #region Map Object Property

        /// <summary>
        /// 默认 Mouse Cursor 的Prefab
        /// </summary>
        public MapMouseCursor mouseCursorPrefab
        {
            get { return m_MouseCursorPrefab; }
            set { m_MouseCursorPrefab = value; }
        }

        /// <summary>
        /// 默认Cursor的Prefab
        /// </summary>
        public MapCursor cursorPrefab
        {
            get { return m_CursorPrefab; }
            set { m_CursorPrefab = value; }
        }

        /// <summary>
        /// 用户光标
        /// </summary>
        public MapMouseCursor mouseCursor
        {
            get
            {
                //只有在测试时，才会使用默认Prefab
                //正是游戏，这里不会为null， 在初始化地图时回加载预制体
                //如果无法加载，则说明代码可能出现问题
                if (m_MouseCursor == null)
                {
                    m_MouseCursor = CreateMapObject(mouseCursorPrefab) as MapMouseCursor;
                }
                return m_MouseCursor;
            }
        }
    
        /// <summary>
        /// 运行时，MapCursor的预制体
        /// </summary>
        public MapCursor runtimeCurSorPrefab
        {
            get
            {
                //只有在测试时，才会使用默认Prefab
                //正是游戏，这里不会为null， 在初始化地图时回加载预制体
                //如果无法加载，则说明代码可能出现问题
                if (m_RuntimeCursorPrefab == null)
                {
                    m_RuntimeCursorPrefab = cursorPrefab;
                }
                return m_RuntimeCursorPrefab;
            }
        }

        #endregion

        /// <summary>
        /// 显示Cursor
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="type"></param>
        public void ShowRangeCursors(IEnumerable<CellData> cells, MapCursor.CursorType type)
        {
            if (type == MapCursor.CursorType.Mouse)
            {
                return;
            }

            foreach (CellData cell in cells)
            {
                MapCursor cursor = CreateMapObject(runtimeCurSorPrefab,cell.position) as MapCursor;
                if (cursor != null)
                {
                    cursor.name = string.Format("{0} Cursor {1}", type.ToString(), cell.position.ToString());
                    cursor.cursorType = type;
                    if (type == MapCursor.CursorType.Move)
                    {
                        cell.hasMoveCursor = true;
                    }else if (type == MapCursor.CursorType.Attack)
                    {
                        cell.hasAttackCursor = true;
                    }

                    m_Cursors.Add(cursor);
                }
            } 
        }

        /// <summary>
        /// 隐藏cursor
        /// </summary>
        public void HideRangeCursors()
        {
            if (m_Cursors.Count > 0)
            {
                foreach (MapCursor cursor in m_Cursors)
                {
                    //TODO 利用对象池回收
                }
                m_Cursors.Clear();
            }
        }

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

        /// <summary>
        /// 静态地图对象列表
        /// </summary>
        public GameObject mapObjectPool;
        
        #endregion

        #region Path Finding Field

        /// <summary>
        /// 寻路核心
        /// </summary>
        private PathFinding m_SearchPath;

        [Header("Path Finding")]
        [SerializeField] private FindRange m_FindAttackRange;

        [SerializeField] private FindRange m_FindMoveRange;

        [SerializeField] private FindRange m_FindPathDirect;
        
        public IEqualityComparer<CellData> cellPositionEqualityCompaper;

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

            CreateCellDatas();

            if (m_SearchPath == null)
            {
                m_SearchPath = new PathFinding(this);
            }

            //TODO Other Init
            InitMapObjectsInMap();
        }
    
        /// <summary>
        /// 初始化地图静态对象。
        /// </summary>
        private void InitMapObjectsInMap()
        {
            if (mapObjectPool == null)
            {
                Debug.LogError("MapGraph -> MapObject Pool(这里的Pool 是指静态地图上的父物体列表) is null");
                return;
            }

            MapObject[] mapObjects = mapObjectPool.gameObject.GetComponentsInChildren<MapObject>();
            if (mapObjects != null)
            {
                foreach (MapObject mapObject in mapObjects)
                {
                    //我们的地图对象不应包含Cursor相关物体
                    if (mapObject.mapObjectType == MapObjectType.MouseCursor || mapObject.mapObjectType == MapObjectType.Cursor)
                    {
                        GameObject.Destroy(mapObject.gameObject);
                        continue;
                    }
                    //初始化
                    mapObject.InitMapObject(this);
                    
                    //更新坐标
                    Vector3 world = mapObject.transform.position;
                    Vector3Int cellPosition = grid.WorldToCell(world);
                    mapObject.cellPosition = cellPosition;
                    //设置 cellData
                    CellData cellData = GetCellData(cellPosition);
                    if (cellData != null)
                    {
                        if (cellData.hasMapObject)
                        {
                            Debug.LogErrorFormat("MapObject in cell {0} already exists.",cellPosition.ToString());
                            continue;
                        }

                        cellData.mapObject = mapObject;
                    }
                    //如果是class,
                    //可选项(可忽略): 如果地图上杂兵过多（一般 > 20）， 并且在消灭之后会由于事件触发再次生成大量此Prefab的实例
                    //可考虑将prefab 直接绘制在地图上，
                    //个人 使用直接读配置的生成方式。
                }
            }
        }

        /// <summary>
        /// 建立CellData
        /// </summary>
        private void CreateCellDatas()
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
        /// 搜寻和显示范围
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="nAtk">包含攻击范围</param>
        /// <returns></returns>
        public bool SearchAndShowMoveRange(MapClass cls, bool nAtk)
        {
            IEnumerable<CellData> moveCells, atkCells;
            if (!SearchMoveRange(cls,nAtk,out moveCells,out atkCells))
            {
                return false;
            }

            if (moveCells != null)
            {
                ShowRangeCursors(moveCells,MapCursor.CursorType.Move);
            }

            if (atkCells != null)
            {
                ShowRangeCursors(atkCells,MapCursor.CursorType.Attack);
            }

            return true;
        }

        public bool SearchMoveRange(MapClass cls, bool nAtk, out IEnumerable<CellData> moveCells,
            out IEnumerable<CellData> atkCells)
        {
            moveCells = null;
            atkCells = null;
            if (cls == null)
            {
                Debug.LogError("MapGraph -> SearchMoveRange: 'cls' is null");
                return false;
            }

            CellData cell = GetCellData(cls.cellPosition);
            if (cell == null)
            {
                Debug.LogErrorFormat("MapGraph -> SearchMoveRange: 'cls.cellPosition is out of range'");
                return false;
            }
            //TODO 搜索移动范围，从MapClass中读取数据
            Role role = cls.role;
            if (role == null)
            {
                Debug.LogErrorFormat(
                    "MapGraph -> SearchMoveRange: `cls.role` is null. Pos: {0}", 
                    cell.position.ToString());
                return false;
            }

            float movePoint = role.movePoint;
            MoveConsumption consumption = role.cls.moveConsumption;
            List<CellData> rangeCells = SearchMoveRange(cell, movePoint, consumption);
            if (rangeCells == null)
            {
                return false;
            }

            HashSet<CellData> moveRangeCells = new HashSet<CellData>(rangeCells, CellPositionEqualityComparer);
            moveCells = moveRangeCells;

            if (nAtk && role.equipedWeapon != null)
            {
                //搜索攻击范围， 从MapClass中读取数据
                Vector2Int atkRange = new Vector2Int(
                    role.equipedWeapon.minRange,
                    role.equipedWeapon.maxRange);

                HashSet<CellData> atkRangeCells = new HashSet<CellData>(CellPositionEqualityComparer);
                foreach (CellData moveCell in moveCells)
                {
                    rangeCells = SearchAttackRange(moveCell, atkRange.x, atkRange.y, true);
                    if (rangeCells == null)
                    {
                        return false;
                    }
                    
                    if (rangeCells.Count > 0)
                    {
                        atkRangeCells.UnionWith(rangeCells.Where(c => !c.hasCursor));
                    }
                }

                atkCells = atkRangeCells;
            }
            
            return true;
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

            return m_SearchPath.Result;
        }

        /// <summary>
        /// 搜寻攻击范围
        /// </summary>
        /// <param name="cell">当前Cell信息</param>
        /// <param name="minRange">最小攻击距离</param>
        /// <param name="maxRange">最大攻击距离</param>
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

            return m_SearchPath.Result;
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

            return m_SearchPath.Result;
        }


        #endregion

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="prefab">对象</param>
        /// <returns></returns>
        public MapObject CreateMapObject(MapObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogErrorFormat("MapGraph -> CreateMapObject ERROR !! {0}","Prefab is null");
                return null;
            }

            MapObjectType type = prefab.mapObjectType;
            //用户光标在整个地图中只能有且只有一个
            if (type == MapObjectType.MouseCursor && m_MouseCursor != null)
            {
                //TODO 销毁 旧的光标 ，使用对象池
            }
            
            //TODO 实例化Map object ， 利用对象池实例化一个新的
            GameObject instance;
            if (type == MapObjectType.Cursor || type == MapObjectType.MouseCursor)
            {
                //Todo 利用CursorPool Object 来进行实例化
                instance = new GameObject("CursorPool Object");
            }
            else
            {
                //Todo 利用ClassPool Object 来进行实例化
                instance = new GameObject("ClassPool Object");
            }

            MapObject mapObject = instance.GetComponent<MapObject>();
            mapObject.InitMapObject(this);
            
            if (type == MapObjectType.MouseCursor)
            {
                m_MouseCursor = mapObject as MapMouseCursor;
            }

            return mapObject;
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="prefab">对象</param>
        /// <param name="cellPosition">位置</param>
        /// <returns></returns>
        public MapObject CreateMapObject(MapObject prefab, Vector3Int cellPosition)
        {
            MapObject mapObject = CreateMapObject(prefab);
            if (mapObject != null)
            {
                mapObject.UpdatePosition(cellPosition);
            }

            return mapObject;
        }

        public MapObject CreateMapObject(string prefabName)
        {
            //TODO Load 加载资源，利用框架的方式
            MapObject prefab = new MapClass();
            return CreateMapObject(prefab);
        }

        public MapObject CreateMapObject(string prefabName, Vector3Int cellPositoin)
        {
            //TODO Load 加载资源，利用框架的方式
            MapObject prefab = new MapClass();
            MapObject mapObject = CreateMapObject(prefab);
            if (mapObject != null)
            {
                mapObject.UpdatePosition(cellPositoin);
            }

            return mapObject;
        }


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