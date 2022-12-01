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

    // Hold
    public bool _isLeftHand;
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

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }


    // "Move" Actions�� �ش��ϴ� Ű �Է� �� �ڵ� ȣ��
    void OnMove(InputValue value)  
    {
        move = value.Get<Vector2>();
    }

    // "Look" Actions�� �ش��ϴ� Ű �Է� �� �ڵ� ȣ��
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
}