using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField] private Hitbox hitParticle;

    public int pointsCount;
    
    private void Awake()
    {
        for (int i = 0; i < pointsCount + 1; i++)
        {
            Transform particle = Instantiate(hitParticle.gameObject, transform).transform;
            float angleBetweenPoints = 360 / pointsCount;
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            particle.localRotation = Quaternion.LookRotation(direction);
        }
    }


}
