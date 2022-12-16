using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittedState : AIState
{
    private float _timer = 0.0f;

    public AIState idleState;

    private readonly int _hashIsHitted = Animator.StringToHash("IsHitted");


    public override void Enter(EnemyController enemy)
    {
        if (enemy._fov.detectedPlayer != null && enemy.currentTarget == null)
            enemy.currentTarget = enemy._fov.detectedPlayer;

        if (enemy._enemy.enemyType == Define.EEnemyType.Boss)
            return;

        _timer = 0f;
        enemy._animator.SetTrigger(_hashIsHitted);
    }

    public override void Exit(EnemyController enemy)
    {

    }

    public override AIState Tick(EnemyController enemy)
    {
        if (enemy._enemy.enemyType == Define.EEnemyType.Boss)
            return idleState;

        _timer += Time.deltaTime;

        if (_timer >= enemy.hittedCoolTime)
            return idleState;

        return this;
    }
}
