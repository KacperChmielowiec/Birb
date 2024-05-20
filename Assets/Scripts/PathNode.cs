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

    public override bool Equals(object obj)
    {
        return obj is PathNode position &&
               pos_x == position.pos_x &&
               pos_y == position.pos_y;
    }

    public bool Equals(PathNode other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(pos_x, pos_y);
    }

    public override string ToString()
    {
        return $"x: {pos_x}; z: {pos_y}";
    }

    public static bool operator == (PathNode a, PathNode b)
    {
        return a.pos_x == b.pos_y && a.pos_x == b.pos_y;
    }

    public static bool operator != (PathNode a, PathNode b)
    {
        return !(a == b);
    }

    public static PathNode operator + (PathNode a, PathNode b)
    {
        return new PathNode(a.pos_x + b.pos_y, a.pos_x + b.pos_y);
    }

    public static PathNode operator -(PathNode a, PathNode b)
    {
        return new PathNode(a.pos_x - b.pos_y, a.pos_x - b.pos_y);
    }

}
