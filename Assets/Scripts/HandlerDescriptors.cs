using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class HandlerDescriptors
{
    public static Dictionary<EventType, HandlerEventDescriptor> Descriptors = new Dictionary<EventType, HandlerEventDescriptor>()
    {
        {EventType.OnSelectedUnitChange, new HandlerEventDescriptor() { GridEventType = EventType.OnSelectedUnitChange} },
        {EventType.OnAnyActionCompleted, new HandlerEventDescriptor() { GridEventType = EventType.OnAnyActionCompleted} },
        {EventType.OnSelectedActionChanged, new HandlerEventDescriptor() { GridEventType = EventType.OnSelectedActionChanged} },
        {EventType.OnAnyUnitMovedGridPosition, new HandlerEventDescriptor() { GridEventType = EventType.OnAnyUnitMovedGridPosition} },
        {EventType.OnAnyActionStarted, new HandlerEventDescriptor() { GridEventType = EventType.OnAnyActionStarted} }
    };

    public static HandlerEventDescriptor GetMemberDescriptor<T>(Type component, Action<object,T> action, EventType type)
    {
        HandlerEventDescriptor handlerEventDescriptor;
        Descriptors.TryGetValue(type, out handlerEventDescriptor);

        handlerEventDescriptor.ComponentType = component;
        handlerEventDescriptor.Handler = (object t1,object t2) => { action.Invoke(t1, (T)t2); };

        return handlerEventDescriptor;
    }

}
