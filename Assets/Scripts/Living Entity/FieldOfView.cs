using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)]
    public float viewAngle = 90f;
    public float viewRaduis = 10.0f;

    public LayerMask enemyLayer;
    public LayerMask blockLayer;

    //[HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    [HideInInspector]
    public Transform closedVisibleTarget;

    public Transform offsetTransform;

    public SpriteRenderer targetSprite;

    private void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        var wait = new WaitForSeconds(delay);
        while(true)
        {
            yield return wait;
            FindVisiableTargets();
        }
    }

    private void FindVisiableTargets()
    {
        visibleTargets.Clear();
        closedVisibleTarget = null;

        if (offsetTransform == null)
            offsetTransform = transform;

        Collider[] targetsInViewRadius = Physics.OverlapSphere(offsetTransform.position, viewRaduis, enemyLayer);

        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            if (target.tag != "Target")
                continue;

            Vector3 dirToTarget = GetDirToTarget(target);
            if(Vector3.Angle(offsetTransform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(offsetTransform.position, target.position);
                if(!Physics.Raycast(offsetTransform.position, dirToTarget, dstToTarget, blockLayer))
                {
                    visibleTargets.Add(target);

                    if (closedVisibleTarget == null || 
                        Vector3.Angle(offsetTransform.forward, GetDirToTarget(closedVisibleTarget)) > 
                        Vector3.Angle(offsetTransform.forward, GetDirToTarget(target)))
                        closedVisibleTarget = target;
                }
            }
        }
    }


    private Vector3 GetDirToTarget(Transform target)
    {
        return (target.position - offsetTransform.position).normalized;
    }



    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += offsetTransform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
