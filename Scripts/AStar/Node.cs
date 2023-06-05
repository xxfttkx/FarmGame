using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; //当前网格的坐标
        public int gCost = 0; //从开始网格到当前网格的距离，即代价函数
        public int hCost = 0; //从当前网格到目标网格的距离，即启发式函数
        public int fCost => gCost + hCost; //当前网格的价值，即估价函数
        public bool isObstacle = false; //当前格子是否是障碍
        public Node parentNode; //表示是从哪个网格找到当前网格的

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            //比较选出最低的F值，返回-1，0，1
            int result = fCost.CompareTo(other.fCost);
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}