using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField] private Hitbox hitParticle;

    private void Awake()
    {
        for (int i = 0; i < 51; i++)
        {
            Transform particle = Instantiate(hitParticle.gameObject, transform).transform;
            float angleBetweenPoints = 360 / 50;
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            particle.localRotation = Quaternion.LookRotation(direction);
        }
    }


}
