using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{

    private static MouseWorld instance;


    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private LayerMask unitsLayer;

    private void Awake()
    {
        instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
    public static Vector3 GetPositionUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.unitsLayer))
        {
            return raycastHit.point;
        }
        return GetPosition();
    }

}
