using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class OrcElder : Enemy
{
    [Header("Skill 1")]
    public Hitbox magicPrefab;
    public float magicSpeed = 3f;

    [Header("Skill 2")]
    public Hitbox rockPliarPrefab;
    public float warningTime = 1f;
    public float pliarRaiseSpeed = 1f;
    public float pliarDuration = 2f;

    [Header("Skill 3")]
    public Hitbox chasingRocksPrefab;
    public float summonRange = 5f;
    public float maxCount = 3f;
    public float minCount = 1f;
    public float chaseSpeed = 3f;
    public float chaseFrequency = 1f;

    [Header("Skill 4")]
    public Hitbox shockWave;
    public float maxRadius;
    public float timeToMax;

    [Header("Skill 5")]
    public Rasor rasor;
    public Transform rasorParent;

    private MagicWeapon magicWeapon;
    public List<Hitbox> currentMagics = new List<Hitbox>();

    private Transform target;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<Player>().lockOnTransform;

        magicWeapon = GetComponentInChildren<MagicWeapon>();
    }

    public override void Dead()
    {
        base.Dead();
    }

    public override void Hitted(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.Hitted(damage, hitPoint, hitNormal);
    }

    #region animation event
    public override void EnableHitBox(int active)
    {
        base.EnableHitBox(active);
    }

    Tweener tween;

    private void InstantiateMagic()
    {
        currentMagics.Clear();

        Hitbox magicObject = Instantiate(magicPrefab);

        magicObject.currentDamage = op * magicWeapon.skillDatas[0].rate;

        magicObject.transform.SetParent(magicWeapon.transform);
        magicObject.transform.localPosition = Vector3.zero;

        tween = magicObject.transform.DOScale(Vector3.zero, 1).From();

        currentMagics.Add(magicObject);
    }

    private void ThrowMagic()
    {
        Debug.Log("마법구를 던진다.");
        tween.Kill();
        currentMagics[0].transform.SetParent(null);
        currentMagics[0].GetComponent<Rigidbody>().AddForce((target.position - currentMagics[0].transform.position).normalized * magicSpeed, ForceMode.Impulse);
    }


    private void SummonPilarOfFire()
    {
        Debug.Log("불기둥을 소환한다.");

        currentMagics.Clear();

        Vector3 spawnPos = new Vector3(target.position.x, transform.position.y - 7f, target.position.z);
        Hitbox pilarObject = Instantiate(rockPliarPrefab);
        pilarObject.currentDamage = op * magicWeapon.skillDatas[1].rate;
        pilarObject.transform.position = spawnPos;

        currentMagics.Add(pilarObject);

        Sequence seq = DOTween.Sequence().SetAutoKill(false).AppendInterval(warningTime).AppendCallback(() =>
        {
            pilarObject.transform.GetChild(0).gameObject.SetActive(false);
            Debug.Log(pilarObject.transform.GetChild(0).name);
        }).Append(pilarObject.transform.DOMoveY(pilarObject.transform.position.y + 10f, pliarRaiseSpeed))
        .AppendInterval(pliarDuration)
        .OnComplete(() => Destroy(pilarObject.gameObject));
    }



    private void SummonGuidedRocks()
    {
        Debug.Log("유도 마법구를 소환한다.");

        currentMagics.Clear();

        for(int i = 0; i < Random.Range(minCount, maxCount); i++)
        {
            Hitbox magicObject = Instantiate(chasingRocksPrefab);
            magicObject.currentDamage = op * magicWeapon.skillDatas[2].rate;
            magicObject.tag = "Untagged";
            currentMagics.Add(magicObject);

            Vector3 randomDirection = Random.insideUnitCircle * summonRange;
            randomDirection += transform.position;

            Debug.Log(randomDirection);

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, summonRange, 1);

            magicObject.transform.position = hit.position;

            magicObject.transform.DOMoveY(magicObject.transform.position.y + 10f, pliarRaiseSpeed).OnComplete(() => 
            {
                magicObject.tag = "Projectile";
                magicObject.GetComponent<ChasingRocks>().StartChase(chaseFrequency, chaseSpeed);
            });
        }
    }

    private void InstantiateShockwave()
    {
        currentMagics.Clear();

        Hitbox magicObject = Instantiate(shockWave);
        magicObject.currentDamage = op * magicWeapon.skillDatas[3].rate;
        magicObject.transform.SetParent(magicWeapon.transform);
        magicObject.transform.localPosition = Vector3.zero;

        tween = magicObject.transform.DOScale(Vector3.zero, 1).From();

        currentMagics.Add(magicObject);
    }

    private void ShockWave()
    {
        Debug.Log("밀어내는 충격파를 쏜다.");

        tween.Kill();
        currentMagics[0].transform.SetParent(null);

        tween = currentMagics[0].transform.DOScale(Vector3.one * maxRadius, timeToMax).OnComplete(() => 
        {
            tween.Kill();
            Destroy(currentMagics[0].gameObject);
        });
    }

    private Rasor rasorObject;

    private void InstantiateRasor()
    {
        currentMagics.Clear();

        rasorObject = Instantiate(rasor);
        rasorObject.transform.SetParent(rasorParent.transform);
        rasorObject.transform.localPosition = Vector3.zero;
        rasorObject.transform.localEulerAngles = Vector3.zero;


        tween = rasorObject.transform.DOScale(Vector3.zero, 1).From();

        Hitbox hitbox = rasorObject.transform.GetComponentInChildren<Hitbox>();
        hitbox.currentDamage = op * magicWeapon.skillDatas[4].rate;

        currentMagics.Add(hitbox);
    }

    private void Raser()
    {
        Debug.Log("레이저를 쏜다.");

        if (rasorObject != null)
        {
            rasorParent.transform.LookAt(target);
            rasorObject.ShootRasor();
        }
    }


    #endregion
}
