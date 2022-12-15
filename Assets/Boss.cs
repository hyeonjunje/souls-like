using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string bossName;
    public BossWall enterance, exit;

    public Enemy enemy { get; set; }


    private void Start()
    {
        enemy = GetComponent<Enemy>();

        enterance.bossEnterAction += () => WorldUIController.instance.StartFightBoss(this);
    }


    public void DeadEvent()
    {
        Debug.Log("¿Ã∞≈ «œº¿");

        enterance.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);

        WorldUIController.instance.EndFightBoss();
    }
}
