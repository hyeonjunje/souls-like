using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcElder : Enemy
{
    public float projectileSpeed;
    public Hitbox projectile;
    public Transform instantiateTransform;

    private Transform target;

    protected override void Start()
    {
        base.Start();

        lineRenderer = GetComponentInChildren<LineRenderer>();

        target = FindObjectOfType<Player>().lockOnTransform;

        lineRenderer.positionCount = pointsCount + 1;

        lineRenderer.gameObject.SetActive(false);
    }

    public override void Dead()
    {
        base.Dead();
    }

    public override void Hitted(float damage)
    {
        base.Hitted(damage);
    }

    #region animation event
    public override void EnableHitBox(int active)
    {
        base.EnableHitBox(active);
    }

    public void ShootRock()
    {
        GameObject proj = Instantiate(projectile.gameObject, instantiateTransform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody>().AddForce((target.position - proj.transform.position).normalized * projectileSpeed, ForceMode.Impulse);
    }

    public int pointsCount;
    private LineRenderer lineRenderer;
    public float maxRadius;
    public float shockwaveSpeed;
    public float startWidth;

    public void ShockWave()
    {
        StartCoroutine(CoShockWave());
    }

    private IEnumerator CoShockWave()
    {
        float currentRadius = 0f;

        lineRenderer.gameObject.SetActive(true);

        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * shockwaveSpeed;
            DrawShockwave(currentRadius);
            yield return null;
        }

        lineRenderer.gameObject.SetActive(false);
    }

    private void DrawShockwave(float currentRadius)
    {
        float angleBetweenPoints = 360f / pointsCount;

        for(int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            Vector3 point = direction * currentRadius;

            lineRenderer.SetPosition(i, point);
            lineRenderer.transform.GetChild(i).localPosition = point;
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }
    #endregion
}
