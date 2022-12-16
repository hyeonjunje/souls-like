using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : BaseWeapon
{

    public override void Equip()
    {
        base.Equip();
    }

    public override void UnEquip()
    {
        base.UnEquip();
    }


    public override void Use(int currentCombo, float op)
    {
        hitbox.currentDamage = op * skillDatas[currentCombo].rate;
    }
}
