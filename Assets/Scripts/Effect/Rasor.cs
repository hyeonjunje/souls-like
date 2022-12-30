using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rasor : MonoBehaviour
{
    public Transform rasorLine;
    public float rasorSpeed;

    public float duration;

    private Coroutine _coRasor;

    private LivingEntity livingEntity;

    private void Start()
    {
        livingEntity = FindObjectOfType<Player>();
    }

    public void ShootRasor()
    {
        if (_coRasor != null)
            StopCoroutine(_coRasor);
        _coRasor = StartCoroutine(CoRasor());
    }

    IEnumerator CoRasor()
    {
        float timer = 0;
        
        while(true)
        {
            InfiniteLoopDetector.Run();
            timer += Time.deltaTime;

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, timer * rasorSpeed);
            rasorLine.localPosition = new Vector3(0, 0, 0.5f);

            if (timer > duration)
                break;

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_coRasor != null)
            StopCoroutine(_coRasor);
    }
}
