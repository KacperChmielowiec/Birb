using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool Passable;
    public GridObject GridObject;
    public float elevation;

    public bool HasAnyUnit()
    {
        return GridObject != null;
    }
    public void RemoveObject()
    {
        GridObject = null;
    }


}
