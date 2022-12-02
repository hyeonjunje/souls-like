using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

[RequireComponent(typeof(CharacterController), typeof(InputController), typeof(UnityEngine.InputSystem.PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public float speed;
    public float lookSensitivity;
    public float jumpHeight = 1.2f;
    [Tooltip("Jump coolTime. Useful when you keep jumping")]
    public float jumpCoolTime = 0.50f;
    [Tooltip("Free fall coolTime. Useful when going down stairs")]
    public float fallCoolTime = 0.15f;
    public float attackCoolTime = 1.0f;
    public float detectionRange = 20.0f;
    public float viewAngle = 150.0f;

    private BaseWeapon _weaponR;
    public BaseWeapon weaponR
    {
        get { return _weaponR; }
        set
        {
            _weaponR = value;

            if (_weaponR == null)
                _weaponR = standardWeapon;

            _weaponR?.Equip();
        }
    }

    private BaseWeapon _weaponL;
    public BaseWeapon weaponL
    {
        get { return _weaponL; }
        set
        {
            _weaponL = value;

            _weaponL?.Equip();
        }
    }

    [Header("Temp")]
    public BaseWeapon standardWeapon;
    public BaseWeapon tempWeaponR;
    public BaseWeapon tempWeaponL;
    public Button equipButton;


    [Header("Layer")]
    public LayerMask groundLayers;
    public LayerMask enemyLayers;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;

    [SerializeField] private Transform _cameraRoot;

    // connect
    private Player _player;
    private CharacterController _controller;
    private InputController _ic;
    private Animator _animator;
    private FieldOfView _fov;

    // player
    private float _currentSpeed = 0.0f;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _currentMoveX;
    private float _currentMoveY;

    // object
    private List<Transform> _objects => _fov.visibleTargets;
    private Transform _closedTarget = null;
    private Transform _currentTarget = null;

    // state
    private bool _isGround;
    private bool _isAttack => _attackTimer > 0;   // �������� ����
    private bool _isDefense;
    private bool _isAct => _isAttack || _isDefense;             // �̵��� �ƴ� � �ൿ�� �ϰ� �ִ� ����
    private bool _isTarget => _currentTarget != null;
    private bool _isLock = false;                           // �Ͽ� �� ����

    // gravity
    private float gravityValue = -9.81f * 2;
    public float groundRadius = 0.25f;

    // camera angle
    private float _cinemachineTargetYaw;     // y�� ȸ��
    private float _cinemachineTargetPitch;   // x�� ȸ��

    // CoolTime timer
    private float _jumpTimer;
    private float _fallTimer;
    private float _attackTimer;

    // coroutine
    private Coroutine _coFOVRoutine;

    // tweener
    private Tweener _attackCoolTimeTweener;
    private Tweener _moveSpeedTweener;
    private Tweener _moveXTweener;
    private Tweener _moveYTweener;
    private Tweener _lockRotationTweener;

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

    private void Awake()
    {
        _player = GetComponent<Player>();
        _controller = GetComponent<CharacterController>();
        _ic = GetComponent<InputController>();
        _animator = GetComponent<Animator>();
        _fov = _cameraRoot.GetComponent<FieldOfView>();
    }

    private void Start()
    {
        _cinemachineTargetYaw = _cameraRoot.rotation.eulerAngles.y;

        weaponR = null;
        weaponL = null;

        equipButton.onClick.AddListener(() =>
        {
            Debug.Log("����մϴ�.");
            weaponR = tempWeaponR;
            weaponL = tempWeaponL;
        });

        _moveSpeedTweener = DOTween.To(() => _currentSpeed, x => _currentSpeed = x, 0.0f, 0.0f).SetAutoKill(false).Pause();
        _attackCoolTimeTweener = DOTween.To(() => _attackTimer, x => _attackTimer = x, 0.0f, attackCoolTime).SetAutoKill(false).Pause();
        _moveXTweener = DOTween.To(() => _currentMoveX, x => _currentMoveX = x, 0.0f, 0.0f).SetAutoKill(false).Pause();
        _moveYTweener = DOTween.To(() => _currentMoveY, x => _currentMoveY = x, 0.0f, 0.0f).SetAutoKill(false).Pause();
        _lockRotationTweener = transform.DORotate(Vector3.zero, 0.5f).SetAutoKill(false).Pause();
    }

    private void Update()
    {
        JumpAndGravity();
        GroundCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    #region PlayerController

    private void Move()
    {
        float targetSpeed;

        Vector3 inputDirection = new Vector3(_ic.move.x, 0f, _ic.move.y).normalized;

        if(_ic.move != Vector2.zero)
        {
            targetSpeed = _ic.sprint ? 6.0f : 3.0f;
            if (_isAct) targetSpeed = 0f;

            _moveSpeedTweener.ChangeEndValue(targetSpeed, 0.5f, true).Restart();

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                _cameraRoot.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

            if(_isTarget)
                transform.LookAt(new Vector3(_currentTarget.position.x, 0f, _currentTarget.position.z));
            else
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        else
        {
            targetSpeed = 0.0f;
            if (_currentSpeed < 0.01f)
                _currentSpeed = 0;
            else
                _moveSpeedTweener.ChangeEndValue(targetSpeed, 0.5f, true).Restart();
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_currentSpeed * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime);

        if (_isTarget)
        {
            _moveXTweener.ChangeEndValue(_ic.move.x, 0.5f, true).Restart();
            _moveYTweener.ChangeEndValue(_ic.move.y, 0.5f, true).Restart();

            _animator.SetFloat(_hashVelocityX, _currentMoveX * _currentSpeed / 6f);
            _animator.SetFloat(_hashVelocityY, _currentMoveY * _currentSpeed / 6f);
        }
        else
        {
            _animator.SetFloat(_hashMove, _currentSpeed);
        }

        if(_isLock && _currentTarget != null && Vector3.Distance(_cameraRoot.position, _currentTarget.position) > _fov.viewRaduis)
        {
            LockOnOff();
        }
    }

    private void CameraRotation()
    {
        if (_isTarget)
        {
            _cameraRoot.LookAt(new Vector3(_currentTarget.position.x, 0f, _currentTarget.position.z));
            return;
        }
            

        if(_ic.look.magnitude > 0.01f)
        {
            _cinemachineTargetYaw += _ic.look.x * lookSensitivity * Time.deltaTime;
            _cinemachineTargetPitch += _ic.look.y * lookSensitivity * Time.deltaTime;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, -int.MaxValue, int.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -30f, 70f);

        _cameraRoot.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
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
        // ���� ���� ��
        if (_isGround)
        {
            _fallTimer = fallCoolTime;

            _animator.SetBool(_hashJump, false);
            _animator.SetBool(_hashInAir, false);
            // �߷°��ӵ��� ��� �����ϴ� ���� ���� �κ�
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if(_ic.jump && _jumpTimer <= 0.0f)
            {
                // ���� ���� �� �����ϸ� ����
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                _animator.SetBool(_hashJump, true);
            }

            if (_jumpTimer >= 0.0f)
                _jumpTimer -= Time.deltaTime;
        }
        // ���߿� ���� ��
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

    private void GroundCheck()
    {
        _isGround = Physics.CheckSphere(transform.position, groundRadius, groundLayers);
        _animator.SetBool(_hashIsGround, _isGround);
    }
    #endregion

    #region Hand
    private Coroutine _CoBlockStaminaDec;

    public void ActLeftHand(bool isLeftHand)
    {
        if(_isGround)
        {
            if (weaponL == null)
                return;

            _isDefense = isLeftHand;

            _animator.SetBool(_hashBlocking, isLeftHand);

            if(isLeftHand)
            {
                if (_CoBlockStaminaDec != null)
                    StopCoroutine(_CoBlockStaminaDec);
                _CoBlockStaminaDec = StartCoroutine(_player.CoEverDecresingStamina(-3f));
            }
            else
                if (_CoBlockStaminaDec != null)
                    StopCoroutine(_CoBlockStaminaDec);
        }
    }

    public void ActRightHand()
    {
        // Cant Attack while in the air 
        if(_isGround && _attackTimer <= 0.0f)
        {
            _attackTimer = attackCoolTime;

            _animator.SetTrigger(_hashIsAttack);
            weaponR?.Use();

            _attackCoolTimeTweener.ChangeEndValue(0.0f, attackCoolTime, true).Restart();

            _player.ChangeStamina(-5f);
        }
    }



    #endregion

    #region Target
    public void SetTarget(Transform target = null)
    {
        _currentTarget = target;
        _animator.SetBool(_hashIsTarget, _isTarget);
        if(target != null)
        {
            transform.LookAt(new Vector3(_currentTarget.position.x, 0f, _currentTarget.position.z));
        }
    }

    public void LockOnOff()
    {
        // enter lock state
        if (!_isLock)
        {
            _closedTarget = _fov.closedVisibleTarget;

            _currentTarget = _closedTarget;
            SetTarget(_currentTarget);
            _isLock = true;
        }
        // enter unlock state
        else
        {
            _closedTarget = null;

            _currentTarget = null;
            SetTarget(_currentTarget);
            _isLock = false;
        }
    }


    public void ChangeTarget(bool flag)
    {
        if(_isLock)
        {
            _objects.OrderBy(x => Vector3.Distance(transform.position, x.position));

            int index = _objects.IndexOf(_closedTarget);
            if (index == -1 && _objects.Count == 1)
                return;

            if (flag)
                index = (index + 1 == _objects.Count ? 0 : index + 1);
            else
                index = (index - 1 == -1 ? _objects.Count - 1 : index - 1);

            _closedTarget = _objects[index];

            _currentTarget = _closedTarget;

            SetTarget(_currentTarget);
        }
    }
    #endregion
}
