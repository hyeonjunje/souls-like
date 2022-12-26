using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AIState
{
    public AIState idleState;
    public AIState ChaseState;

    private float _timer = 0.0f;

    private readonly int _hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int _hashCombo = Animator.StringToHash("Combo");

    private bool _attackable = true;

    public float _attackCooltime;

    public override void Enter(EnemyController enemy)
    {
        if(_attackable)
        {
            _timer = 0;

            enemy._animator.SetTrigger(_hashIsAttack);

            int index = GetSkillData(enemy);

            if(enemy._enemy.enemyType == Define.EEnemyType.Boss)
                enemy._animator.SetInteger(_hashCombo, index);

            enemy.weapon?.Use(index, enemy._enemy.op);

            _attackable = false;
        }
    }

    public override void Exit(EnemyController enemy)
    {

    }

    public override AIState Tick(EnemyController enemy)
    {
        Transform enemyTransform = enemy._enemy.lockOnTransform;

        if (Vector3.Distance(enemyTransform.position, enemy.currentTarget.position) > enemy.viewRaduis)
        {
            enemy.currentTarget = null;
            return idleState;
        }

        Vector3 dir = enemy.currentTarget.position - enemyTransform.position;
        dir.y = 0.0f;

        if (Vector3.Distance(enemyTransform.position, enemy.currentTarget.position) > enemy.attackRange
            && Vector3.Angle(dir, enemyTransform.forward) > enemy.attackAngle)
        {
            return ChaseState;
        }
        else
        {
            _timer += Time.deltaTime;
            if (_timer >= _attackCooltime)
            {
                _attackable = true;
                return ChaseState;
            }

            return this;
        }
    }

    public List<int> results;

    private int GetSkillData(EnemyController enemy)
    {
        float distance = Vector3.Distance(enemy._enemy.lockOnTransform.position, enemy.currentTarget.position);

        results = new List<int>();

        for (int i = 0; i < enemy.weapon.skillDatas.Length; i++)
        {
            SkillData skillData = enemy.weapon.skillDatas[i];
            if(distance >= skillData.minRange && distance <= skillData.maxRange)
            {
                results.Add(i);
            }
        }

        if (results.Count == 0)
        {
            _attackCooltime = enemy.weapon.skillDatas[enemy.weapon.skillDatas.Length - 1].coolTime;
            Debug.Log(_attackCooltime);
            return enemy.weapon.skillDatas.Length - 1;
        }
        else
        {
            int index = results[Random.Range(0, results.Count)];
            _attackCooltime = enemy.weapon.skillDatas[index].coolTime;
            Debug.Log(_attackCooltime);
            return index;
        }
    }
}
