using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static Define;

public class Enemy : LivingEntity
{
    [Header("Enemy Info")]
    public EEnemyType enemyType;
    public float maxHp;
    public float recoveryHpCoolTime = 10.0f;
    [Tooltip("Amount of stamina restored per second")]
    public float recoveryHpAmount = 2.0f;

    [Header("Drop Item")]
    public ItemData[] dropItems;
    public DropItem drop;

    [Header("UI")]
    public Image hpBar;

    [SerializeField] private TargetMark _targetMark;

    private PlayerController _pc;

    private float _currentHp;
    public float currentHp
    {
        get { return _currentHp; }
        private set
        {
            _currentHp = value;

            _currentHp = Mathf.Clamp(_currentHp, 0, maxHp);

            hpBar.fillAmount = _currentHp / maxHp;

            if (_currentHp == 0)
                Dead();
        }
    }

    // connect

    // state
    public bool isDead => currentHp == 0;

    // timer
    private float _hpRecoveryTimer;
    // tweener
    private Tweener _hpCoolTimeTweener;


    protected override void Start()
    {
        base.Start();

        _pc = GameObject.FindObjectOfType<PlayerController>();

        if(enemyType != Define.EEnemyType.Boss)
            currentHp = maxHp;

        _hpCoolTimeTweener = DOTween.To(() => _hpRecoveryTimer, x => _hpRecoveryTimer = x, 0.0f, recoveryHpCoolTime).SetAutoKill(false).Pause();
    }

    private void Update()
    {
        if (isDead)
            return;

        RecoveryHp();
    }

    private void RecoveryHp()
    {
        if (_hpRecoveryTimer <= 0.01 && currentHp < maxHp)
        {
            ChangeHp(recoveryHpAmount * Time.deltaTime);
        }
    }

    public void SetTargetActive(bool active)
    {
        _targetMark.SetTargetActive(active);
    }

    public void SetHp(float value)
    {
        currentHp = value;
    }

    public void ChangeHp(float value)
    {
        if (value < 0)
        {
            _hpRecoveryTimer = recoveryHpCoolTime;
            _hpCoolTimeTweener.ChangeEndValue(0.0f, recoveryHpCoolTime, true).Restart();
        }

        currentHp += value;
    }

    #region override
    public override void Hitted(float damage)
    {
        if (isDead)
            return;

        ChangeHp(-damage);

        if (isDead)
            return;

        base.Hitted(damage);
    }

    public override void Dead()
    {
        base.Dead();

        if(dropItems.Length != 0)
        {
            DropItem dropObject = Instantiate(drop, transform.position + Vector3.up, Quaternion.identity);
            dropObject.item = dropItems[Random.Range(0, dropItems.Length)];
        }

        _targetMark.gameObject.layer = LayerMask.NameToLayer("DeadBody");

        if (_pc._currentTarget == _targetMark.transform)
            _pc.SetTarget(null);

        if(enemyType == Define.EEnemyType.Boss)
        {
            GetComponent<Boss>()?.DeadEvent();
        }
    }

    #endregion
}