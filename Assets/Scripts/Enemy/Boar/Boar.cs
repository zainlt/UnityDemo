using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy
{
    protected virtual void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
    }

}
