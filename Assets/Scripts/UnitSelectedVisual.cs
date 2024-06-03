using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] GridObject unit;
    private MeshRenderer _MeshRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        _MeshRenderer = GetComponent<MeshRenderer>();
    }


    private void Start()
    {
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(typeof(UnitSelectedVisual), OnSelectedHandler, EventType.OnSelectedUnitChange));
        UpdateSelected();
    }

    void OnSelectedHandler(object sender, EventArgs empty)
    {
        UpdateSelected();
    }

    void UpdateSelected()
    {
        if (GameControl.Instance.GetTargetCharacter() == unit && unit != null)
        {
            _MeshRenderer.enabled = true;
        }
        else
        {
            _MeshRenderer.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        GameControl.Instance.DetachHandler(typeof(UnitSelectedVisual), EventType.OnSelectedUnitChange);
    }
}

