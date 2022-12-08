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


    public abstract void Use(int currentCombo, float op);


    public abstract void Equip();

    public abstract void UnEquip();

    public void EnableHitBox(bool active)
    {
        hitbox.EnableHitBox(active);
    }
}
