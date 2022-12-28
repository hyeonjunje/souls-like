using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float defaultDamage;
    public AudioClip ac;
    public float currentDamage { get; set; }

    // connect
    private Collider _bc;
    private AudioSource audioSource;

    private List<LivingEntity> charactersDamagedDuringThisCalculation = new List<LivingEntity>();

    private LayerMask targetLayer;

    private void Start()
    {
        _bc = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
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
            {
                if (audioSource != null && ac != null)
                    audioSource.PlayOneShot(ac);
                Destroy(gameObject);
            }
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
            Debug.Log("�ƾ�");
            if (character != null)
            {
                if (charactersDamagedDuringThisCalculation.Contains(character))
                    return;

                if (currentDamage == 0)
                    currentDamage = defaultDamage;

                charactersDamagedDuringThisCalculation.Add(character);

                // ���� ���̾�(�� ������ ���� �� �ް� ����)
                if(targetLayer != 1 << character.gameObject.layer)
                {
                    Debug.Log("���� �� ������ �ȵ�");
                    return;
                }

                character.Hitted(currentDamage);
                if (gameObject.tag == "Projectile")
                {
                    if (audioSource != null && ac != null)
                        audioSource.PlayOneShot(ac);
                    Destroy(gameObject);
                }
            }
        }
    }
}
