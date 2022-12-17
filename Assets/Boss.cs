using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string bossName;

    public Enemy enemy { get; set; }


    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }
}
