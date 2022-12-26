using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float defaultDamage;

    public float currentDamage { get; set; }

    // connect
    private Collider _bc;

    private List<LivingEntity> charactersDamagedDuringThisCalculation = new List<LivingEntity>();

    private void Start()
    {
        _bc = GetComponent<Collider>();
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
            if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
                Destroy(gameObject);
            else
                if (charactersDamagedDuringThisCalculation.Count > 0)
                    charactersDamagedDuringThisCalculation.Clear();

        }

        if(gameObject.tag == "Particle")
        {
            if (charactersDamagedDuringThisCalculation.Count > 0)
                charactersDamagedDuringThisCalculation.Clear();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Hitable"))
        {
            LivingEntity character = other.GetComponentInParent<LivingEntity>();

            if (character != null)
            {
                if (charactersDamagedDuringThisCalculation.Contains(character))
                    return;

                if (currentDamage == 0)
                    currentDamage = defaultDamage;

                charactersDamagedDuringThisCalculation.Add(character);

                character.Hitted(currentDamage);
            }
        }
    }
}
