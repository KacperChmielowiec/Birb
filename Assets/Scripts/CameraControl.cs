using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float Sensitive = 1f;
    [SerializeField] float MouseSensitive = 1f;
    [SerializeField] UnityEngine.Transform TopRightBorder;
    [SerializeField] UnityEngine.Transform BottomLeftBorder;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;

    Vector3 input;
    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;

    }

    // Update is called once per frame
    void Update()
    {
        input.x = 0f;
        input.y = 0f;
        input.z = 0f;
        MoveCameraInput();
        MoveCamera();
        HandleRotation();
    }

    private void MoveCamera()
    {
        Vector3 position = transform.position;
        position += (input * Time.deltaTime);
        position.x = Math.Clamp(position.x, BottomLeftBorder.position.x, TopRightBorder.position.x);
        position.z = Math.Clamp(position.z, BottomLeftBorder.position.z, TopRightBorder.position.z);
        transform.position = position;
    }

    private void MoveCameraInput()
    {
        AxisInput();
        MouseInput();
        HandleZoom();
    }

    private void MouseInput()
    {
        if(Input.GetMouseButtonDown(1))
        {
            origin = Input.mousePosition;
        }
        if(Input.GetMouseButton(1))
        {
            Vector3 pos = Input.mousePosition;
            var rotateVector = transform.forward * pos.y + transform.right * pos.x;
            var Origin = transform.forward * origin.y + transform.right * origin.x;
            input -= (rotateVector - Origin) * MouseSensitive;

        }
    }

    private void AxisInput()
    {
        input.x -= Input.GetAxis("Horizontal") * Sensitive;
        input.z -= Input.GetAxis("Vertical") * Sensitive;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;
        cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }


}
