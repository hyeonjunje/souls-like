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
                pc.ChangeTarget(true);
            else if(_wheelValue < 0)
                pc.ChangeTarget(false);
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
                pc.LockOnOff();
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

            pc.ActLeftHand(_isLeftHand);
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
                pc.ActRightHand();
                _isRightHand = false;
            }
        }
    }


    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;


    private PlayerController pc;

    private bool locking;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
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
}
