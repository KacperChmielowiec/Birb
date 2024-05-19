using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform GridVisualTile;
    [SerializeField] private Grid grid;
    GridVisualSingle[,] gridVisualSingles;
    public static GridSystemVisual Instance { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Init()
    {
        gridVisualSingles = new GridVisualSingle[grid.width, grid.height];
        for (int i = 0; i < grid.height; i++)
        {
            for (int j = 0; j < grid.width; j++)
            {
                if (grid.CheckWalkable(i, j))
                {
                    var buffor = Instantiate(GridVisualTile, grid.GetWorldPosition(i, j, true) + new Vector3(0, 0.1f, 0), Quaternion.AngleAxis(90, new Vector3(0, 1, 0)));
                    gridVisualSingles[i, j] = buffor.GetComponent<GridVisualSingle>();
                }
            }
        }
    }


    public void ActiveAllPosition()
    {
        for (int i = 0; i < grid.height; i++)
        {
            for (int j = 0; j < grid.width; j++)
            {
                if (grid.CheckWalkable(i, j))
                {
                    gridVisualSingles[i, j].Active();
                }
            }
        }
    }

    public void ActiveList(List<PathNode> nodes)
    {
        foreach (PathNode node in nodes)
        {
            gridVisualSingles[node.pos_x,node.pos_y].Active();
        }
    }

}
