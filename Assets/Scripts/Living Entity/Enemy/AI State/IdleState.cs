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

        _coFindTarget = StartCoroutine(CoFindTarget(enemy, 0.2f));
    }

    public override void Exit(EnemyController enemy)
    {
        // 탐색 종료
        if (_coFindTarget != null)
            StopCoroutine(_coFindTarget);
    }

    public override AIState Tick(EnemyController enemy)
    {
        if (enemy.currentTarget != null)
            return ChaseState;
        else
            return this;
    }

    IEnumerator CoFindTarget(EnemyController enemy, float delay)
    {
        var wait = new WaitForSeconds(delay);
        while (true)
        {
            FindVisiableTargets(enemy);
            yield return wait;
        }
    }


    private void FindVisiableTargets(EnemyController enemy)
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(enemy._enemy.lockOnTransform.position, enemy.viewRaduis, (1 << LayerMask.NameToLayer("Player")));

        Transform enemyTransform = enemy._enemy.lockOnTransform;

        enemy.detachedTarget = null;

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].GetComponent<LivingEntity>().lockOnTransform;

            if(target != null)
            {
                enemy.detachedTarget = target;

                Vector3 targetPos = target.position;
                Vector3 enemyPos = enemyTransform.position;

                targetPos.y = 0;
                enemyPos.y = 0;

                Vector3 dirToTargetWithoutY = (targetPos - enemyPos).normalized;

                Vector3 dirToTarget = (target.position - enemyTransform.position).normalized;
                if (Vector3.Angle(enemyTransform.forward, dirToTargetWithoutY) < enemy.viewAngle / 2
                    && Vector3.Angle(enemyTransform.forward, dirToTargetWithoutY) > -enemy.viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(enemyTransform.position, target.position);
                    if (!Physics.Raycast(enemyTransform.position, dirToTarget, dstToTarget, 1 << LayerMask.NameToLayer("Ground")))
                    {
                        enemy.currentTarget = target;
                    }
                }
            }
        }
    }
}
