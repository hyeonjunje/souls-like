using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMark : MonoBehaviour
{
    [SerializeField] private TargetMarkLookAt _mark;

    public void SetTargetActive(bool active)
    {
        _mark.gameObject.SetActive(active);
    }
}
