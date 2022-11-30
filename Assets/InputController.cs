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


    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

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
}
