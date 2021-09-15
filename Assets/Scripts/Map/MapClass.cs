using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.Models;
using UnityEngine;

namespace Arycs_Fe.Maps
{
    [AddComponentMenu("SRPG/Map Object/Map Class")]
    [RequireComponent(typeof(ClassAnimatorController))]
    public class MapClass : MapObstacle
    {
        #region Delegate

        public delegate void OnMovingEndDelegate(CellData endCell);

        public event OnMovingEndDelegate onMovingEnd;

        #endregion

        public override MapObjectType mapObjectType
        {
            get { return MapObjectType.Class; }
        }

        //TODO


        private ClassAnimatorController m_AnimatorController;

        /// <summary>
        /// 职业动画控制器
        /// </summary>
        public ClassAnimatorController animatorController
        {
            get
            {
                if (m_AnimatorController == null)
                {
                    m_AnimatorController = GetComponent<ClassAnimatorController>();
                }

                return m_AnimatorController;
            }
        }

        [SerializeField] private float m_MoveTimePerCell = 1f;

        private bool m_Moving = false;

        /// <summary>
        /// 每一格子移动时间
        /// </summary>
        public float moveTimePerCell
        {
            get { return m_MoveTimePerCell; }
            set { m_MoveTimePerCell = value; }
        }

        /// <summary>
        /// 是否在移动状态
        /// </summary>
        public bool moving
        {
            get { return m_Moving; }
        }

        private object m_Role; //TODO 没有写数据，先使用object代替

        /// <summary>
        /// 获取移动中的方向
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private Direction getMovingDirection(CellData current, CellData next)
        {
            Vector3Int offset = next.position - current.position;
            if (offset.x == 0)
            {
                if (offset.y == 1)
                {
                    return Direction.Up;
                }
                else if (offset.y == -1)
                {
                    return Direction.Down;
                }
            }
            else if (offset.y == 0)
            {
                if (offset.x == 1)
                {
                    return Direction.Right;
                }
                else if (offset.x == -1)
                {
                    return Direction.Left;
                }
            }

            Debug.LogError("MapClass -> GetMovingDirection:" + "Check the code. Offset: " + offset.ToString());
            return Direction.Down;
        }

        /// <summary>
        /// 移动每一格
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private IEnumerator MovingTo(CellData current, CellData next)
        {
            //设置方向
            Direction direction = getMovingDirection(current, next);
            animatorController.SetMoveDirection(direction);

            //计算 sortingOrder
            if (renderer != null)
            {
                renderer.sortingOrder = MapObject.ClacSortingOrder(map, next.position);
            }
            
            if (m_MoveTimePerCell <= 0f)
            {
                Debug.LogError(name + "Move time can not be less equal zero");
                UpdatePosition(next.position);
                yield break;
            }

            //获取起始和结束坐标
            Vector3 start = map.GetCellPosition(current.position);
            Vector3 end = map.GetCellPosition(next.position);

            //开始移动
            Vector3 pos;
            float time = 0f;
            while (true)
            {
                time += Time.deltaTime;
                pos = Vector3.Lerp(start, end, time / m_MoveTimePerCell);
                transform.position = pos;
                yield return null;
                if (pos == end)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="movePath"></param>
        public void StartMove(Stack<CellData> movePath)
        {
            if (movePath == null || movePath.Count == 0)
            {
                Debug.LogError(name + "StartMove Error, path is null or empty");
                return;
            }

            if (m_Moving)
            {
                Debug.LogError(name + "is in moving");
                return;
            }


            if (map == null)
            {
                Debug.LogError(name + "map is null.");
                return;
            }

            StartCoroutine(Moving(movePath));
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="movePath"></param>
        /// <returns></returns>
        private IEnumerator Moving(Stack<CellData> movePath)
        {
            m_Moving = true;

            //获取当前节点
            CellData current = movePath.Pop();
            CellData next;
            while (movePath.Count > 0)
            {
                //获取下一个节点
                next = movePath.Pop();
                if (next == null)
                {
                    Debug.LogError("MapClass -> Moving Error." +
                                   "Next cellData is null" +
                                   "Current cell Data is" +
                                   current.position.ToString());
                    continue;
                }

                //等待移动一个格子完成
                yield return MovingTo(current, next);
                //设置当前节点
                current = next;
            }

            m_Moving = false;
            if (onMovingEnd != null)
            {
                onMovingEnd(current);
            }
        }

        /// <summary>
        /// 读取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Load(int id)
        {
            //TODO 
            return true;
        }

        //TODO 对象池 ，回池调用
        public void OnDespawn()
        {
            if (moving)
            {
                StopAllCoroutines();
                m_Moving = false;
            }
        }
    }
}