using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
            _animator.runtimeAnimatorController = _weaponR?.overrideController;
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

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;

    [SerializeField] private Transform _cameraRoot;

    // connect
    private CharacterController _controller;
    private InputController _ic;
    private Animator _animator;

    // player
    private float _currentSpeed = 0.0f;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;

    // state
    private bool _isGround;
    private bool _isAttack => _attackTimer > 0;   // 공격중일 상태
    private bool _isDefense;
    private bool _isAct => _isAttack || _isDefense;             // 이동이 아닌 어떤 행동을 하고 있는 상태

    // gravity
    private float gravityValue = -9.81f * 2;
    public float groundRadius = 0.25f;

    // camera angle
    private float _cinemachineTargetYaw;     // y축 회전
    private float _cinemachineTargetPitch;   // x축 회전

    // CoolTime timer
    private float _jumpTimer;
    private float _fallTimer;
    private float _attackTimer;

    // animation Hash
    private readonly int _hashMove = Animator.StringToHash("Speed");
    private readonly int _hashIsGround = Animator.StringToHash("IsGround");  
    private readonly int _hashJump = Animator.StringToHash("Jump");
    private readonly int _hashInAir = Animator.StringToHash("InAir");
    private readonly int _hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int _hashBlocking = Animator.StringToHash("Blocking");

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _ic = GetComponent<InputController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _cinemachineTargetYaw = _cameraRoot.rotation.eulerAngles.y;

        weaponR = null;
        weaponL = null;

        equipButton.onClick.AddListener(() =>
        {
            Debug.Log("장비합니다.");
            weaponR = tempWeaponR;
            weaponL = tempWeaponL;
        });
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

            DOTween.To(() => _currentSpeed, x => _currentSpeed = x, targetSpeed, 0.5f);

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                _cameraRoot.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        else
        {
            targetSpeed = 0.0f;
            if (_currentSpeed < 0.01f)
                _currentSpeed = 0;
            else
                DOTween.To(() => _currentSpeed, x => _currentSpeed = x, targetSpeed, 0.5f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_currentSpeed * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime);

        _animator.SetFloat(_hashMove, _currentSpeed);
    }

    private void CameraRotation()
    {
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

    private void GroundCheck()
    {
        _isGround = Physics.CheckSphere(transform.position, groundRadius, groundLayers);
        _animator.SetBool(_hashIsGround, _isGround);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_isGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            groundRadius);
    }
    #endregion

    #region Hand
    public void ActLeftHand(bool isLeftHand)
    {
        if(_isGround)
        {
            if (weaponL == null)
                return;

            _isDefense = isLeftHand;

            _animator.SetBool(_hashBlocking, isLeftHand);
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
            DOTween.To(() => _attackTimer, x => _attackTimer = x, 0.0f, attackCoolTime);
        }
    }



    #endregion
}
