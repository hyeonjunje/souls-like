using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    protected Animator _animator;

    protected int _hashIsHitted = Animator.StringToHash("IsHitted");  

    protected virtual void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void Hitted(float damage)
    {
        _animator.SetTrigger(_hashIsHitted);
    }

    #region animation event
    public virtual void EnableHitBox(int active)
    {

    }
    #endregion
}
