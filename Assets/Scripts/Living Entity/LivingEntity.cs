using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour
{
    public Transform lockOnTransform;

    protected Animator _animator;

    protected readonly int _hashIsHitted = Animator.StringToHash("IsHitted");
    protected readonly int _hashIsDead = Animator.StringToHash("IsDead");


    protected virtual void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void Hitted(float damage)
    {
        _animator.SetTrigger(_hashIsHitted);
    }

    public virtual void Dead()
    {
        _animator.SetTrigger(_hashIsDead);
    }

    #region animation event
    public virtual void EnableHitBox(int active)
    {

    }
    #endregion
}
