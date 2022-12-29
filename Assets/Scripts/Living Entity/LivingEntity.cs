using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class LivingEntity : MonoBehaviour
{
    public Transform lockOnTransform;

    public ParticleSystem hitParticleSystem;

    protected Animator _animator;
    protected CharacterSoundManager _characterSoundManager;

    protected readonly int _hashIsHitted = Animator.StringToHash("IsHitted");
    protected readonly int _hashIsDead = Animator.StringToHash("IsDead");


    protected virtual void Start()
    {
        _animator = GetComponent<Animator>();
        _characterSoundManager = GetComponent<CharacterSoundManager>();
    }

    public virtual void Hitted(float damage, Vector3 hitPoint, Vector3 hitNormal)
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
