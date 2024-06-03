using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UnitActionSystemUI : MonoBehaviour
{

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Image PointsBar;

    private List<ActionButtonUI> actionButtonUIList;
    public static UnitActionSystemUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        actionButtonUIList = new List<ActionButtonUI>();
    }

    public void Init()
    {
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(this.GetType(), UnitActionSystem_OnSelectedUnitChanged, EventType.OnSelectedUnitChange));
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(this.GetType(), UnitActionSystem_OnSelectedActionChanged, EventType.OnSelectedActionChanged));
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(this.GetType(), UnitActionSystem_OnActionStarted, EventType.OnActionStarted));
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(this.GetType(), TurnSystem_OnTurnChanged, EventType.OnTurnChanged));
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(this.GetType(), TurnSystem_OnTurnChanged, EventType.OnActionStarted));
        GameControl.Instance.AddHandlerToGridControl(this, HandlerDescriptors.GetMemberDescriptor<EventArgs>(this.GetType(), Unit_OnAnyActionPointsChanged, EventType.OnAnyActionPointsChanged));

        UpdateActionPoints();
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }


    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Character selectedUnit = UnitActionSystem.Instance.GetSelectedUnit()?.GetComponent<Character>();
        if (selectedUnit == null) return;
        foreach (BaseAction baseAction in selectedUnit?.GetBaseActionsArray())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }
    private void UnitActionSystem_OnSelectedUnitChanged()
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
        Character selectedUnit = UnitActionSystem.Instance.GetSelectedUnit()?.GetComponent<Character>();
        PointsBar.fillAmount = ((float)selectedUnit.GetActionPoints() / (float)selectedUnit.GetMaxActionPoints());
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        var selectedUnit = UnitActionSystem.Instance?.GetSelectedUnit();

        if(selectedUnit != null)
        {   var character = selectedUnit?.GetComponent<Character>();
            actionPointsText.text = "Action Points: " + character.GetActionPoints();
        }
            
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            actionButtonUIList.ForEach(b => b.gameObject.SetActive(false));
        }
        else
        {
            actionButtonUIList.ForEach(b => b.gameObject.SetActive(true));
        }
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
        Character selectedUnit = UnitActionSystem.Instance.GetSelectedUnit()?.GetComponent<Character>();
        if(selectedUnit != null)
        PointsBar.fillAmount = ((float)selectedUnit.GetActionPoints() / (float)selectedUnit.GetMaxActionPoints());
    }
}

