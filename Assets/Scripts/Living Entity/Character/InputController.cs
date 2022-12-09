using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    private float _wheelValue;
    public float wheelValue
    {
        get { return _wheelValue; }
        set
        {
            _wheelValue = value;

            if (_wheelValue > 0)
                _pc.ChangeTarget(true);
            else if(_wheelValue < 0)
                _pc.ChangeTarget(false);
        }
    }


    private bool _isLockPressed;
    public bool isLockPressed
    {
        get { return _isLockPressed; }
        set
        {
            _isLockPressed = value;

            if (_isLockPressed)
                _pc.LockOnOff();
        }
    }

    // Hold
    private bool _isLeftHand;
    public bool isLeftHand
    {
        get { return _isLeftHand; }
        set
        {
            _isLeftHand = value;

            _pc.ActLeftHand(_isLeftHand);
        }
    }

    // Press
    private bool _isRightHand;
    public bool isRightHand
    {
        get { return _isRightHand; }
        set
        {
            _isRightHand = value;

            if (_isRightHand)
            {
                _pc.ActRightHand();
            }
        }
    }

    private bool _isRoll;
    public bool isRoll
    {
        get { return _isRoll; }
        set
        {
            _isRoll = value;

            if(_isRoll)
            {
                _pc.Roll();
            }
        }
    }
    
    // connect
    private PlayerController _pc;


    private void Awake()
    {
        _pc = GetComponent<PlayerController>();
    }


    // "Move" Actions에 해당하는 키 입력 시 자동 호출
    void OnMove(InputValue value)  
    {
        move = value.Get<Vector2>();
    }

    // "Look" Actions에 해당하는 키 입력 시 자동 호출
    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }


    void OnSprint(InputValue value)
    {
        sprint = value.isPressed;
    }


    void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }


    void OnLeftHand(InputValue value)
    {
        isLeftHand = value.isPressed;
        Debug.Log(isLeftHand);
    }


    void OnRightHand(InputValue value)
    {
        isRightHand = value.isPressed;
    }


    void OnLock(InputValue value)
    {
        isLockPressed = value.isPressed;
    }

    void OnWheel(InputValue value)
    {
        wheelValue = value.Get<float>();
    }

    void OnNum1(InputValue value)
    {
        if (value.isPressed)
            _pc.ChangeWeapon(1);
    }

    void OnNum2(InputValue value)
    {
        if (value.isPressed)
            _pc.ChangeWeapon(2);
    }

    void OnRoll(InputValue value)
    {
        isRoll = value.isPressed;
    }
}
