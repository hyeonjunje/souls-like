using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWeapon : BaseWeapon
{
    private List<Hitbox> magicHitboxs = new List<Hitbox>();

    private OrcElder orcElder;

    private void Start()
    {
        orcElder = GetComponentInParent<OrcElder>();
    }

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

    }
}
