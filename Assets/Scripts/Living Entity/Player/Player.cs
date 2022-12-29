using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntity
{
    [Header("Player Info")]
    public float maxHp;
    public float maxStamina;
    public float recoveryStaminaCoolTime = 0.5f;
    [Tooltip("Amount of stamina restored per second")]
    public float recoveryStaminaAmount = 5.0f;
    public float invincibilityTime = 0.3f;


    [Header("UI")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image staminaBar;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem recoveryParticleSystem;

    private float _currentHp;
    public float currentHp
    {
        get { return _currentHp; }
        private set
        {
            _currentHp = value;

            _currentHp = Mathf.Clamp(_currentHp, 0, maxHp);

            hpBar.fillAmount = _currentHp / maxHp;

            if(_currentHp == 0)
            {
                Dead();
            }
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

    // interact
    public List<IInteractable> interactiveObjects = new List<IInteractable>();
    public IInteractable closedInteractiveObject
    {
        get
        {
            IInteractable result = null;
            foreach(var interactiveObject in interactiveObjects)
            {
                if (result == null)
                    result = interactiveObject;
                else
                    result = Vector3.Distance(transform.position, result.GetPos()) <= Vector3.Distance(transform.position, interactiveObject.GetPos()) ?
                        result : interactiveObject;
            }
            return result;
        }
    }

    private int _op;
    public int op
    {
        get { return _op; }
        set
        {
            _op = value;

            _playerUI.opText.text = _op.ToString();
        }
    }

    private int _dp;
    public int dp
    {
        get { return _dp; }
        set
        {
            _dp = value;

            _playerUI.dpText.text = _dp.ToString();
        }
    }

    // connect
    private PlayerController _pc;
    private InputController _ic;
    [SerializeField] private PlayerUI _playerUI;
    private Inventory _inventory;

    // state
    public bool isDead => currentHp == 0;
    public bool isInteract => closedInteractiveObject != null;

    // timer
    private float _staminaRecoveryTimer;

    // tweener
    private Tweener _staimaCoolTimeTweener;

    // animation Hash
    private int _hashIsRevive = Animator.StringToHash("isRevive");
    private int _hashSit = Animator.StringToHash("Sit");

    protected override void Start()
    {
        base.Start();

        _pc = GetComponent<PlayerController>();
        _ic = GetComponent<InputController>();
        _inventory = GetComponentInChildren<Inventory>();

        currentHp = maxHp;
        currentStamina = maxStamina;

        _staimaCoolTimeTweener = DOTween.To(() => _staminaRecoveryTimer, x => _staminaRecoveryTimer = x, 0.0f, recoveryStaminaCoolTime).SetAutoKill(false).Pause();

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        GameManager.instance.player = this;
        GameManager.instance.ConnectPlayer();

        Debug.Log(transform.position);
    }


    private void Update()
    {
        if (isDead)
            return;

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

    public void Revive()
    {
        _animator.SetTrigger(_hashIsRevive);
        ChangeHp(maxHp);
    }


    public void UseItem()
    {
        if (isDead)
            return;

        if (_inventory.currentUtilItem == null)
            return;

        int amount = _inventory.utilItemCount[_inventory.currentUtilSlot];

        if (_inventory.currentUtilItem.itemName == "포션")
        {
            if (amount <= 0 || currentHp == maxHp)
                return;

            recoveryParticleSystem.Play();
            ChangeHp(30);
        }

        amount = --_inventory.utilItemCount[_inventory.currentUtilSlot];
        _playerUI.UtillSlotAmount(amount);
    }

    public void UsePotion(float amount)
    {
        recoveryParticleSystem.Play();
        ChangeHp(amount);
    }


    public void RestInBonfire()
    {
        currentHp = maxHp;
        currentStamina = maxStamina;

        for(int i = 0; i < _inventory.myUtilItems.Count; i++)
        {
            if(_inventory.myUtilItems[i].itemName == "포션")
            {
                _inventory.utilItemCount[i] = 3;
                _playerUI.UtillSlotAmount(3);
            }
        }

        recoveryParticleSystem.Play();
    }

    public void SitChair(Transform sitTransform)
    {
        _pc.isTimeline = true;

        _pc.WalkAnimation(3);

        Sequence seq = DOTween.Sequence().Append(transform.DOMove(sitTransform.position, 2))
            .Join(transform.DORotate(sitTransform.eulerAngles, 2))
            .OnComplete(() => 
            {
                _animator.SetTrigger(_hashSit);
                TimelineController.instance.ShowEndingTimeline();
            });
    }

    private float invincibilityTimer = 0.0f;
    private bool isInvincibility = false;

    #region override
    public override void Hitted(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(isInvincibility)
        {
            return;
        }

        isInvincibility = true;
        DOTween.To(() => invincibilityTimer, x => invincibilityTimer = x, invincibilityTime, invincibilityTime)
                .OnComplete(() =>
                {
                    isInvincibility = false;
                    invincibilityTimer = 0.0f;
                });

        damage -= dp;
        if (damage < 0)
            damage = 0;

        if (isDead)
            return;

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

        _characterSoundManager.PlayRandomHitSound();

        // 카메라 흔들림
        CameraHandler.instance.ShakeCamera();

        ParticleSystem particleObject = Instantiate(hitParticleSystem);
        particleObject.transform.position = hitPoint;
        particleObject.transform.rotation = Quaternion.Euler(hitNormal);
        Destroy(particleObject.gameObject, 5f);

        ChangeHp(-damage);
        if (isDead)
            return;

        base.Hitted(damage, hitPoint, hitNormal);
    }


    public override void Dead()
    {
        if (_ic.lockOnFlag)
        {
            _ic.lockOnFlag = false;
            CameraHandler.instance.EndLockOn();
            CameraHandler.instance.ClearLockOnTargets();
            _pc.ActiveTargetAnim(_ic.lockOnFlag);
        }

        _pc.enabled = false;

        GameLogicManager.instance.isBossFight = false;
        WorldUIController.instance.EndFightBoss();
        WorldSoundManager.instance.ActiveBossBGM(false);

        base.Dead();

        GameLogicManager.instance.GameOver();

        DataManager.instance.deadCount++;
        DataManager.instance.Save();
    }

    #endregion

    #region animation event
    public override void EnableHitBox(int active)
    {
        bool flag = active == 1 ? true : false;
        _pc.weapon.EnableHitBox(flag);
    }


    public void EnablePlayerController()
    {
        _pc.enabled = true;
    }

    #endregion


    #region OnTrigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Interactive"))
        {
            IInteractable interactiveObject = other.GetComponent<IInteractable>();
            interactiveObject.EnterInteractZone();
            interactiveObjects.Add(interactiveObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactive"))
        {
            IInteractable interactiveObject = other.GetComponent<IInteractable>();
            interactiveObject.ExitInteractZone();
            interactiveObjects.Remove(interactiveObject);
        }
    }
    #endregion
}
