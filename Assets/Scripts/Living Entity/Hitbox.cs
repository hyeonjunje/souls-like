using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float currentDamage { get; set; }

    // connect
    private BoxCollider _bc;

    private List<LivingEntity> charactersDamagedDuringThisCalculation = new List<LivingEntity>();

    private void Start()
    {
        _bc = GetComponent<BoxCollider>();
    }


    public void EnableHitBox(bool active)
    {
        _bc.enabled = active;

        if (!active)
        {
            if (charactersDamagedDuringThisCalculation.Count > 0)
                charactersDamagedDuringThisCalculation.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "Projectile" && other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Hitable"))
        {
            LivingEntity character = other.GetComponentInParent<LivingEntity>();

            if (character != null)
            {
                if (charactersDamagedDuringThisCalculation.Contains(character))
                    return;

                charactersDamagedDuringThisCalculation.Add(character);

                character.Hitted(currentDamage);
            }
        }
    }
}
