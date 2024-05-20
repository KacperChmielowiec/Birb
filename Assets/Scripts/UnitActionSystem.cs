using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{

    public static UnitActionSystem Instance { get; private set; }


    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;


    [SerializeField] private GridObject selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;
    private bool Inited = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        
    }
    public void Init()
    {
        SetSelectedUnit(selectedUnit);
        Inited = true;
    }

    private void Update()
    {
        if(!Inited) { return; }

        if (isBusy)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandleUnitSelection())
        {
            return;
        }

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Vector2Int mouseGridPosition = GameControl.Instance.grid.getGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (!selectedUnit.GetComponent<Character>().TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;
            }

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }


    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<GridObject>(out GridObject unit))
                {
                    if (unit == selectedUnit)
                    {
                        // Unit is already selected
                        return false;
                    }

                    if (unit.GetComponent<Character>().IsEnemy())
                    {
                        // Clicked on an Enemy
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }

    public void SetSelectedUnit(GridObject unit)
    {
        selectedUnit = unit;
        
        Character character = selectedUnit.GetComponent<Character>();

        SetSelectedAction(character.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public GridObject GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

}
