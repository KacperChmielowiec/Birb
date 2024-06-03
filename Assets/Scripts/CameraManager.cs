using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{


    [SerializeField] private CinemachineVirtualCamera actionCameraGameObject;
    [SerializeField] private CameraControl VirtualCamera;
    private Vector3 currPos, target;
    private bool isAnimate = false;
    private void Start()
    { 
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitMoveCamera;
        HideActionCamera();
    }
    private void Update()
    {
        if (isAnimate)
        {
            if (Vector3.Distance(currPos, target) > .1f)
            {
                VirtualCamera.transform.position = Vector3.MoveTowards(currPos, target, .5f);
                currPos = VirtualCamera.transform.position;

            }
            else
            {
                isAnimate = false;
            }
        }
    }
    private void ShowActionCamera()
    {
        actionCameraGameObject.gameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.gameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
      

        switch (sender)
        {

            case ShootAction shootAction:
                GridObject shooterUnit = shootAction.GetUnit();
                GridObject targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                Debug.Log("shoot camera action");
                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

                Vector3 actionCameraPosition =
                    shooterUnit.GetWorldPosition() +
                    cameraCharacterHeight +
                    shoulderOffset +
                    (shootDir * -1);

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);


                ShowActionCamera();
                break;
            case MoveAction moveAction:
                actionCameraGameObject.Follow = moveAction.GetUnit().transform;
                actionCameraGameObject.LookAt = moveAction.GetUnit().transform;
                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
            case MoveAction moveAction:
                HideActionCamera();
                VirtualCamera.transform.position = new Vector3(moveAction.GetUnit().position.x, VirtualCamera.transform.position.y, moveAction.GetUnit().position.y);
                actionCameraGameObject.Follow = null;
                actionCameraGameObject.LookAt = null;
                break;
        }
    }

    private void OnSelectedUnitMoveCamera(object sender, EventArgs e)
    {
        GridObject SelectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        currPos = VirtualCamera.transform.position;
        target = new Vector3(SelectedUnit.position.x, VirtualCamera.transform.position.y, SelectedUnit.position.y);
        isAnimate = true;
    }

    
}
