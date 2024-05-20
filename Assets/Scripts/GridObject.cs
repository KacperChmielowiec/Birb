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
       
    }

    public void Init()
    {
        position = grid.getGridPosition(transform.position);
        grid.PlaceObject(this, position);
        Vector3 pos = grid.GetWorldPosition(position.x, position.y, true);
        transform.position = pos;
        OnInitObject?.Invoke(this,EventArgs.Empty);
    }


    public Vector2Int GetGridPosition() { return position; }
    public PathNode GetGridPositionNode() { return new PathNode(position.x, position.y); }
    // Update is called once per frame
    void Update()
    {
      
    }
}
