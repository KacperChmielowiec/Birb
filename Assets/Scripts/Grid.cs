using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    Node[,] grid;
    public int width = 20;
    public int height = 20;
    public int cellSize = 1;
    [SerializeField] LayerMask obstaclesMask;
    [SerializeField] LayerMask terrainLayer;


    public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition;
    public class OnAnyUnitMovedGridPositionEventArgs : EventArgs
    {
        public GridObject unit;
        public Vector2Int fromGridPosition;
        public Vector2Int toGridPosition;
    }


    // Start is called before the first frame update
    void Awake()
    {
        this.grid = new Node[width, height];
    }
    private void Start()
    {
        GenerateGrid();
        GameControl.Instance.CreateEvent(ref OnAnyUnitMovedGridPosition, EventType.OnAnyUnitMovedGridPosition, this);
        GameControl.Instance.OnInitGrid();
    }
    public void UnitMovedGridPosition(GridObject unit, Vector2Int fromGridPosition, Vector2Int toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
        {
            unit = unit,
            fromGridPosition = fromGridPosition,
            toGridPosition = toGridPosition,
        });
    }
    public void RemoveUnitAtGridPosition(Vector2Int gridPosition, GridObject unit)
    {
        Node node = GetGridNode(gridPosition);
        if(node?.GridObject)
        {
            node.RemoveObject();
        }
    }

    public void AddUnitAtGridPosition(Vector2Int gridPosition, GridObject unit)
    {
        Node node = GetGridNode(gridPosition);
        if (node != null)
        {
            node.GridObject = unit;
        }
    }

    public PathNode GetNode(Vector2Int pos)
    {
        if(BoundaryCheck(pos))
        {
            return new PathNode(pos.x,pos.y);
        }
        return null;
    }
    private void CalculateElevation()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Ray ray = new Ray(GetWorldPosition(i,j) + Vector3.up * 30f, Vector3.down);
                if(Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, terrainLayer))
                {
                    grid[i, j].elevation = hitInfo.point.y;
                }
               
            }
        }
    }
    public bool CheckWalkable(int x, int y)
    {
        return grid[x, y].Passable;
    }
    public bool CheckWalkable(Vector2Int pathNode)
    {
        return grid[pathNode.x, pathNode.y].Passable;
    }
    public bool BoundaryCheck(Vector2Int pos)
    {
        if(pos.x >= width || pos.y >= height) return false;
        if(pos.x < 0 || pos.y < 0) return false;
        return true;
    }
    public bool BoundaryCheck(PathNode pos)
    {
        if (pos.pos_x >= width || pos.pos_y >= height) return false;
        if (pos.pos_x < 0 || pos.pos_y < 0) return false;
        return true;
    }
    public bool BoundaryCheck(int x, int y)
    {
        if (x >= width || y >= height) return false;
        if (x < 0 || y < 0) return false;
        return true;
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j] = new Node();
            }
        }
        CalculateElevation();
        CheckPassableTerrain();
        
    }
    public bool HasAnyUnitOnGridPosition(Vector2Int gridPosition)
    {
        Node gridNode = GetGridNode(gridPosition);
        return gridNode?.HasAnyUnit() ?? false;
    }
    public Node GetGridNode(Vector2Int pathNode)
    {
        if(!BoundaryCheck(pathNode)) return null;
        return grid[pathNode.x, pathNode.y];
    }
    private void CheckPassableTerrain()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Ray ray = new Ray(GetWorldPosition(i, j) + Vector3.up * 30f, Vector3.down);
                bool passable = !Physics.CheckBox(GetWorldPosition(i, j), Vector3.one / 4 * cellSize, Quaternion.identity, obstaclesMask) &&
                    Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, terrainLayer);

                grid[i, j].Passable = passable;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
       
        if(this.grid == null) return;
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Vector3 vector = GetWorldPosition(i, j, true);
                Gizmos.color = !grid[i,j].Passable ? Color.red : Color.white;
                Gizmos.DrawCube(vector, Vector3.one/4);
            }
        }
    }
    public Vector3 GetWorldPosition(int x , int y, bool elevation = false)
    {
      
        return new Vector3(x * cellSize, elevation ? grid[x,y].elevation : 1f , y * cellSize);
    }
    public Vector2Int getGridPosition(Vector3 pos)
    {
     
        pos.x -= cellSize / 2;
        pos.z -= cellSize / 2;
        return new Vector2Int() { x = (int)Mathf.Round(((pos.x / cellSize) * 2) / 2), y = (int)Mathf.Round(((pos.z / cellSize) * 2) / 2) };
    }
    public void PlaceObject(GridObject obj,Vector2Int pos)
    {
        if (!BoundaryCheck(pos)) return;
        grid[pos.x, pos.y].GridObject = obj;
        
    }
    public GridObject GetPlacedObject(Vector2Int pos)
    {
        if(!BoundaryCheck(pos)) return null;
        if (grid[pos.x,pos.y].GridObject != null)
        {
            return grid[pos.x, pos.y].GridObject;
        }
        return null;
    }

    internal List<Vector3> ConvertPathToWorldPositions(List<PathNode> pathNodes)
    {
        List<Vector3> result = new List<Vector3>();
        foreach (var item in pathNodes)
        {
            result.Add(this.GetWorldPosition(item.pos_x, item.pos_y,true));
        }
        return result;
    }
}
