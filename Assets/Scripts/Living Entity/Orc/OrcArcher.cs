using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcArcher : Enemy
{
    public float arrowSpeed = 20.0f;

    [SerializeField] GameObject arrow;
    WeaponHolder _weaponHolder;

    private Transform arrowTransform;

    protected override void Start()
    {
        base.Start();
        _weaponHolder = GetComponent<WeaponHolder>();
    }

    public override void Dead()
    {
        base.Dead();
    }

    public override void Hitted(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.Hitted(damage, hitPoint, hitNormal);
    }


    public void InstantiateArrow()
    {
        GameObject arrowObject = Instantiate(arrow.gameObject, arrow.transform.position, arrow.transform.rotation);
        arrowTransform = arrowObject.transform;
        arrowTransform.SetParent(_weaponHolder.ep[(int)Define.EEquipParts.Right], false);
        Hitbox arrowHitbox = arrowObject.AddComponent<Hitbox>();

        _ec.weapon.GetComponent<Bow>().hitbox = arrowHitbox;

        _ec.weapon.Use(_ec.currentCombo, _ec._enemy.op);
    }

    public void Shoot()
    {
        if(_ec.currentTarget != null)
        {
            arrowTransform.SetParent(null);
            arrowTransform.rotation = Quaternion.LookRotation(_ec.currentTarget.position - arrowTransform.position);
            arrowTransform.GetComponent<Rigidbody>().AddForce((_ec.currentTarget.position - arrowTransform.position).normalized * arrowSpeed, ForceMode.Impulse);
            Destroy(arrowTransform.gameObject, 5f);
        }
    }

    #region animation event
    public override void EnableHitBox(int active)
    {
        base.EnableHitBox(active);
    }
    #endregion
}
