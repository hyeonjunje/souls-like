using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    public abstract void Enter(EnemyController enemy);

    public abstract AIState Tick(EnemyController enemy);

    public abstract void Exit(EnemyController enemy);
}
