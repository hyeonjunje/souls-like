using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController), typeof(InputController), typeof(UnityEngine.InputSystem.PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public float speed;
    public float lookSensitivity;
    public float jumpHeight = 1.2f;
    public float jumpCoolTime = 0.50f;
    public float fallCoolTime = 0.15f;

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
    private bool IsGround;
    private float _verticalVelocity;

    // gravity
    private float gravityValue = -9.81f * 2;
    public float groundRadius = 0.25f;

    // camera angle
    private float _cinemachineTargetYaw;     // y�� ȸ��
    private float _cinemachineTargetPitch;   // x�� ȸ��

    // CoolTime timer
    private float _jumpTimer;
    private float _fallTimer;

    // animation Hash
    private readonly int _hashMove = Animator.StringToHash("Speed");
    private readonly int _hashIsGround = Animator.StringToHash("IsGround");  
    private readonly int _hashJump = Animator.StringToHash("Jump");
    private readonly int _hashInAir = Animator.StringToHash("InAir");

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _ic = GetComponent<InputController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _cinemachineTargetYaw = _cameraRoot.rotation.eulerAngles.y;
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

    private void Move()
    {
        float targetSpeed;

        Vector3 inputDirection = new Vector3(_ic.move.x, 0f, _ic.move.y).normalized;

        if(_ic.move != Vector2.zero)
        {
            targetSpeed = _ic.sprint ? 6.0f : 3.0f;
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
        IsGround = Physics.CheckSphere(transform.position, groundRadius, groundLayers);
        _animator.SetBool(_hashIsGround, IsGround);
        // ���� ���� ��
        if (IsGround)
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
        IsGround = Physics.CheckSphere(transform.position, groundRadius, groundLayers);
        _animator.SetBool(_hashIsGround, IsGround);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (IsGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            groundRadius);
    }
}
