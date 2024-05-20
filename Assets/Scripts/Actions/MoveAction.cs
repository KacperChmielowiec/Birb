using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);

                ActionComplete();
            }
        }
    }


    public override void TakeAction(Vector2Int gridPosition, Action onActionComplete)
    {
        List<PathNode> pathGridPositionList = GameControl.Instance.FindPath(unit.GetGridPosition(), gridPosition);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (PathNode pathGridPosition in pathGridPositionList)
        {
            positionList.Add(GameControl.Instance.grid.GetWorldPosition(pathGridPosition.pos_x,pathGridPosition.pos_y,true));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override List<Vector2Int> GetValidActionGridPositionList()
    {
        List<Vector2Int> validGridPositionList = new List<Vector2Int>();

        Vector2Int unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                Vector2Int offsetGridPosition = new Vector2Int(x, z);
                Vector2Int testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GameControl.Instance.grid.BoundaryCheck(testGridPosition))
                {
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }

                if (GameControl.Instance.grid.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position already occupied with another Unit
                    continue;
                }

                if (!GameControl.Instance.grid.CheckWalkable(testGridPosition))
                {
                    continue;
                }

                if (!GameControl.Instance.pathFinding.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultiplier = 10;
                if (!(GameControl.Instance.pathFinding.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier))
                {
                    // Path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }


    public override string GetActionName()
    {
        return "Move";
    }


}
