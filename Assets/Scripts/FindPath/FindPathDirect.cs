using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arycs_Fe.Maps;
using UnityEngine;

namespace Arycs_Fe.FindPath
{
    [CreateAssetMenu(fileName = "FindPathDirect.asset", menuName = "SRPG/How To Find Path")]
    public class FindPathDirect : FindMoveRange
    {
        public override bool IsFinishedOnChose(PathFinding search)
        {
            //如果开放集中已经空了，则说明没有到达目标点
            if (search.currentCell == null)
            {
                //使用H最小值建立结果
                CellData minHCell = search.explored.First(cell => cell.h == search.explored.Min(c => c.h));
                search.BuildPath(minHCell, true);
                return true;
            }

            if (search.currentCell == search.endCell)
            {
                return true;
            }

            if (!search.IsCellInExplored(search.currentCell))
            {
                search.explored.Add(search.currentCell);
            }
            return false;
        }

        public override float CalcH(PathFinding search, CellData adjacent)
        {
            Vector2 hVec;
            hVec.x = Mathf.Abs(adjacent.position.x - search.endCell.position.x);
            hVec.y = Mathf.Abs(adjacent.position.y - search.endCell.position.y);
            return hVec.x + hVec.y;
        }

        public override bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent)
        {
            //没有Tile
            if (!adjacent.hasTile)
            {
                return false;
            }
            //已经有对象了
            if (adjacent.hasMapObject)
            {
                return false;
            }
            //如果已经在关闭集
            if (search.IsCellInExplored(adjacent))
            {
                return false;
            }
            
            //计算消耗 = 当前cell 的消耗 + 邻居cell 的消耗
            float g = search.currentCell.g + CalcGPerCell(search, adjacent);
            
            //已经加入过开放集
            if (search.IsCellInReachable(adjacent))
            {
                //如果新消耗更低
                if (g < adjacent.g)
                {
                    adjacent.g = g;
                    adjacent.previous = search.currentCell;
                }

                return false;
            }

            adjacent.g = g;
            adjacent.h = CalcH(search, adjacent);
            adjacent.previous = search.currentCell;
            return true;
        }
        
        public override void BuildResult(PathFinding search)
        {
            //当没有达到目标时，已经建立过结果
            if (search.result.Count > 0)
            {
                return;
            }

            search.BuildPath(search.endCell, true);
        }
        
    }
}