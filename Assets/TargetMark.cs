using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMark : MonoBehaviour
{
    [SerializeField] private TargetMarkLookAt _mark;
    private LivingEntity _livingEntity => transform.GetComponentInParent<LivingEntity>();
    public LivingEntity livingEntity
    { get { return _livingEntity; } }

    public GameObject hpBar;

    public void SetTargetActive(bool active)
    {
        _mark.gameObject.SetActive(active);

        Enemy target = transform?.GetComponentInParent<Enemy>();
        if(target != null && target.enemyType == Define.EEnemyType.Common)
        {
            hpBar.SetActive(active);
        }
    }
}
