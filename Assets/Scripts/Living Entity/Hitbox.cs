using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float currentDamage { get; set; }

    // connect
    private BoxCollider _bc;

    private void Start()
    {
        _bc = GetComponent<BoxCollider>();
    }


    public void EnableHitBox(bool active)
    {
        _bc.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Target")
        {
            Debug.Log("¿Ã∞≈«‘ : " + other.name);
            other.GetComponent<TargetMark>().livingEntity.Hitted(currentDamage);
        }
    }
}
