using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using UnityEngine;

namespace Arycs_Fe.FindPath
{
    [CreateAssetMenu(fileName = "FindMoveRange.asset", menuName = "SRPG/How To Find Move Range")]
    public class FindMoveRange : FindRange
    {
        public override CellData ChoseCell(PathFinding search)
        {
            if (search.reachable.Count == 0)
            {
                return null;
            }

            //取得F最小的节点（因为我们没有计算H，这里就是G）
            //当你在寻找路径有卡顿时，请一定使用更好的查找方式，
            //例如可以改用二叉树的方式
            //也可以将PathFinding里面reachable.Add(Adjacent)的方法改成边排序边加入的方法
            search.reachable.Sort((cell1, cell2) => -cell1.f.CompareTo(cell2.f));
            int index = search.reachable.Count - 1;
            CellData chose = search.reachable[index];
            search.reachable.RemoveAt(index);

            return chose;
        }

        public override float CalcGPerCell(PathFinding search, CellData adjacent)
        {
            //获取邻居的Tile
            SrpgTile tile = search.map.GetTile(adjacent.position);

            return search.GetMoveConsumption(tile.terrainType);
        }

        public override bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent)
        {
            //是否可移动
            if (!adjacent.canMove)
            {
                return false;
            }

            //如果已经在关闭集
            if (search.IsCellInExplored(adjacent))
            {
                return false;
            }

            //计算消耗 = 当前cell的消耗 + 邻居cell的消耗
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

            //不在范围内
            if (g < 0f || g > search.range.y)
            {
                return false;
            }

            adjacent.g = g;
            adjacent.previous = search.currentCell;

            return true;
        }

        public override void BuildResult(PathFinding search)
        {
            for (int i = 0; i < search.explored.Count; i++)
            {
                CellData cell = search.explored[i];
                if (cell.g >= search.range.x && cell.g <= search.range.y)
                {
                    search.result.Add(cell);
                }
            }
        }
    }
}