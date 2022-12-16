using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AIState
{
    public AIState ChaseState;

    private Coroutine _coFindTarget;

    public override void Enter(EnemyController enemy)
    {
        // idleState �� �� Ž�� 0.2�ʸ��� ����

        if (_coFindTarget != null)
            StopCoroutine(_coFindTarget);

        _coFindTarget = StartCoroutine(enemy._fov.FindTargetsWithDelay(0.2f));
    }

    public override void Exit(EnemyController enemy)
    {
        // Ž�� ����
        if (_coFindTarget != null)
            StopCoroutine(_coFindTarget);
    }

    public override AIState Tick(EnemyController enemy)
    {
        // �°��� idle���ٰ� �ٷ� chase�� ��ȯ
        if (enemy.currentTarget != null)
            return ChaseState;

        // �÷��̾ �����Ǹ� �i�� ���·� ��ȯ

        if (enemy._fov.closedVisibleTarget == null)
            return this;
        else
        {
            enemy.currentTarget = enemy._fov.closedVisibleTarget;

            return ChaseState;
        }
    }

    
}
