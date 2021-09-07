using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using UnityEngine;

namespace Arycs_Fe.FindPath
{
    public interface IHowToFind
    {
        /// <summary>
        /// 获取检测的Cell
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        CellData ChoseCell(PathFinding search);

        /// <summary>
        /// 选择cell后，是否结束
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        bool IsFinishedOnChose(PathFinding search);

        /// <summary>
        /// 计算移动到下一格的消耗
        /// </summary>
        /// <param name="search"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        float CalcGPerCell(PathFinding search, CellData adjacent);

        /// <summary>
        /// 无视范围，直接寻路用，计算预计消耗值（这里用距离）
        /// </summary>
        /// <param name="search"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        float CalcH(PathFinding search, CellData adjacent);

        /// <summary>
        /// 是否能把邻居加入到检测列表中
        /// </summary>
        /// <param name="search"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent);

        /// <summary>
        /// 生成最终显示范围
        /// </summary>
        /// <param name="search"></param>
        void BuildResult(PathFinding search);
    }
}