using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance { get; private set; }


    private List<Character> unitList;
    private List<Character> friendlyUnitList;
    private List<Character> enemyUnitList;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        unitList = new List<Character>();
        friendlyUnitList = new List<Character>();
        enemyUnitList = new List<Character>();
    }

    private void Start()
    {
        Character.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Character.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Character unit = sender as Character;

        unitList.Add(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Character unit = sender as Character;

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Character> GetUnitList()
    {
        return unitList;
    }

    public List<Character> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }

    public List<Character> GetEnemyUnitList()
    {
        return enemyUnitList;
    }

}

