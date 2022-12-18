using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : AIState
{
    public AIState attackState;
    public AIState idleState;

    private float _targetRotation;
    private float _rotationVelocity;


    private readonly int _hashIsWalk = Animator.StringToHash("IsWalk");

    public override void Enter(EnemyController enemy)
    {

    }

    public override void Exit(EnemyController enemy)
    {
        if(enemy._agent.enabled)
            enemy._agent.ResetPath();
        enemy._animator.SetBool(_hashIsWalk, false);
    }

    public override AIState Tick(EnemyController enemy)
    {
        if (Vector3.Distance(enemy.offset.position, enemy.currentTarget.position) > enemy._fov.viewRaduis)
        {
            if (enemy._enemy.enemyType == Define.EEnemyType.Common)
            {
                enemy.currentTarget = null;
                return idleState;
            }
        }

        if (enemy.currentTarget == null)
            return idleState;

        Vector3 dir1 = enemy.currentTarget.position - enemy.offset.position;
        dir1.y = 0.0f;

        if (Vector3.Distance(enemy.offset.position, enemy.currentTarget.position) < enemy.attackRange
            && Vector3.Angle(dir1, enemy.offset.forward) < enemy.attackAngle)
            return attackState;
        else
        {
            // 이동
            if (Vector3.Distance(enemy.offset.position, enemy.currentTarget.position) > enemy.attackRange)
            {
                if (!enemy._agent.enabled)
                    enemy._agent.enabled = true;

                enemy._agent.SetDestination(enemy.currentTarget.position);
            }

            // 회전
            else if(Vector3.Angle(enemy.currentTarget.position - enemy.offset.position, enemy.offset.forward) > enemy.attackAngle)
            {
                if (enemy._agent.enabled)
                    enemy._agent.enabled = false;
                /*
                                Vector3 dir = enemy.currentTarget.position - enemy.offset.position;
                                _targetRotation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                                float rotation = Mathf.SmoothDampAngle(enemy.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, 15f);
                                enemy.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);*/

                Vector3 dir = enemy.currentTarget.position - enemy.transform.position;
                dir.y = 0.0f;

                Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);

                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
            }

            // 애니메이션
            enemy._animator.SetBool(_hashIsWalk, true);

            return this;
        }
    }
}
