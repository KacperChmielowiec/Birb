using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float Sensitive = 1f;
    [SerializeField] float MouseSensitive = 1f;
    [SerializeField] Transform TopRightBorder;
    [SerializeField] Transform BottomLeftBorder;
    Vector3 input;
    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        input.x = 0f;
        input.y = 0f;
        input.z = 0f;
        MoveCameraInput();
        MoveCamera();
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
            input.x -= (pos.x - origin.x) * MouseSensitive;
            input.z -= (pos.y - origin.y) * MouseSensitive;
        }
    }

    private void AxisInput()
    {
        input.x -= Input.GetAxis("Horizontal") * Sensitive;
        input.z -= Input.GetAxis("Vertical") * Sensitive;
    }
}
