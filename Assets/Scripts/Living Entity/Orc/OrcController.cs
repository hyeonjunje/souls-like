using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class OrcController : MonoBehaviour
{
    [Header("Orc")]
    public float attackRange;
    public float attackCoolTime;
    public float hittedCoolTime;

    private BaseWeapon _weapon;
    public BaseWeapon weapon
    {
        get { return _weapon; }
        set
        {
            BaseWeapon prevWeapon = _weapon;

            _weapon = value;

            if (_weapon == null)
                _weapon = tempWeapon;

            _weapon.Equip();

            // parent
            if (prevWeapon != null)
                _weaponHolder.HoldWeapon(prevWeapon);
            _weaponHolder.EquipWeapon(_weapon);

            // setting combo
            _maxCombo = _weapon.maxCombo;
            _currentCombo = 0;
        }
    }
    public BaseWeapon tempWeapon;

    // orc
    private float _targetRotation;
    private float _rotationVelocity;
    private int _maxCombo = 0;
    private int _currentCombo = 0;

    // connect
    private FieldOfView _fov;
    private Animator _animator;
    private NavMeshAgent _agent;
    private WeaponHolder _weaponHolder;

    // target
    private Transform _target => _fov.closedVisibleTarget;
    private Transform _currentTarget = null;

    // state
    private bool _isAttack => _attackTimer > 0;
    private bool _isHitted => _hittedTimer > 0;
    private bool _isAct => _isAttack || _isHitted;

    // timer
    private float _attackTimer = 0.0f;
    private float _hittedTimer = 0.0f;
    private float _detachTimer = 0.0f;

    // tweener
    private Tweener _attackCoolTimeTweener;
    private Tweener _hittedCoolTimeTweener;

    // animation Hash
    private readonly int _hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int _hashIsWalk = Animator.StringToHash("IsWalk");
    private readonly int _hashIsHitted = Animator.StringToHash("IsHitted");

    private void Start()
    {
        _fov = GetComponent<FieldOfView>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _weaponHolder = GetComponent<WeaponHolder>();
        _attackCoolTimeTweener = DOTween.To(() => _attackTimer, x => _attackTimer = x, 0.0f, 0.0f).SetAutoKill(false).Pause();
        _hittedCoolTimeTweener = DOTween.To(() => _hittedTimer, x => _hittedTimer = x, 0.0f, 0.0f).SetAutoKill(false).Pause();

        weapon = null;
    }


    private void Update()
    {
        DetachTarget();
        Move();
        Attack();
    }

    private void DetachTarget()
    {
        if(_detachTimer <= 0.0f)
        {
            if (_currentTarget == null && _target != null)
            {
                _currentTarget = _target;
                _detachTimer = 5f;
                DOTween.To(() => _detachTimer, x => _detachTimer = x, 0.0f, 5.0f).SetAutoKill(false).OnComplete(() =>
                {
                    if (_target == null)
                        _currentTarget = null;
                });
            }
        }
    }

    private void Move()
    {
        if(_currentTarget != null)
        {
            Vector3 dir = _currentTarget.position - transform.position;

            _targetRotation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                0.3f);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            if(_isAct)
            {
                _agent.ResetPath();
                _animator.SetBool(_hashIsWalk, false);
                return;
            }
            else
            {
                _agent.SetDestination(_currentTarget.position);
                _animator.SetBool(_hashIsWalk, true);
            }
        }
        else
        {
            _agent.ResetPath();
            _animator.SetBool(_hashIsWalk, false);
        }
    }


    private void Attack()
    {
        if (_currentTarget != null)
        {
            Vector3 targetPos = _currentTarget.position - _currentTarget.localPosition;
            if(Vector3.Distance(transform.position, targetPos) < attackRange && Vector3.Angle(targetPos - transform.position, transform.forward) < 20.0f)
            {
                _animator.SetBool(_hashIsWalk, false);
                _agent.ResetPath();
                if (_attackTimer <= 0)
                {
                    _attackTimer = attackCoolTime;
                    _attackCoolTimeTweener.ChangeEndValue(0.0f, attackCoolTime, true).Restart();
                    _animator.SetTrigger(_hashIsAttack);

                    weapon?.Use(_currentCombo, 5);
                    _currentCombo = _currentCombo + 1 == _maxCombo ? 0 : _currentCombo + 1;
                }
            }
        }
    }
}
