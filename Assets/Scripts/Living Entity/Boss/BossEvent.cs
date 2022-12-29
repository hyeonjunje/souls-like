using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossEvent : MonoBehaviour
{
    public int bossNum;
    public Boss boss;
    public BossWall enterance, exit;
    public Bonfire bonfire;

    public Chair chair;

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

        if (chair != null)
            chair.isActive = false;
    }


    private void StartBossFight()
    {
        WorldUIController.instance.StartFightBoss(boss);
        WorldSoundManager.instance.ActiveBossBGM(true);
        GetComponentInChildren<EnemyController>().currentTarget = _target;
    }


    private void EndBossFight()
    {
        if(bonfire != null)
            bonfire.ActiveBonfire();

        if (chair != null)
            chair.ActiveChair();

        if (enterance != null)
            enterance.gameObject.SetActive(false);

        if(exit != null)
            exit.gameObject.SetActive(false);


        GameLogicManager.instance.isBossFight = false;
        WorldUIController.instance.EndFightBoss();
        WorldSoundManager.instance.ActiveBossBGM(false);

        DataManager.instance.isClearBoss[bossNum] = true;
        DataManager.instance.Save();
    }
}
