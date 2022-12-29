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
        Transform enemyTransform = enemy._enemy.lockOnTransform;

        if (Vector3.Distance(enemyTransform.position, enemy.currentTarget.position) > enemy.viewRaduis)
        {
            if (enemy._enemy.enemyType == Define.EEnemyType.Common)
            {
                enemy.currentTarget = null;
                return idleState;
            }
        }

        if (enemy.currentTarget == null)
            return idleState;

        Vector3 dir = enemy.currentTarget.position - enemyTransform.position;
        dir.y = 0.0f;

        if(Vector3.Distance(enemyTransform.position, enemy.currentTarget.position) > enemy.attackRange)
        {
            if (!enemy._agent.enabled)
                enemy._agent.enabled = true;

            enemy._agent.SetDestination(enemy.currentTarget.position);
            enemy._animator.SetBool(_hashIsWalk, true);
            return this;
        }

        if (Physics.Raycast(enemyTransform.position, dir, Vector3.Distance(enemyTransform.position, enemy.currentTarget.position), (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Interactive"))))
        {
            if (!enemy._agent.enabled)
                enemy._agent.enabled = true;

            enemy._agent.SetDestination(enemy.currentTarget.position);
            enemy._animator.SetBool(_hashIsWalk, true);
            return this;
        }

        if (Vector3.Angle(dir, enemyTransform.forward) > enemy.attackAngle)
        {
            if (enemy._agent.enabled)
                enemy._agent.enabled = false;

            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);

            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
            enemy._animator.SetBool(_hashIsWalk, true);
            return this;
        }

        return attackState;
    }
}
