using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossEvent : MonoBehaviour
{
    public Boss boss;
    public BossWall enterance, exit;
    public Bonfire bonfire;

    public Action StartBossFightAction;
    public Action EndBossFightAction;

    private Transform _target;

    public void Awake()
    {
        _target = GameObject.Find("Player").transform;

        // enterance로 들어갈때
        StartBossFightAction += () => StartBossFight();

        // 보스 해치울때
        EndBossFightAction += () => EndBossFight();

        if(bonfire != null)
            bonfire.isActive = false;
    }


    private void StartBossFight()
    {
        WorldUIController.instance.StartFightBoss(boss);

        GetComponentInChildren<EnemyController>().currentTarget = _target;
    }


    private void EndBossFight()
    {
        if(bonfire != null)
            bonfire.ActiveBonfire();

        if (enterance != null)
            enterance.gameObject.SetActive(false);

        if(exit != null)
            exit.gameObject.SetActive(false);

        WorldUIController.instance.EndFightBoss();
    }
}
