using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int pos_x;
    public int pos_y;

    public int gValue;
    public int hValue;

    public PathNode parent;
    public int fValue
    {
        get { return gValue + hValue; }
    }
    public PathNode(int xpos, int ypos)
    {
        this.pos_x = xpos;
        this.pos_y = ypos;
    }

    public override string ToString()
    {
        return $"x: {pos_x}; z: {pos_y}";
    }

}
