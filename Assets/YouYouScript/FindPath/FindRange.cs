using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.FindPath;
using Arycs_Fe.Maps;
using UnityEngine;

namespace Arycs_Fe.FindPath
{
    [CreateAssetMenu(fileName = "FindRange.asset", menuName = "SRPG/How To Find Range")]
    public class FindRange : ScriptableObject, IHowToFind
    {
        public virtual CellData ChoseCell(PathFinding search)
        {
            if (search.reachable.Count == 0)
            {
                return null;
            }

            int index = search.reachable.Count - 1;
            CellData chose = search.reachable[index];
            search.reachable.RemoveAt(index);
            return chose;
        }

        public virtual bool IsFinishedOnChose(PathFinding search)
        {
            if (search.currentCell == null)
            {
                return true;
            }

            if (!search.IsCellInExplored(search.currentCell))
            {
                search.explored.Add(search.currentCell);
            }

            return false;
        }

        public virtual float CalcGPerCell(PathFinding search, CellData adjacent)
        {
            return 1f;
        }

        public virtual float CalcH(PathFinding search, CellData adjacent)
        {
            return 0f;
        }

        public virtual bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent)
        {
            //如果已经在关闭集
            if (search.IsCellInExplored(adjacent))
            {
                return false;
            }

            //已经加入开放集
            if (search.IsCellInReachable(adjacent))
            {
                return false;
            }

            //计算消耗
            float h = search.currentCell.g + CalcGPerCell(search, adjacent);

            //不在范围内
            if (h < 0f || h > search.range.y)
            {
                return false;
            }

            adjacent.h = h;
            return true;
        }

        public virtual void BuildResult(PathFinding search)
        {
            for (int i = 0; i < search.explored.Count; i++)
            {
                CellData cell = search.explored[i];
                if (cell.h >= search.range.x && cell.h <= search.range.y)
                {
                    search.result.Add(cell);
                }
            }
        }
    }
}