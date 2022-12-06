using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrcController : MonoBehaviour
{
    [Header("Orc")]
    public float attackRange;

    [SerializeField] private GameObject _model;

    // orc
    private float _targetRotation;
    private float _rotationVelocity;

    // connect
    private FieldOfView _fov;
    private Animator _animator;
    private NavMeshAgent _agent;

    // target
    private Transform _target => _fov.closedVisibleTarget;

    // state
    private bool _isAttack = false;

    // animation Hash
    private readonly int _hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int _hashIsWalk = Animator.StringToHash("IsWalk");

    private void Start()
    {
        _fov = GetComponent<FieldOfView>();
        _animator = _model.GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        Move();
        Attack();

        Debug.Log(_target);
    }


    private void Move()
    {
        if (_isAttack)
        {
            _agent.ResetPath();
            return;
        }


        if (_target != null)
        {
            _agent.SetDestination(_target.position);
            _animator.SetBool(_hashIsWalk, true);

            Vector3 dir = _target.position - transform.position;

            _targetRotation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                0.3f);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
/*
            Quaternion targetRot = Quaternion.LookRotation(_target.position - transform.position);
            transform.rotation = */
        }
        else
        {
            _agent.ResetPath();
            _animator.SetBool(_hashIsWalk, false);
        }
    }


    private void Attack()
    {
        if(_target != null && Vector3.Distance(transform.position, _target.position) < attackRange)
        {
            _animator.SetTrigger(_hashIsAttack);
        }
    }
}
