using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public Define.EWeaponType weaponType;
    public Define.EEquipParts equipParts;
    public Define.EHoldParts holdParts;
    public int maxOp, minOp;
    public int maxCombo;
    public Vector3 offsetPos;
    public Vector3 offsetRot;

    public abstract void Use();


    public abstract void Equip();

    public abstract void UnEquip();
}
