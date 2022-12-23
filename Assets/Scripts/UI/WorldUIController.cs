using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIController : MonoBehaviour
{
    public static WorldUIController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    public GameObject bossUI;
    public Image bossHpBar;
    public Text bossName;

    private Boss _currentBoss = null;

    public void StartFightBoss(Boss boss)
    {
        _currentBoss = boss;

        bossUI.SetActive(true);
        boss.enemy.hpBar = bossHpBar;
        boss.enemy.SetHp(boss.enemy.maxHp);
        bossName.text = boss.bossName;
    }

    public void EndFightBoss()
    {
        bossUI.SetActive(false);
        _currentBoss.enemy.hpBar = null;

        _currentBoss = null;
    }
}
