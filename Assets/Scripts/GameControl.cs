using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EventType
{
    INIT_GRID_OBJECT,
    MOVE_ACTION_OBJECT_TARGET,
    OnSelectedUnitChange,
    OnSelectedActionChanged,
    OnBusyChanged,
    OnActionStarted,
    OnAnyActionCompleted,
    OnSelectedUnitChanged,
    OnAnyUnitMovedGridPosition,
    OnAnyActionStarted,
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
    public Type EventArgsType; 
}

public struct HandlerEventDescriptor
{
    public Type ComponentType;
    public EventType GridEventType;
    public Action<object,object> Handler;
}

public struct EventQueue
{
    public EventType GridEventType;
    public object sender;
    public object args;
}

public class GameControl : MonoBehaviour
{
    [SerializeField] public Grid grid;
    [SerializeField] LayerMask TerrainMask;
    [SerializeField] GridObject selectedChatacter;

    public PathFinding pathFinding;

    private List<GridObject> GridObjectsList = new List<GridObject>();
    private List<GridObject> GridObjectsWaiting = new List<GridObject>();

    private List<EventDescriptor> GameEvents = new List<EventDescriptor>();
    private List<HandlerEventDescriptor> GameHandlers = new List<HandlerEventDescriptor>();

    private bool QuequeLock = false;
    private List<EventQueue> QuequeEvents = new List<EventQueue>();

    public static GameControl Instance {
        get;
        private set;
    }
 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            pathFinding = GetComponent<PathFinding>();  
        }
        else
        {
            Destroy(this);
        }
    }
 
    public void CreateEvent<T>(ref EventHandler<T> handler, EventType eventType, object gridObject, Action callback = null)
    {
        EventDescriptor Event = new EventDescriptor();
        Event.IdEvent = Guid.NewGuid();
        Event.EventObject = gridObject;
        Event.GridEventType = eventType;
        handler += (object sender, T args) =>
        {
         
            Guid guid = Event.IdEvent;
            HandleEvent(guid,sender, args);
            if (callback != null) callback();
            if (!QuequeLock)
            {
                QuequeEvents.Add(new EventQueue() { GridEventType = eventType, args = args, sender = sender });
            }
        };
        GameEvents.Add(Event);
    }
    public void CreateEvent(ref EventHandler handler, EventType eventType, object gridObject, Action callback = null)
    {
        EventDescriptor Event = new EventDescriptor();
        Event.IdEvent = Guid.NewGuid();
        Event.EventObject = gridObject;
        Event.GridEventType = eventType;
        handler += (object sender, EventArgs args) =>
        {
          
            Guid guid = Event.IdEvent;
            HandleEvent(guid, sender, args);
            if (callback != null) callback();
            if (!QuequeLock)
            {
                QuequeEvents.Add(new EventQueue() { GridEventType = eventType, args = args, sender = sender });
            }
        };
        GameEvents.Add(Event);
       
    }
    private void HandleEvent(Guid guid, object sender, object args)
    {
        EventDescriptor eventDescriptor = GameEvents.Find(x => x.IdEvent == guid);
        if(eventDescriptor.IdEvent == guid)
        {
            List<HandlerEventDescriptor> handlers = new List<HandlerEventDescriptor>();

            handlers = GameHandlers?
                .Where(handler => handler.GridEventType == eventDescriptor.GridEventType).ToList();

            foreach (var item in handlers)
            {
                Debug.Log("HANDLE INVOKED");
                item.Handler.Invoke(sender, args);
            }
        }
    }

    public void attachObject(GridObject gridObject)
    {
        if(!GridObjectsList.Contains(gridObject))
        {
            Debug.Log("GRID OBJECT HAS BEEN ATTACHED");
            GridObjectsList.Add(gridObject);
            GridObjectsWaiting.Add(gridObject);
            CreateEvent(
                ref gridObject.OnInitObject,EventType.INIT_GRID_OBJECT,
                gridObject,
                () => { GridObjectsWaiting.Remove(gridObject); }
            );
        }
    }

    public void OnInitGrid()
    {
        GridObject[] cloneList = new GridObject[GridObjectsList.Count];
        GridObjectsWaiting.CopyTo(cloneList);
        foreach (var obj in cloneList)
        {
            obj.Init();
        }

        pathFinding.Init();
        UnitActionSystem.Instance.Init(selectedChatacter);
        GridSystemVisual.Instance.Init();
        UnitActionSystemUI.Instance.Init();
        ActionBusyUI.Instance.Init();

        QuequeLock = true;
        QuequeEvents.Clear();


    }
    public void AddHandlerToGridControl(object sender, HandlerEventDescriptor handlerEventGrid)
    {
        GameHandlers.Add(handlerEventGrid);
        if(!QuequeLock)
        {
            foreach (var item in QuequeEvents)
            {
                if(item.GridEventType == handlerEventGrid.GridEventType)
                {
                    handlerEventGrid.Handler.Invoke(item.sender,item.args);
                }
            }
        }
        
    }
    public void UpdateTargetCharacter(GridObject gridObject)
    {
        selectedChatacter = gridObject;
    }
    public GridObject GetTargetCharacter()
    {
        return selectedChatacter;
    }

    public List<PathNode> FindPath(Vector2Int start, Vector2Int end)
    {
        return pathFinding.FindPath(start.x, start.y, end.x, end.y);
    }

}
