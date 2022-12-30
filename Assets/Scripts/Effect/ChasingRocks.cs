using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingRocks : MonoBehaviour
{
    private Transform target;
    private Rigidbody rigid;

    public float duration = 6f;

    private void Start()
    {
        target = FindObjectOfType<Player>().lockOnTransform;
        rigid = GetComponent<Rigidbody>();

        Destroy(gameObject, duration);
    }

    private Coroutine _coChaseTarget;

    public void StartChase(float second, float speed)
    {
        if (_coChaseTarget != null)
            StopCoroutine(_coChaseTarget);
        _coChaseTarget = StartCoroutine(CoChaseTarget(second, speed));
    }

    IEnumerator CoChaseTarget(float seconds, float speed)
    {
        WaitForSeconds wait = new WaitForSeconds(seconds);

        while (true)
        {
            rigid.velocity = (target.position - rigid.transform.position).normalized * speed;
            yield return wait;
        }
    }

    private void OnDestroy()
    {
        if (_coChaseTarget != null)
            StopCoroutine(_coChaseTarget);


    }
}
