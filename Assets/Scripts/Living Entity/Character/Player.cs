using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntity
{
    [Header("Player Info")]
    public float maxHp;
    public float maxStamina;
    public float recoveryStaminaCoolTime = 0.5f;
    [Tooltip("Amount of stamina restored per second")]
    public float recoveryStaminaAmount = 5.0f;    
    

    [Header("UI")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image staminaBar;

    private float _currentHp;
    public float currentHp
    {
        get { return _currentHp; }
        private set
        {
            _currentHp = value;

            _currentHp = Mathf.Clamp(_currentHp, 0, maxHp);

            hpBar.fillAmount = _currentHp / maxHp;
        }
    }

    private float _currentStamina;
    public float currentStamina
    {
        get { return _currentStamina; }
        private set
        {
            _currentStamina = value;

            _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);

            staminaBar.fillAmount = _currentStamina / maxStamina;
        }
    }

    // connect
    private PlayerController _pc;

    private float _staminaRecoveryTimer;
    private Tweener _staimaCoolTimeTweener;
    protected override void Start()
    {
        base.Start();

        _pc = GetComponent<PlayerController>();

        currentHp = maxHp;
        currentStamina = maxStamina;

        _staimaCoolTimeTweener = DOTween.To(() => _staminaRecoveryTimer, x => _staminaRecoveryTimer = x, 0.0f, recoveryStaminaCoolTime).SetAutoKill(false).Pause();
    }


    private void Update()
    {
        RecoveryStamina();
    }


    public void ChangeHp(float value)
    {
        currentHp += value;
    }

    public void ChangeStamina(float value)
    {
        if (value < 0)
        {
            _staminaRecoveryTimer = recoveryStaminaCoolTime;
            _staimaCoolTimeTweener.ChangeEndValue(0.0f, recoveryStaminaCoolTime, true).Restart();
        }

        currentStamina += value;
    }

    public IEnumerator CoEverDecresingStamina(float value)
    {
        while(true)
        {
            if (currentStamina <= 0)
                break;

            ChangeStamina(value * Time.deltaTime);

            yield return null;
        }
    }

    private void RecoveryHp()
    {
        if (_staminaRecoveryTimer <= 0.01 && currentStamina < maxStamina)
        {
            ChangeHp(recoveryStaminaAmount * Time.deltaTime);
        }
    }

    private void RecoveryStamina()
    {
        if (_staminaRecoveryTimer <= 0.01 && currentStamina < maxStamina)
        {
            ChangeStamina(recoveryStaminaAmount * Time.deltaTime);
        }
    }

    #region override
    public override void Hitted(float damage)
    {
        // 회피중일 때
        if (_pc.isRoll)
            return;

        // 막았을 때
        if (_pc.isDefense)
        {
            if (currentStamina < damage)
            {
                damage -= currentStamina;
                ChangeStamina(-damage);
                _pc.ActLeftHand(false);
            }
            else
            {
                ChangeStamina(-damage);
                return;
            }
        }

        base.Hitted(damage);
        ChangeHp(-damage);
    }
    #endregion

    #region animation event
    public override void EnableHitBox(int active)
    {
        bool flag = active == 1 ? true : false;
        _pc.weapon.EnableHitBox(flag);
    }
    #endregion
}
