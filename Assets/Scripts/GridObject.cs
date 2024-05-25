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
        GameControl.Instance.attachObject(this);
    }
    private void Update()
    {
        Vector2Int newGridPosition = GameControl.Instance.grid.getGridPosition(transform.position);
        if (newGridPosition != position)
        {
            // Unit changed Grid Position
            Vector2Int oldGridPosition = position;
            position = newGridPosition;

            GameControl.Instance.grid.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public void Init()
    {
        position = grid.getGridPosition(transform.position);
        GetComponent<Character>().Init();
        grid.PlaceObject(this, position);
        Vector3 pos = grid.GetWorldPosition(position.x, position.y, true);
        transform.position = pos;
        OnInitObject?.Invoke(this,EventArgs.Empty);
    }


    public Vector2Int GetGridPosition() { return position; }
    public Vector3 GetWorldPosition() { return transform.position; }
    public PathNode GetGridPositionNode() { return new PathNode(position.x, position.y); }

}
