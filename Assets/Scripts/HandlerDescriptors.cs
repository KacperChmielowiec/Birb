using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class HandlerDescriptors
{
    public static HandlerEventGrid GetInitObjectTargetDescriptor(this object sender, Action<GridObject> callback)
    {
        var handler = new HandlerEventGrid();
        handler.Handler = callback;
        handler.GridEventType = GridEventType.INIT_GRID_OBJECT;
        handler.GridObjectType = GridObjectType.TARGET;
        handler.ComponentType = sender.GetType();

        return handler;
    }
    public static HandlerEventGrid GetMoveTargetDescriptor(this object sender, Action<GridObject> callback)
    {
        var handler = new HandlerEventGrid();
        handler.Handler = callback;
        handler.GridEventType = GridEventType.MOVE_ACTION_OBJECT;
        handler.GridObjectType = GridObjectType.TARGET;
        handler.ComponentType = sender.GetType();

        return handler;
    }

}
