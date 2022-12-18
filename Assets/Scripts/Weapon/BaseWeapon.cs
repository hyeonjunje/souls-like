using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public Define.EWeaponType weaponType;
    public Define.EEquipParts equipParts;
    public Define.EHoldParts holdParts;
    public int op;
    public int maxCombo;
    public Vector3 offsetPos;
    public Vector3 offsetRot;
    public Hitbox hitbox;
    public SkillData[] skillDatas;

    private Player _player;

    private void Start()
    {
        _player = GetComponentInParent<Player>();
    }

    public abstract void Use(int currentCombo, float op);


    public virtual void Equip()
    {
        if (_player != null)
            _player.op += op;
    }

    public virtual void UnEquip()
    {
        if (_player != null)
            _player.op -= op;
    }

    public virtual void EnableHitBox(bool active)
    {
        hitbox.EnableHitBox(active);
    }
}
