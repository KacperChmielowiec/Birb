using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum EventType
{
    INIT_GRID_OBJECT,
    MOVE_ACTION_OBJECT_TARGET,
}
public enum GridObjectType
{
    TARGET,
    ANY
}

public struct EventDescriptor
{
    public Guid IdEvent;
    public Type ComponentType;
    public object EventObject;
    public EventType GridEventType;
}

public struct HandlerEventDescriptor
{
    public Type ComponentType;
    public EventType GridEventType;
    public Action<object> Handler;
}

public class GameControl : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] LayerMask TerrainMask;
    [SerializeField] GameObject TargetPrefarb;
    [SerializeField] List<GameObject> Teammates;
    [SerializeField] public PathFinding pathFinding;

    private List<GridObject> GridObjectsList = new List<GridObject>();
    private List<GridObject> GridObjectsWaiting = new List<GridObject>();

    private List<EventDescriptor> GameEvents = new List<EventDescriptor>();
    private List<HandlerEventDescriptor> GameHandlers = new List<HandlerEventDescriptor>();

    public static GameControl Instance {
        get;
        private set;
    }
 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
 
    private void CreateEvent(ref EventHandler handler, EventType eventType, object gridObject, Action callback = null)
    {
        EventDescriptor Event = new EventDescriptor();
        Event.IdEvent = Guid.NewGuid();
        Event.EventObject = gridObject;
        Event.GridEventType = eventType;
        handler += (object sender, EventArgs args) =>
        {
            Debug.Log("TRIGGERED");
            Guid guid = Event.IdEvent;
            HandleEvent(guid);
            if (callback != null) callback();
        };
        GameEvents.Add(Event);
    }
    private void HandleEvent(Guid guid)
    {
        EventDescriptor eventDescriptor = GameEvents.Find(x => x.IdEvent == guid);
        Debug.Log("HANDLE INVOKED");
        if(eventDescriptor.IdEvent == guid)
        {
            List<HandlerEventDescriptor> handlers = new List<HandlerEventDescriptor>();

            handlers = GameHandlers
                .Where(handler => handler.GridEventType == eventDescriptor.GridEventType).ToList();

            foreach (var item in handlers)
            {
                item.Handler.Invoke(eventDescriptor.EventObject);
            }
        }
    }

    public void attachObject(GridObject gridObject)
    {
        if(!GridObjectsList.Contains(gridObject))
        {
            Debug.Log("GRID OBJECT HAS BEEN ATTACHED");
            GridObjectsList.Add(gridObject);
            CreateEvent(
                ref gridObject.OnInitObject,EventType.INIT_GRID_OBJECT,
                gridObject,
                () => { GridObjectsWaiting.Remove(gridObject); }
            );
        }
    }

    public void OnInitGrid()
    {
        Debug.Log("INIT GRID COMPLETE");
        UnitActionSystem.Instance.Init();
        GridSystemVisual.Instance.Init();
        GridObject[] cloneList = new GridObject[GridObjectsList.Count];
        GridObjectsWaiting.CopyTo(cloneList);
        foreach (var obj in cloneList)
        {
            obj.Init();
        }
    }
    public void AddHandlerToGridControl(object sender, HandlerEventDescriptor handlerEventGrid)
    {
        GameHandlers.Add(handlerEventGrid);
    }
    public void UpdateTargetCharacter(GridObject gridObject)
    {
        UnitActionSystem.Instance.SetSelectedUnit(gridObject);
    }
    public GridObject GetTargetCharacter()
    {
        return UnitActionSystem.Instance.GetSelectedUnit();
    }

    public List<PathNode> FindPath(Vector2Int start, Vector2Int end)
    {
        return pathFinding.FindPath(start.x, start.y, end.x, end.y);
    }

}
