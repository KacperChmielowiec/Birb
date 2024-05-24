using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystemVisual;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }


    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
        Green,
        Orange,
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;


    private GridVisualSingle[,] gridSystemVisualSingleArray;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Init()
    {
        gridSystemVisualSingleArray = new GridVisualSingle[
            GameControl.Instance.grid.width,
            GameControl.Instance.grid.height
        ];

        for (int x = 0; x < GameControl.Instance.grid.width; x++)
        {
            for (int z = 0; z < GameControl.Instance.grid.height; z++)
            {
                if (GameControl.Instance.grid.CheckWalkable(x, z))
                {
                    var buffor = Instantiate(gridSystemVisualSinglePrefab, GameControl.Instance.grid.GetWorldPosition(x, z, true) + new Vector3(0, 0.1f, 0), Quaternion.AngleAxis(90, new Vector3(0, 1, 0)));
                    gridSystemVisualSingleArray[x, z] = buffor.GetComponent<GridVisualSingle>();
                }
            }
        }


        GameControl.Instance
            .AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>( typeof(GridSystemVisual), UnitActionSystem_OnSelectedActionChanged, EventType.OnAnyActionCompleted) );
        GameControl.Instance
            .AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(typeof(GridSystemVisual), LevelGrid_OnAnyStartAction, EventType.OnAnyActionStarted));
        GameControl.Instance
          .AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(typeof(GridSystemVisual), UnitActionSystem_OnSelectedActionChanged, EventType.OnSelectedUnitChange));

        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < GameControl.Instance.grid.width; x++)
        {
            for (int z = 0; z < GameControl.Instance.grid.height; z++)
            {
                if (GameControl.Instance.grid.CheckWalkable(x, z))
                {
                    gridSystemVisualSingleArray[x, z].Show(GetGridVisualTypeMaterial(GridVisualType.White));
                }
            }
        }
    }

    private void ShowGridPositionRange(Vector2Int gridPosition, int range, GridVisualType gridVisualType)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector2Int testGridPosition = gridPosition + new Vector2Int(x, z);

                if (!GameControl.Instance.grid.BoundaryCheck(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(Vector2Int gridPosition, int range, GridVisualType gridVisualType)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector2Int testGridPosition = gridPosition + new Vector2Int(x, z);

                if (!GameControl.Instance.grid.BoundaryCheck(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<Vector2Int> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.y].
                Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Character selectedUnit = UnitActionSystem.Instance.GetSelectedUnit()?.GetComponent<Character>();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = selectedUnit.GetActionPoints() >= selectedAction.GetActionPointsCost() ? GridVisualType.Green : GridVisualType.Orange;
                break;
        }

        ShowGridPositionList(
            selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        Debug.Log("ACTION CHAHNGE");
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void LevelGrid_OnAnyStartAction(object sender, EventArgs e)
    {
        HideAllGridPosition();
    }
    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }

}
