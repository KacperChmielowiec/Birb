using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GridObject : MonoBehaviour
{
    // Start is called before the first frame update
    public EventHandler OnInitObject;
    [SerializeField] public Grid grid;
    
    public Vector2Int position;
    void Start()
    {
        GridControl.Instance.attachObject(this);
    }

    public void Init()
    {
        position = grid.getGridPosition(transform.position);
        grid.PlaceObject(this, position);
        Vector3 pos = grid.GetWorldPosition(position.x, position.y, true);
        transform.position = pos;
        OnInitObject?.Invoke(this,EventArgs.Empty);
    }

    public List<PathNode> GetMoveScope()
    {
      
        List<PathNode> scope = new List<PathNode>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;
                if (grid.BoundaryCheck(position.x + i, position.y + j) == false)
                {
                    continue;
                }
                var NewPos = position + new Vector2Int(i,j);
                scope.Add(grid.GetNode(NewPos));
            }
        }
        return scope;
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
