using UnityEngine;

namespace Dot.Core.AI.AStar
{
    public class FinderSetting : ScriptableObject
    {
        //启法式算法
        public HeuristicFormula formula = HeuristicFormula.Manhattan;
        //对角线是否可走
        public bool isDiagonals = true;
        //是否惩罚走对角线
        public bool isHeavyDiagonals = true;
        //对角线代价
        public float heavyDiagonalsWeight = 1.414f;
        //一个格子的代价
        public int heuristicEstimate = 1;
        //是否惩罚切换方向
        public bool isPunishChangeDirection = true;
        //切换方向代价
        public int punishChangeDirection = 1;
        //是否提高代价，为了减少格子数量，微量增加代价
        public bool isTieBreaker = true;
        //微小代价
        public float tieBreakerWeight = 0.001f;
        //最大搜索量
        public int searchLimit = 200;
    }
}
