using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{

    public static event EventHandler OnAnySwordHit;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;


    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    private int maxSwordDistance = 1;
    private State state;
    private float stateTimer;
    private GridObject targetUnit;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.GetComponent<Character>()?.Damage(100);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(Vector2Int gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<Vector2Int> GetValidActionGridPositionList()
    {
        List<Vector2Int> validGridPositionList = new List<Vector2Int>();

        Vector2Int unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                Vector2Int offsetGridPosition = new Vector2Int(x, z);
                Vector2Int testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GameControl.Instance.grid.BoundaryCheck(testGridPosition))
                {
                    continue;
                }

                if (!GameControl.Instance.grid.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty, no Unit
                    continue;
                }

                Character targetUnit = GameControl.Instance.grid.GetPlacedObject(testGridPosition)?.GetComponent<Character>();

                if (targetUnit.IsEnemy() == unit.GetComponent<Character>().IsEnemy())
                {
                    // Both Units on same 'team'
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(Vector2Int gridPosition, Action onActionComplete)
    {
        targetUnit = GameControl.Instance.grid.GetPlacedObject(gridPosition);

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }

}
