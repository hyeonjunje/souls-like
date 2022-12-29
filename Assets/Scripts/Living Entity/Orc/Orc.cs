using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Dead()
    {
        base.Dead();
    }

    public override void Hitted(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.Hitted(damage, hitPoint, hitNormal);
    }

    #region animation event
    public override void EnableHitBox(int active)
    {
        base.EnableHitBox(active);
    }
    #endregion
}
