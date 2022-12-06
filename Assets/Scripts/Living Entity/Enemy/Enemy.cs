using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private TargetMark _targetMark;

    public void SetTargetActive(bool active)
    {
        _targetMark.SetTargetActive(active);
    }
}
