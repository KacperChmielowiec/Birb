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

    // Update is called once per frame
    void Update()
    {
        
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



}
