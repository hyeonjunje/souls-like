using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AIState
{
    public AIState ChaseState;

    private Coroutine _coFindTarget;

    public override void Enter(EnemyController enemy)
    {
        // idleState 들어갈 시 탐색 0.2초마다 실행

        if (_coFindTarget != null)
            StopCoroutine(_coFindTarget);

        _coFindTarget = StartCoroutine(enemy._fov.FindTargetsWithDelay(0.2f));
    }

    public override void Exit(EnemyController enemy)
    {
        // 탐색 종료
        if (_coFindTarget != null)
            StopCoroutine(_coFindTarget);
    }

    public override AIState Tick(EnemyController enemy)
    {
        // 맞고나서 idle갔다가 바로 chase로 반환
        if (enemy.currentTarget != null)
            return ChaseState;

        // 플레이어가 감지되면 쫒는 상태로 반환

        if (enemy._fov.closedVisibleTarget == null)
            return this;
        else
        {
            enemy.currentTarget = enemy._fov.closedVisibleTarget;

            return ChaseState;
        }
    }

    
}
