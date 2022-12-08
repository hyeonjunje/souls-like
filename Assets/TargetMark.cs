using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMark : MonoBehaviour
{
    [SerializeField] private TargetMarkLookAt _mark;
    private LivingEntity _livingEntity => transform.root.GetComponent<LivingEntity>();
    public LivingEntity livingEntity
    { get { return _livingEntity; } }

    public void SetTargetActive(bool active)
    {
        _mark.gameObject.SetActive(active);
    }
}
