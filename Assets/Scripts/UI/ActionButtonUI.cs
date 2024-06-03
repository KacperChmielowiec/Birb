using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ActionButtonUI : MonoBehaviour
{

    [SerializeField] private Text textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;
    private Action refreshAction;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedBaseAction == baseAction);
    }

}

