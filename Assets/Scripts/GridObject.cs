using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
        int Max = GetComponent<Character>()?.Scope ?? 0;
        List<PathNode> scope = new List<PathNode>();
        for (int i = 0; i < Max; i++)
        {
            NextMoveScope(scope, i);
        }
        return scope;
    }

    private void NextMoveScope(List<PathNode> pathNodes, int step)
    {
        
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;
                if (grid.BoundaryCheck(position.x + step + i, position.y + step + j) == false || !grid.CheckWalkable(position.x + step + i, position.y + step + j))
                {
                    continue;
                }
                var NewPos = position + new Vector2Int(i + step, j + step);
                if(NewPos != null)
                    pathNodes.Add(grid.GetNode(NewPos));
            }
        }
        return;
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
