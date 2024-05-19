using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    public event EventHandler OnSelectedUnitChange;
    public EventHandler OnUnitMoveStart;
    [SerializeField] Grid grid;
    [SerializeField] LayerMask TerrainMask;
    [SerializeField] LayerMask UnitMask;
    GridObject targetCharacter;
    PathFinding PathFinding;
    List<PathNode> pathNodes = new List<PathNode>();

    public static MoveCharacter Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PathFinding = grid.GetComponent<PathFinding>();
           
        }
    }

    private void Start()
    {
        targetCharacter = GridControl.Instance.GetTargetCharacter();
        GridControl.Instance.attachMoveCharacter(this);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, UnitMask))
            {
                
                if(hit.transform.TryGetComponent(out GridObject unit))
                { 
                    targetCharacter = unit;
                    GridControl.Instance.UpdateTargetCharacter(unit);
                    OnSelectedUnitChange?.Invoke(this,null);
                }
            }
            else if (Physics.Raycast(ray, out hit, float.MaxValue, TerrainMask))
            {
                Vector2Int posGrid = grid.getGridPosition(hit.point);
                pathNodes = PathFinding.FindPath(targetCharacter.position.x , targetCharacter.position.y, posGrid.x, posGrid.y);
                OnUnitMoveStart?.Invoke(this,null);
                targetCharacter.GetComponent<Movement>().Move(pathNodes);
            }
        }
    }

 

    public GridObject GetSelectedGridObject()
    {
        return targetCharacter;
    }
}
