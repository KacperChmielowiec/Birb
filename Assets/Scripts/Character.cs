using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Character : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 3;
    [SerializeField] public string NameChar = string.Empty;
    private int actionPoints = ACTION_POINTS_MAX;
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    private BaseAction[] baseActionArray;
    [SerializeField] private bool isEnemy;
    // Start is called before the first frame update
    void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
    }

    public void Init()
    {
        GameControl.Instance.CreateEvent(ref OnAnyActionPointsChanged, EventType.OnAnyActionPointsChanged, this);
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(this.GetType(), TurnSystem_OnTurnChanged, EventType.OnTurnChanged));
        OnAnyUnitSpawned.Invoke(this,EventArgs.Empty);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public BaseAction[] GetBaseActionsArray()
    {
        return baseActionArray;
    }

}
