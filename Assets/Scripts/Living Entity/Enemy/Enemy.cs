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
    public GameObject hpCanvas;

    private PlayerController _pc;
    private InputController _ic;
    private EnemyController _ec;

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

    public int op;
    public int dp;

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
        _ec = GetComponent<EnemyController>();
        _ic = GameObject.FindObjectOfType<InputController>();

        if (enemyType == Define.EEnemyType.Common)
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
    private Coroutine _coShowDamageText = null;
    public override void Hitted(float damage)
    {
        damage -= dp;
        if (damage < 0)
            damage = 0;

        if (isDead)
            return;

        ChangeHp(-damage);

        if (isDead)
            return;

        if(enemyType != EEnemyType.Boss)
            _ec.Hitted();


        if (enemyType != EEnemyType.Common)
        {
            if (_coShowDamageText != null)
                StopCoroutine(_coShowDamageText);
            _coShowDamageText = StartCoroutine(CoShowDamageText(damage));
        }
    }

    IEnumerator CoShowDamageText(float damage)
    {
        Text damageText = hpBar.transform.GetChild(0).GetComponent<Text>();

        damageText.gameObject.SetActive(true);
        damageText.text = damage.ToString();

        yield return new WaitForSeconds(1.5f);

        damageText.gameObject.SetActive(false);
    }

    public override void Dead()
    {
        base.Dead();

        if(dropItems.Length != 0)
        {
            DropItem dropObject = Instantiate(drop, transform.position + Vector3.up, Quaternion.identity);
            dropObject.item = dropItems[Random.Range(0, dropItems.Length)];
        }

        gameObject.layer = LayerMask.NameToLayer("DeadBody");

        if (_ic.lockOnFlag)
        {
            _ic.lockOnFlag = false;
            CameraHandler.instance.EndLockOn();
            CameraHandler.instance.ClearLockOnTargets();
            _pc.ActiveTargetAnim(_ic.lockOnFlag);
        }

        if (enemyType != Define.EEnemyType.Common)
        {
            GetComponentInParent<BossEvent>()?.EndBossFightAction.Invoke();
        }
    }

    #endregion

    #region animation event
    public override void EnableHitBox(int active)
    {
        bool flag = active == 1 ? true : false;
        _ec.weapon.EnableHitBox(flag);
    }
    #endregion
}
