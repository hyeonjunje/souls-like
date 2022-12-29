using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using Cinemachine;


[RequireComponent(typeof(CharacterController), typeof(InputController), typeof(UnityEngine.InputSystem.PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public float speed = 3.0f;
    public float sprintSpeed = 6.0f;
    public float lookSensitivity;
    public float jumpHeight = 1.2f;
    [Tooltip("Jump coolTime. Useful when you keep jumping")]
    public float jumpCoolTime = 0.50f;
    [Tooltip("Free fall coolTime. Useful when going down stairs")]
    public float fallCoolTime = 0.15f;
    public float attackCoolTime = 1.0f;
    public float detectionRange = 26.0f;
    public float viewAngle = 150.0f;
    public float comboResetCoolTime = 3.0f;
    public AnimationCurve rollCurve;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;


    private BaseWeapon _weapon;
    public BaseWeapon weapon
    {
        get { return _weapon; }
        set
        {
            BaseWeapon prevWeapon = _weapon;

            _weapon = value;

            if (_weapon == null)
                return;
            _weapon.Equip();

            // parent
            if (prevWeapon != null)
            {
                prevWeapon.UnEquip();
                _weaponHolder.HoldWeapon(prevWeapon);
            }
            _weaponHolder.EquipWeapon(_weapon);

            // setting combo
            _maxCombo = _weapon.maxCombo;
            _currentCombo = 0;


            _animator.SetInteger(_hashEquipWeapon, (int)_weapon.weaponType);
        }
    }

    [Header("Layer")]
    public LayerMask groundLayers;
    public LayerMask enemyLayers;

    [Header("Connect")]


    // connect
    private Player _player;
    private CharacterController _controller;
    private InputController _ic;
    private Animator _animator;
    private WeaponHolder _weaponHolder;
    private Inventory _inventory;
    private PlayerUI _playerUI;
    private CameraHandler cameraHandler;

    // player
    private float _currentSpeed = 0.0f;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private Vector2 _currentMove;
    private int _maxCombo = 0;
    private int _currentCombo = 0;

    // state
    private bool _isGround;
    private bool _isAttack => _attackTimer > 0;   // 공격중일 상태
    private bool _isDefense;
    public bool isDefense => _isDefense;
    private bool _isAct => _isAttack || _isDefense || _isRoll || _isGathering;             // 이동 불가인 행동을 하고 있는 상태
    private bool _isRoll = false;
    private bool _isGathering = false;
    public bool isRoll => _isRoll;
    public bool isControllable = true;                                     // 보스방 입장같은 원격으로 transform을 조종할때는 조종 불가하게 하는 상태

    public bool isTimeline = false;

    // gravity
    private float gravityValue = -9.81f * 2;
    public float groundRadius = 0.25f;

    // CoolTime timer
    private float _jumpTimer;
    private float _fallTimer;
    private float _attackTimer;
    private float _comboResetTimer;
    private float _rollTimer;


    // tweener
    private Tweener _attackCoolTimeTweener;
    private Tweener _comboResetTimeTweener;
    private Tweener _moveSpeedTweener;
    private Tweener _moveTweener;

    // animation Hash
    private readonly int _hashMove = Animator.StringToHash("Speed");
    private readonly int _hashIsGround = Animator.StringToHash("IsGround");  
    private readonly int _hashJump = Animator.StringToHash("Jump");
    private readonly int _hashInAir = Animator.StringToHash("InAir");
    private readonly int _hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int _hashBlocking = Animator.StringToHash("Blocking");
    private readonly int _hashVelocityX = Animator.StringToHash("VelocityX");
    private readonly int _hashVelocityY = Animator.StringToHash("VelocityY");
    private readonly int _hashIsTarget = Animator.StringToHash("isTarget");
    private readonly int _hashCombo = Animator.StringToHash("Combo");
    private readonly int _hashEquipWeapon = Animator.StringToHash("EquipWeapon");
    private readonly int _hashRoll = Animator.StringToHash("Roll");
    private readonly int _hashGather = Animator.StringToHash("IsGather");
    

    private void Awake()
    {
        _player = GetComponent<Player>();
        _controller = GetComponent<CharacterController>();
        _ic = GetComponent<InputController>();
        _animator = GetComponent<Animator>();
        _weaponHolder = GetComponent<WeaponHolder>();
        _inventory = GetComponentInChildren<Inventory>();

        _playerUI = GameObject.FindObjectOfType<PlayerUI>();

        //cameraHandler = CameraHandler.instance;
    }

    private void Start()
    {
        cameraHandler = CameraHandler.instance;

        weapon = _inventory.myWeapons[0];

        _moveSpeedTweener = DOTween.To(() => _currentSpeed, x => _currentSpeed = x, 0.0f, 0.0f).SetAutoKill(false).Pause();
        _attackCoolTimeTweener = DOTween.To(() => _attackTimer, x => _attackTimer = x, 0.0f, attackCoolTime).SetAutoKill(false).Pause();
        _moveTweener = DOTween.To(() => _currentMove, x => _currentMove = x, Vector2.zero, 0.0f).SetAutoKill(false).Pause();
        _comboResetTimeTweener = DOTween.To(() => _comboResetTimer, x => _comboResetTimer = x, 0.0f, 0.0f).SetAutoKill(false).Pause()
            .OnComplete(() => _currentCombo = 0);

        Keyframe roll_lastFrame = rollCurve[rollCurve.length - 1];
        _rollTimer = roll_lastFrame.time;

        _playerUI.SetWeaponSlot(_inventory.myWeaponsData[0], 0);
    }

    private void Update()
    {
        if (!isControllable || isTimeline)
            return;

        if (_player.isDead)
            return;

        JumpAndGravity();

        Move();
        PlayerRotation();
    }


    #region PlayerController

    private void Move()
    {
        if (_isAct)
            return;

        float targetSpeed = 0.0f;

        Vector3 inputDirection = new Vector3(_ic.move.x, 0f, _ic.move.y).normalized;

        if (_ic.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                CameraHandler.instance.transform.eulerAngles.y;

            targetSpeed = _ic.sprint ? sprintSpeed : speed;
        }
        
        _moveSpeedTweener.ChangeEndValue(targetSpeed, 0.5f, true).Restart();

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_currentSpeed * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime);

        if (_ic.lockOnFlag)
        {
            _moveTweener.ChangeEndValue(new Vector2(_ic.move.x, _ic.move.y), 0.5f, true).Restart();

            _animator.SetFloat(_hashVelocityX, _ic.move.x * _currentSpeed / 6f);
            _animator.SetFloat(_hashVelocityY, _ic.move.y * _currentSpeed / 6f);
        }
        else
        {
            _animator.SetFloat(_hashMove, _currentSpeed);
        }
        _animator.SetFloat(_hashMove, _currentSpeed);


        

        if (_ic.lockOnFlag)
        {
            if(Vector3.Distance(transform.position, CameraHandler.instance.currentLockOnTarget.position) > detectionRange)
            {
                _ic.lockOnFlag = false;
                CameraHandler.instance.EndLockOn();
                CameraHandler.instance.ClearLockOnTargets();
                ActiveTargetAnim(_ic.lockOnFlag);
            }
        }
    }


    private void PlayerRotation()
    {
        if(!_ic.lockOnFlag)
        {
            if(_ic.move != Vector2.zero)
            {
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

                transform.eulerAngles = new Vector3(0f, rotation, 0f);
            }
        }
        else
        {
            if(_isRoll)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = cameraHandler.cameraTransform.forward * _ic.move.y;
                targetDirection += cameraHandler.cameraTransform.right * _ic.move.x;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if(targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSmoothTime);

                transform.rotation = tr;
            }
            else
            {
                Debug.Log(transform.rotation);
                Debug.Log(cameraHandler.transform.rotation);
                Debug.Log(rotationSmoothTime);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, cameraHandler.transform.rotation, rotationSmoothTime);
                transform.rotation = targetRotation;
            }
        }
    }


    private float ClampAngle(float angle, float minAngle, float maxAngle)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, minAngle, maxAngle);
    }


    private void JumpAndGravity()
    {
        if (_isAct)
            return;

        _isGround = Physics.CheckSphere(transform.position, groundRadius, groundLayers);
        _animator.SetBool(_hashIsGround, _isGround);
        // 땅에 있을 때
        if (_isGround)
        {
            _fallTimer = fallCoolTime;

            _animator.SetBool(_hashJump, false);
            _animator.SetBool(_hashInAir, false);
            // 중력가속도가 계속 감소하는 것을 막는 부분
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if(_ic.jump && _jumpTimer <= 0.0f)
            {
                // 땅에 있을 때 점프하면 점프
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                _animator.SetBool(_hashJump, true);
            }

            if (_jumpTimer >= 0.0f)
                _jumpTimer -= Time.deltaTime;
        }
        // 공중에 있을 때
        else
        {
            _jumpTimer = jumpCoolTime;

            if(_fallTimer >= 0.0f)
            {
                _fallTimer -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(_hashInAir, true);
            }
            _ic.jump = false;
        }
        _verticalVelocity += gravityValue * Time.deltaTime;
    }

    private Coroutine _coRoll;
    public void Roll()
    {
        if (!isControllable || isTimeline)
            return;

        if (!this.enabled)
            return;

        if (_isAct)
            return;

        if (!_isRoll && _isGround && _ic.move != Vector2.zero)
        {
            _isRoll = true;
            _coRoll = StartCoroutine(CoRoll());
        }
    }

    public void EndRoll()
    {
        _isRoll = false;

        if (_coRoll != null)
            StopCoroutine(_coRoll);
    }

    IEnumerator CoRoll()
    {
        if (_coRoll != null)
            StopCoroutine(_coRoll);

        float timer = 0;
        _animator.SetTrigger(_hashRoll);
        _player.ChangeStamina(-5f);

        _controller.center = new Vector3(0.0f, 0.5f, 0.0f);
        _controller.height = 1;

        float rotation = Mathf.Atan2(_ic.move.x, _ic.move.y) * Mathf.Rad2Deg + CameraHandler.instance.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));

        while (timer < _rollTimer)
        {
            timer += Time.deltaTime;
            float speed = rollCurve.Evaluate(timer);
            Vector3 dir = transform.forward * speed + Vector3.up * _verticalVelocity;
            _controller.Move(dir * Time.deltaTime);
            yield return null;
        }

        _controller.center = new Vector3(0.0f, 1.1f, 0.0f);
        _controller.height = 2;

        _isRoll = false;
    }
    #endregion

    #region Hand

    public void ChangeWeapon(int slot)
    {
        if (!isControllable || isTimeline)
            return;

        slot = slot - 1;
        if(_inventory.myWeapons.Count > slot && _inventory.myWeapons[slot] != null && weapon != _inventory.myWeapons[slot])
        {
            weapon = _inventory.myWeapons[slot];

            _playerUI.SetWeaponSlot(_inventory.myWeaponsData[slot], slot);
        }
    }

    public void ChangeUtilItem(int slot)
    {
        //slot = slot - 1;
        if (_inventory.myUtilItems.Count > slot && _inventory.myUtilItems[slot] != null && _inventory.currentUtilItem != _inventory.myUtilItems[slot])
        {
            _inventory.currentUtilItem = _inventory.myUtilItems[slot];
            _inventory.currentUtilSlot = slot;
            _playerUI.SetUtillSlot(_inventory.currentUtilItem, _inventory.utilItemCount[slot]);
        }
    }


    public void ActLeftHand(bool isLeftHand)
    {
        if (!isControllable || isTimeline)
            return;

        if (_player.currentStamina < 0)
            return;

        if(_isGround || weapon.weaponType != Define.EWeaponType.None)
        {
            _isDefense = isLeftHand;

            _animator.SetBool(_hashBlocking, isLeftHand);
        }
    }

    public void ActRightHand()
    {
        if(isTimeline)
        {
            // 로비창으로 돌아가기
            Debug.Log("메인메뉴로 갑니다.");
            isTimeline = false;

            GameManager.instance.ReturnMainMenu();
        }

        if (!isControllable)
            return;

        if (_player.currentStamina < 5.0f)
            return;

        // Can't Attack while in the air 
        if (_isGround && _attackTimer <= 0.0f)
        {
            _attackTimer = attackCoolTime;
            _comboResetTimer = comboResetCoolTime;

            _animator.SetTrigger(_hashIsAttack);
            _animator.SetInteger(_hashCombo, _currentCombo);

            weapon?.Use(_currentCombo, _player.op);

            _currentCombo = _currentCombo + 1 == _maxCombo ? 0 : _currentCombo + 1;

            _attackCoolTimeTweener.ChangeEndValue(0.0f, attackCoolTime, true).Restart();  // attack Cool Time Tweener
            _comboResetTimeTweener.ChangeEndValue(0.0f, comboResetCoolTime, true).Restart();   // combo Reset Cool Time Tweener

            _player.ChangeStamina(-5f);
        }
    }



    #endregion

    #region Target

    public void ActiveTargetAnim(bool active)
    {
        _animator.SetBool(_hashIsTarget, active);
    }
    #endregion

    #region Interact
    public void Interact()
    {
        if(_player.isInteract)
        {
            _player.closedInteractiveObject.Interact();
        }
    }

    public void AddItem(ItemData itemData)
    {
        _inventory.AddItem(itemData);
    }

    public void AnimationGathering()
    {
        _isGathering = true;
        _animator.SetTrigger(_hashGather);
        Invoke("SetFalseIsGathering", 2.0f);
    }

    private void SetFalseIsGathering()
    {
        _isGathering = false;
    }


    public void EnterWall()
    {
        _controller.Move(transform.forward * (3 * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime);
        _animator.SetFloat(_hashMove, 3);
    }

    public void WalkAnimation(float value)
    {
        _animator.SetFloat(_hashMove, value);
    }
    #endregion
}
