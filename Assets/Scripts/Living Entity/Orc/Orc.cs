using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    private OrcController _oc;

    protected override void Start()
    {
        base.Start();

        _oc = GetComponent<OrcController>();
    }

    public override void Dead()
    {
        base.Dead();

        _oc.ResetPath();
    }

    #region animation event
    public override void EnableHitBox(int active)
    {
        bool flag = active == 1 ? true : false;
        _oc.weapon.EnableHitBox(flag);
    }
    #endregion
}
