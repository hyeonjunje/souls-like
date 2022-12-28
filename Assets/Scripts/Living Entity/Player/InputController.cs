using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public bool lockOnFlag;

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

            if(lockOnFlag)
            {
                if (_wheelValue > 0)
                {
                    cameraHandler.HandleLockOn();
                    if (cameraHandler.rightLockTarget != null)
                    {
                        cameraHandler.EndLockOn();
                        cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                        cameraHandler.StartLockOn();
                    }
                }
                else if (_wheelValue < 0)
                {
                    cameraHandler.HandleLockOn();
                    if (cameraHandler.leftLockTarget != null)
                    {
                        cameraHandler.EndLockOn();
                        cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                        cameraHandler.StartLockOn();
                    }
                }
            }
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
            {
                if (lockOnFlag)
                {
                    lockOnFlag = false;
                    cameraHandler.EndLockOn();
                    cameraHandler.ClearLockOnTargets();
                }
                else
                {
                    cameraHandler.ClearLockOnTargets();
                    cameraHandler.HandleLockOn();

                    if(cameraHandler.nearestLockOnTarget != null)
                    {
                        cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                        lockOnFlag = true;

                        _pc.ActiveTargetAnim(lockOnFlag);

                        cameraHandler.StartLockOn();
                    }
                }
                _pc.ActiveTargetAnim(lockOnFlag);
            }

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

    private bool _isInteract;
    public bool isInteract
    {
        get { return _isInteract; }
        set
        {
            _isInteract = value;

            if (_isInteract)
            {
                _pc.Interact();
            }
        }
    }

    private bool _isUse;
    public bool isUse
    {
        get { return _isUse; }
        set
        {
            _isUse = value;

            if (_isUse)
            {
                _player.UseItem();
            }
        }
    }

    // connect
    private PlayerController _pc;
    private Player _player;
    private CameraHandler cameraHandler;
    private Inventory _inventory;

    private void Awake()
    {
        _pc = GetComponent<PlayerController>();
        _player = GetComponent<Player>();

        cameraHandler = CameraHandler.instance;

        _inventory = GetComponentInChildren<Inventory>();
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;

        if(cameraHandler != null && _pc.isControllable)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, look.x, look.y);
        }
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


    void OnInteract(InputValue value)
    {
        isInteract = value.isPressed;
    }


    void OnUse(InputValue value)
    {
        isUse = value.isPressed;
    }


    void OnUpArrow(InputValue value)
    {
        bool result = value.isPressed;
        Debug.Log(result);
        if (result)
        {
            if (_inventory.myUtilItems.Count <= 1)
                return;

            int index = _inventory.currentUtilSlot + 1;
            if (index >= _inventory.myUtilItems.Count)
                index = 0;
            _pc.ChangeUtilItem(index);
        }
    }


    void OnDownArrow(InputValue value)
    {
        bool result = value.isPressed;
        Debug.Log(result);
        if(result)
        {
            if (_inventory.myUtilItems.Count <= 1)
                return;

            int index = _inventory.currentUtilSlot - 1;
            if (index < 0)
                index = _inventory.myUtilItems.Count - 1;
            _pc.ChangeUtilItem(index);
        }
    }
}
