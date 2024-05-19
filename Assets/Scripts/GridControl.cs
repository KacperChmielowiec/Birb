using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GridEventType
{
    INIT_GRID_OBJECT
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

    private static GridControl _instance;
    public static GridControl Instance {
        get { if (_instance == null) _instance = new GridControl(); return _instance; } 
        private set { _instance = value; } 
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
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnDrawGizmos()
    {
        
  
    }

    private void CreateInitGridObjectEvent(GridObject gridObject)
    {
        EventDescriptorGrid gridEvent = new EventDescriptorGrid();
        gridEvent.IdEvent = Guid.NewGuid();

        gridObject.OnInitObject += (object sender, EventArgs args) =>
        {
            Guid guid = gridEvent.IdEvent;
            GridObjectsWaiting.Remove(gridObject);
            HandleEvent(guid);
        };

        EventsGrid.Add(gridEvent);
    }

    private void HandleEvent(Guid guid)
    {
        EventDescriptorGrid eventDescriptorGrid = EventsGrid.Find(x => x.IdEvent == guid);
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
            GridObjectsList.Add(gridObject);
            GridObjectsWaiting.Add(gridObject);
            CreateInitGridObjectEvent(gridObject);
        }
    }

    public void OnInitGrid()
    {
        GridSystemVisual.Instance.Init();
        foreach (var obj in GridObjectsWaiting)
        {
            obj.Init();
        }
    }
}
