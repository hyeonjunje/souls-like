using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : LivingEntity
{
    [Header("Player Info")]
    public float maxHp;
    public float recoveryHpCoolTime = 0.5f;
    [Tooltip("Amount of stamina restored per second")]
    public float recoveryHpAmount = 2.0f;

    [Header("UI")]
    [SerializeField] private Image hpBar;

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

    [SerializeField] private TargetMark _targetMark;

    protected override void Start()
    {
        base.Start();

        currentHp = maxHp;
    }

    public void SetTargetActive(bool active)
    {
        _targetMark.SetTargetActive(active);
    }

    public void ChangeHp(float value)
    {
        currentHp += value;
    }

    #region override
    public override void Hitted(float damage)
    {
        base.Hitted(damage);

        ChangeHp(-damage);
    }

    #endregion
}
