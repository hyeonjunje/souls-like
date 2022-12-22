using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcElder : Enemy
{
    public float projectileSpeed;
    public Hitbox projectile;
    public Transform instantiateTransform;

    private Transform target;

    protected override void Start()
    {
        base.Start();

        target = FindObjectOfType<Player>().lockOnTransform;
    }

    public override void Dead()
    {
        base.Dead();
    }

    public override void Hitted(float damage)
    {
        base.Hitted(damage);
    }

    #region animation event
    public override void EnableHitBox(int active)
    {
        base.EnableHitBox(active);
    }

    public void ShootRock()
    {
        GameObject proj = Instantiate(projectile.gameObject, transform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody>().AddForce((target.position - proj.transform.position).normalized * projectileSpeed, ForceMode.Impulse);
    }
    #endregion
}
