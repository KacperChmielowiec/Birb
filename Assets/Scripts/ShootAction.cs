using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public GridObject targetUnit;
        public GridObject shootingUnit;
    }



    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    [SerializeField] private LayerMask obstaclesLayerMask;

    private State state;
    private int maxShootDistance = 7;
    private float stateTimer;
    private GridObject targetUnit;
    private bool canShootBullet;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
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
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        targetUnit.GetComponent<Character>()?.Damage(40);
    }



    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<Vector2Int> GetValidActionGridPositionList()
    {
        Vector2Int unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<Vector2Int> GetValidActionGridPositionList(Vector2Int unitGridPosition)
    {
        List<Vector2Int> validGridPositionList = new List<Vector2Int>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                Vector2Int offsetGridPosition = new Vector2Int(x, z);
                Vector2Int testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GameControl.Instance.grid.BoundaryCheck(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                if (!GameControl.Instance.grid.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty, no Unit
                    continue;
                }

                GridObject targetUnit = GameControl.Instance.grid.GetPlacedObject(testGridPosition);

                if (targetUnit?.GetComponent<Character>().IsEnemy() == unit.GetComponent<Character>()?.IsEnemy())
                {
                    // Both Units on same 'team'
                    continue;
                }

                Vector3 unitWorldPosition = GameControl.Instance.grid.GetWorldPosition(unitGridPosition.x,unitGridPosition.y,true);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDir,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask))
                {
                    // Blocked by an Obstacle
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

        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    public GridObject GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(Vector2Int gridPosition)
    {
        GridObject targetUnit = GameControl.Instance.grid.GetPlacedObject(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetComponent<Character>().GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(Vector2Int gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

}

