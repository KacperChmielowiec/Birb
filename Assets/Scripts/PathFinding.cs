using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




[RequireComponent(typeof(Grid))]
public class PathFinding : MonoBehaviour
{
    public Grid gridMap;
    public PathNode[,] pathNodes;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if(gridMap == null) gridMap = GetComponent<Grid>();
        pathNodes = new PathNode[gridMap.width, gridMap.height];

        for(int i = 0; i < gridMap.width ; i++)
        {
            for(int j = 0; j < gridMap.height; j++)
            {
                pathNodes[i, j] = new PathNode(i,j);
            }
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        
        PathNode startNode = pathNodes[startX, startY];
        PathNode endNode = pathNodes[endX, endY];
       
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        openList.Add(startNode);
      
        while (openList.Count > 0)
        {
            
            PathNode currentNode = openList[0];
            for(int i = 0; i <  openList.Count; i++)
            {
                
                if (currentNode.fValue > openList[i].fValue)
                {
                    currentNode = openList[i];
                }
                if(currentNode.fValue == openList[i].fValue && currentNode.hValue > openList[i].hValue)
                {
                    currentNode = openList[i];
                }
            }
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode == endNode)
            {
                
                return RetracePath(startNode,endNode);
                
            }

            List<PathNode> Neighbours = new List<PathNode>();

            for(int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if( gridMap.BoundaryCheck(currentNode.pos_x + i, currentNode.pos_y + j) == false )
                    {
                        continue;
                    }
                    Neighbours.Add(pathNodes[currentNode.pos_x + i, currentNode.pos_y + j]);
                }
            }
            for(int i = 0; i < Neighbours.Count; i++)
            {
                if (closedList.Contains(Neighbours[i])) { continue; }
                if (!gridMap.CheckWalkable(Neighbours[i].pos_x, Neighbours[i].pos_y)) continue;

                int cost = currentNode.gValue + CalculateDistance(currentNode, Neighbours[i]);

                if (openList.Contains(Neighbours[i]) == false || Neighbours[i].gValue > cost  ) {
                    Neighbours[i].gValue = cost;
                    Neighbours[i].hValue = CalculateDistance(Neighbours[i], endNode);
                    Neighbours[i].parent = currentNode;

                    if(openList.Contains(Neighbours[i]) == false)
                    {
                        openList.Add(Neighbours[i]);
                    }
                }
            }
     
        }
        return null;
    }

    private int CalculateDistance(PathNode currentNode, PathNode pathNode)
    {
        int distX = Math.Abs(currentNode.pos_x  - pathNode.pos_x);
        int distY = Math.Abs(currentNode.pos_y - pathNode.pos_y);

        if( distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> ret = new List<PathNode>();
        PathNode curr = endNode;
       
        while(curr != startNode)
        {
            ret.Add(curr);
            curr = curr.parent;
        }
       
        ret.Reverse();
        return ret;
    }
    public bool HasPath(Vector2Int start, Vector2Int end)
    {
        return true;
    }
    public int GetPathLength(Vector2Int start, Vector2Int end)
    {
        return 1;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
