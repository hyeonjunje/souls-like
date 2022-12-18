using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    public float attackRange;
    public float attackAngle = 5f;
    public float attackCoolTime;
    public float hittedCoolTime;
    public float detachTime;

    public float rotationSpeed;

    public Transform offset;

    public FieldOfView _fov { get; private set; }
    public NavMeshAgent _agent { get; private set; }
    public Animator _animator { get; private set; }
    public Enemy _enemy { get; private set; }
    private WeaponHolder _weaponHolder;


    public AIState currentState;
    public AIState initState;
    public AIState hittedState;

    public Transform currentTarget;


    public int maxCombo = 0;
    public int currentCombo = 0;

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
            maxCombo = _weapon.maxCombo;
            currentCombo = 0;
        }
    }
    public BaseWeapon tempWeapon;

    protected virtual void Start()
    {
        _fov = GetComponent<FieldOfView>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _weaponHolder = GetComponent<WeaponHolder>();
        _enemy = GetComponent<Enemy>();

        ChangeState(initState);

        weapon = null;
    }

    protected virtual void Update()
    {
        if(!_enemy.isDead)
            Tick();
    }

    protected void Tick()
    {
        if(currentState != null)
        {
            AIState state = currentState.Tick(this);

            if(state != null)
            {
                if (state != currentState)
                    ChangeState(state);
            }
        }
    }


    private void ChangeState(AIState newState)
    {
        if(currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }

    public void Hitted()
    {
        ChangeState(hittedState);
    }
}
