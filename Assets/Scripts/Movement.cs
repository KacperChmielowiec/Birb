using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    GridObject targetObject;
    List<Vector3> path;
    [SerializeField] float moveSpeed = 1f;
    internal void Move(List<PathNode> pathNodes)
    {
        if(pathNodes == null || pathNodes.Count == 0) return;
        path = targetObject.grid.ConvertPathToWorldPositions(pathNodes);
        targetObject.position.x = pathNodes[pathNodes.Count - 1].pos_x;
        targetObject.position.y = pathNodes[pathNodes.Count - 1].pos_y;
    }

    // Start is called before the first frame update
    void Awake()
    {
        targetObject = GetComponent<GridObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(path != null && path.Count > 0) {
            transform.position = Vector3.MoveTowards(transform.position,path[0],moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, path[0]) < 0.05f )
            {
                path.RemoveAt(0);
            }
        }
    }
}
