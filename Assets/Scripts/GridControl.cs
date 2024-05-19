using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum GridEventType
{
    INIT_GRID_OBJECT,
    MOVE_ACTION_OBJECT,
}
public enum GridObjectType
{
    TARGET,
    ANY
}

public struct EventDescriptorGrid
{
    public Guid IdEvent;
    public Type ComponentType;
    public GridObject GridObject;
    public GridEventType GridEventType;
}

public struct HandlerEventGrid
{
    public Type ComponentType;
    public GridEventType GridEventType;
    public GridObjectType GridObjectType;
    public Action<GridObject> Handler;
}

public class GridControl : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] LayerMask TerrainMask;
    [SerializeField] GridObject TargetObject;
    private List<GridObject> GridObjectsList = new List<GridObject>();
    private List<GridObject> GridObjectsWaiting = new List<GridObject>();

    private List<EventDescriptorGrid> EventsGrid = new List<EventDescriptorGrid>();
    private List<HandlerEventGrid> HandlersGrid = new List<HandlerEventGrid>();

    public static GridControl Instance {
        get;
        private set;
    }
    // Start is called before the first frame update

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
 
    private void CreateEvent(ref EventHandler handler, GridEventType eventType, GridObject gridObject, Action? callback = null)
    {
        EventDescriptorGrid gridEvent = new EventDescriptorGrid();
        gridEvent.IdEvent = Guid.NewGuid();
        gridEvent.GridObject = gridObject;
        gridEvent.GridEventType = eventType;
        handler += (object sender, EventArgs args) =>
        {
            Debug.Log("TRIGGERED");
            Guid guid = gridEvent.IdEvent;
            if(callback != null) callback();
            HandleEvent(guid);
        };
        EventsGrid.Add(gridEvent);
    }
    private void HandleEvent(Guid guid)
    {
        EventDescriptorGrid eventDescriptorGrid = EventsGrid.Find(x => x.IdEvent == guid);
        Debug.Log("HANDLE INVOKED");
        if(eventDescriptorGrid.IdEvent == guid)
        {
            List<HandlerEventGrid> handlers = new List<HandlerEventGrid>();
            if(eventDescriptorGrid.GridObject == TargetObject)
            {
                handlers = HandlersGrid.Where(h => h.GridObjectType == GridObjectType.TARGET).ToList();
                handlers = handlers?.Where(h => h.GridEventType == eventDescriptorGrid.GridEventType)?.ToList();
            }
            else
            {
                handlers = handlers?.Where(h => h.GridEventType == eventDescriptorGrid.GridEventType)?.ToList();
            }
            foreach (var handler in handlers)
            {
                handler.Handler.Invoke(eventDescriptorGrid.GridObject);
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
            //CreateInitGridObjectEvent(gridObject);
            CreateEvent(
                ref gridObject.OnInitObject,GridEventType.INIT_GRID_OBJECT,
                gridObject, 
                () => { GridObjectsWaiting.Remove(gridObject); }
            );
        }
    }
    public void attachMoveCharacter(MoveCharacter moveCharacter)
    {
        //CreateMoveStartGridObjectEvent(moveCharacter);
        CreateEvent(ref moveCharacter.OnUnitMoveStart, GridEventType.MOVE_ACTION_OBJECT, TargetObject);
    }

    public void OnInitGrid()
    {
        Debug.Log("INIT GRID COMPLETE");
        GridSystemVisual.Instance.Init();
        GridObject[] cloneList = new GridObject[GridObjectsList.Count];
        GridObjectsWaiting.CopyTo(cloneList);
        foreach (var obj in cloneList) 
        {
            obj.Init();
        }
    }
    public void AddHandlerToGridControl(object sender, HandlerEventGrid handlerEventGrid)
    {
        HandlersGrid.Add(handlerEventGrid);
    }

    public void AddStartMoveObjectTarget(Action<GridObject> callback, object sender)
    {
        var handler = new HandlerEventGrid();
        handler.Handler = callback;
        handler.GridEventType = GridEventType.MOVE_ACTION_OBJECT;
        handler.GridObjectType = GridObjectType.TARGET;
        handler.ComponentType = sender.GetType();

        HandlersGrid.Add(handler);
    }


    public void UpdateTargetCharacter(GridObject gridObject)
    {
        TargetObject = gridObject;
    }
    public GridObject GetTargetCharacter()
    {
        return TargetObject;
    }
}
