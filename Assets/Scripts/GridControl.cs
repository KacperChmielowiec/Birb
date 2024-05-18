using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridControl : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] LayerMask TerrainMask;
    PathFinding PathFinding;
    Vector2Int currPosition;
    List<PathNode> pathNodes = new List<PathNode>();
    // Start is called before the first frame update
    void Start()
    {
        PathFinding = grid.GetComponent<PathFinding>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit hit, float.MaxValue,TerrainMask))
            {
                Vector2Int posGrid = grid.getGridPosition(hit.point);
                Debug.Log("currPostition");
                Debug.Log(posGrid);
                if(currPosition != null)
                {
                    pathNodes = PathFinding.FindPath(currPosition.x, currPosition.y, posGrid.x, posGrid.y);
                }
                
                currPosition = posGrid;
      
            }
        }
    }

    private void OnDrawGizmos()
    {
        
        if (PathFinding != null && pathNodes != null) {
           
            for (int i = 0; i < pathNodes.Count  - 1; i++)
            {
               
                Gizmos.color = Color.green;
                Gizmos.DrawLine(grid.GetWorldPosition(pathNodes[i].pos_x,
                    pathNodes[i].pos_y, true), grid.GetWorldPosition(pathNodes[i + 1].pos_x, pathNodes[i + 1].pos_y, true));
            }
        
        }
    }
}
