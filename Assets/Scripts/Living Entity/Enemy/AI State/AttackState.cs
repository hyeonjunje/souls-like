using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AIState
{
    public AIState idleState;
    public AIState ChaseState;

    private float _timer = 0.0f;

    private readonly int _hashIsAttack = Animator.StringToHash("IsAttack");

    public override void Enter(EnemyController enemy)
    {
        _timer = 0.0f;
        enemy._animator.SetTrigger(_hashIsAttack);
        enemy.weapon?.Use(enemy.currentCombo, 5f);

        enemy.currentCombo = enemy.currentCombo + 1 == enemy.maxCombo ? 0 : enemy.currentCombo + 1;
    }

    public override void Exit(EnemyController enemy)
    {

    }

    public override AIState Tick(EnemyController enemy)
    {
        if (Vector3.Distance(enemy.offset.position, enemy.currentTarget.position) > enemy._fov.viewRaduis)
        {
            enemy.currentTarget = null;
            return idleState;
        }

        if (Vector3.Distance(enemy.offset.position, enemy.currentTarget.position) > enemy.attackRange
            && Vector3.Angle(enemy.currentTarget.position - enemy.offset.position, enemy.offset.forward) > 5.0f)
        {
            return ChaseState;
        }
        else
        {
            _timer += Time.deltaTime;
            if (_timer >= enemy.attackCoolTime)
                return ChaseState;

            return this;
        }
    }
}
