using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;


    protected GridObject unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<GridObject>();
        
    }

    public virtual void Start()
    {
        
        GameControl.Instance.CreateEvent(ref OnAnyActionCompleted, EventType.OnAnyActionCompleted, this);
        GameControl.Instance.CreateEvent(ref OnAnyActionStarted, EventType.OnAnyActionStarted, this);
    }

    public abstract string GetActionName();

    public abstract void TakeAction(Vector2Int gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(Vector2Int gridPosition)
    {
        List <Vector2Int> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<Vector2Int> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
        Debug.Log("Any action complete");
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public GridObject GetUnit()
    {
        return unit;
    }


}
