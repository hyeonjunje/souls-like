using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public Define.EWeaponType weaponType;
    public Define.EEquipParts equipParts;
    public AnimatorOverrideController overrideController;
    public int maxOp, minOp;


    public abstract void Use();


    public abstract void Equip();

    public abstract void UnEquip();
}
