using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquipment : MonoBehaviour
{
    public int id;
    public Define.EEquipmentType equipmentType;

    public int dp;

    public int speed;
    public float attackCoolTime;

    public float recoveryStaminaCoolTime;
    public float recoveryStaminaAmount;

    private Player _player;
    private PlayerController _pc;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _pc = GetComponentInParent<PlayerController>();
    }

    public void Equip()
    {
        if(_player != null)
        {
            _player.dp += dp;

            _pc.speed += speed;
            _pc.sprintSpeed += speed;

            _pc.attackCoolTime -= attackCoolTime;

            _player.recoveryStaminaAmount += recoveryStaminaAmount;
            _player.recoveryStaminaCoolTime -= recoveryStaminaCoolTime;
        }
    }

    public void UnEquip()
    {
        if (_player != null)
        {
            _player.dp -= dp;

            _pc.speed -= speed;
            _pc.sprintSpeed -= speed;

            _pc.attackCoolTime += attackCoolTime;

            _player.recoveryStaminaAmount -= recoveryStaminaAmount;
            _player.recoveryStaminaCoolTime += recoveryStaminaCoolTime;
        }
    }
}
