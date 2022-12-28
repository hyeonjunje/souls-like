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

    private LayerMask targetLayer;

    private void Start()
    {
        _bc = GetComponent<Collider>();

        if (GetComponentInParent<Player>() == null)
        {
            targetLayer = 1 << LayerMask.NameToLayer("Player");
        }
        else
        {
            targetLayer = 1 << LayerMask.NameToLayer("Enemy");
        }
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
        if (gameObject.tag == "Ignore")
            return;

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
            Debug.Log("아야");
            if (character != null)
            {
                if (charactersDamagedDuringThisCalculation.Contains(character))
                    return;

                if (currentDamage == 0)
                    currentDamage = defaultDamage;

                charactersDamagedDuringThisCalculation.Add(character);

                // 같은 레이어(적 끼리는 피해 안 받게 설정)
                if(targetLayer != 1 << character.gameObject.layer)
                {
                    Debug.Log("같은 적 때리기 안됨");
                    return;
                }

                character.Hitted(currentDamage);
            }
        }
    }
}
