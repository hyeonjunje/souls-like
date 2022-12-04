using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Transform[] holdParts;
    [SerializeField] private Transform[] equipParts;

    public void HoldWeapon(BaseWeapon weapon)
    {
        if (weapon.holdParts == Define.EHoldParts.None)
            return;

        weapon.transform.SetParent(holdParts[(int)weapon.holdParts], false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localEulerAngles = Vector3.zero;
    }

    public void EquipWeapon(BaseWeapon weapon)
    {
        if (weapon.equipParts == Define.EEquipParts.None)
            return;

        weapon.transform.SetParent(equipParts[(int)weapon.equipParts], false);
        weapon.transform.localPosition = Vector3.zero + weapon.offsetPos;
        weapon.transform.localEulerAngles = Vector3.zero + weapon.offsetRot;
    }

    private void Update()
    {
        Debug.Log(equipParts[1].localEulerAngles);
    }
}
