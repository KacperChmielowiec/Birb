using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    Animator animator;
    bool move, attack;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Attack()
    {
        attack = true;
    }
    void StartMoving()
    {
        move = true;
    }
    void StopMoving()
    {
        move = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        if (attack) {
            attack = false;
        }
    }
}
