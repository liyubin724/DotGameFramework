using Advanced.Algorithms.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.AI.AStar
{
    public class PathFinder
    {
        private static readonly sbyte[,] GRID_DIRECTION = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
        private static readonly sbyte[,] GRID_DIAGONALS_DIRECTION = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };

        private FinderSetting setting = null;
        private IMapData mapData = null;
        private IHeuristicAlgorithm algorithm = null;
        private PriorityQueue<Node> openNodeQueue = null;
        private Dictionary<int, Node> nodeDic = new Dictionary<int, Node>();

        private sbyte[,] direction = null;
        public PathFinder(FinderSetting setting, IMapData mapData)
        {
            this.setting = setting;
            this.mapData = mapData;

            NodeFactroy.Init();
            algorithm = AlgorithmFactory.GetAlgorithm(setting.formula);
            openNodeQueue = new PriorityQueue<Node>();
            direction = setting.isDiagonals ? GRID_DIAGONALS_DIRECTION : GRID_DIRECTION;
        }

        public Vector3[] Find(Vector3 startPos, Vector3 endPos)
        {
            return null;
        }
        public Vector3[] Find(int startIndex, int endIndex)
        {
            return null;
        }

        private int[] Find(int startX, int startY, int endX, int endY)
        {
            if (mapData.IsBlock(startX, startY) || mapData.IsBlock(endX, endY)) return null;
            if (startX == endX && startY == endY) return new int[] { endX, endY };

            Node node = NodeFactroy.GetNode();
            node.X = startX;
            node.Y = startY;
            node.Index = mapData.GetIndex(startX, startY);
            node.G = mapData.GetCost(startX, endY);
            node.H = algorithm.GetH(setting.heuristicEstimate, startX, startY, endX, endY);
            node.Status = NodeStatus.Open;

            AddNode(node);

            bool isFound = false;
            while (openNodeQueue.Count > 0)
            {
                node = openNodeQueue.Dequeue();
                if (node.X == endX && node.Y == endY)
                {
                    isFound = true;
                    node.Status = NodeStatus.Close;
                    break;
                }

                if (setting.searchLimit != 0 && nodeDic.Count >= setting.searchLimit)
                {
                    break;
                }

                for (int i = 0; i < direction.GetLength(0); ++i)
                {
                    int neighbourX = node.X + direction[i, 0];
                    int neighbourY = node.Y + direction[i, 1];
                    int neighbourIndex = mapData.GetIndex(neighbourX, neighbourY);

                    if (!mapData.IsGridValid(neighbourX, neighbourY) || mapData.IsBlock(neighbourX, neighbourY))
                    {
                        continue;
                    }

                    float g = node.G + GetGWithHeavyDiagonals(i > 3) + GetGWithChangeDirection(node.Px, node.Py, node.X, node.Y, neighbourX, neighbourY) + GetHWithTieBreaker(node.X,node.Y,startX,startY,endX,endY);
                    if (!nodeDic.TryGetValue(neighbourIndex, out Node neighbourNode))
                    {
                        neighbourNode = NodeFactroy.GetNode();
                        neighbourNode.X = neighbourX;
                        neighbourNode.Y = neighbourY;
                        neighbourNode.Px = node.X;
                        neighbourNode.Py = node.Y;
                        neighbourNode.Index = neighbourIndex;
                        neighbourNode.G = g;
                        neighbourNode.H = algorithm.GetH(setting.heuristicEstimate, neighbourX, neighbourY, endX, endY);
                        neighbourNode.Status = NodeStatus.Open;

                        AddNode(neighbourNode);
                        continue;
                    }

                    if (neighbourNode.Status == NodeStatus.Close)
                    {
                        if (neighbourNode.G > g)
                        {
                            neighbourNode.G = g;
                            neighbourNode.Px = node.X;
                            neighbourNode.Py = node.Y;
                            neighbourNode.Status = NodeStatus.Open;

                            openNodeQueue.Enqueue(neighbourNode);
                        }
                    }
                    else if (neighbourNode.Status == NodeStatus.Open)
                    {
                        if (neighbourNode.G > g)
                        {
                            neighbourNode.G = g;
                            neighbourNode.Px = node.X;
                            neighbourNode.Py = node.Y;

                            openNodeQueue.Update(neighbourNode);
                        }
                    }
                }

                node.Status = NodeStatus.Close;
            }

            openNodeQueue = new PriorityQueue<Node>();
            if (isFound)
            {
                int endIndex = mapData.GetIndex(endX, endY);
                List<int> pathList = new List<int>();
                Node preNode = nodeDic[endIndex];
                while(preNode.Px!=-1&&preNode.Py!=-1)
                {
                    pathList.Add(preNode.X);
                    pathList.Add(preNode.Y);

                    int preIndex = mapData.GetIndex(preNode.Px, preNode.Py);
                    preNode = nodeDic[preIndex];
                }

                return pathList.ToArray();
            }

            foreach(var kvp in nodeDic)
            {
                NodeFactroy.ReleaseNode(kvp.Value);
            }
            nodeDic.Clear();
            
            return null;
        }

        public void AddNode(Node node)
        {
            nodeDic.Add(node.Index, node);
            if (node.Status == NodeStatus.Open)
                openNodeQueue.Enqueue(node);
        }


        private void AddOpenNode(Node openNode)
        {
            openNodeQueue.Enqueue(openNode);
        }

        private float GetHWithTieBreaker(int px, int py, int sx, int sy, int ex, int ey)
        {
            if (setting.isTieBreaker)
            {
                int dx1 = px - ex;
                int dy1 = py - ey;
                int dx2 = sx - ex;
                int dy2 = sy - ey;
                int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
                return cross * setting.tieBreakerWeight;
            }
            else
            {
                return 0;
            }
        }

        private float GetGWithHeavyDiagonals(bool isDiagonals)
        {
            if (setting.isHeavyDiagonals && isDiagonals)
            {
                return setting.heavyDiagonalsWeight;
            }
            else
            {
                return 0;
            }
        }

        private float GetGWithChangeDirection(int ppx, int ppy, int px, int py, int x, int y)
        {
            if ((ppx == -1 && ppy == -1) || !setting.isPunishChangeDirection)
                return 0;
            int value = (py - ppy) * (x - px) - (y - py) * (px - ppx);
            if (value == 0)
            {
                return 0;
            }
            else
            {
                return setting.punishChangeDirection;
            }
        }
    }
}
