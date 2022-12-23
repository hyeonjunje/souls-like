using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    private Image bossHpBar;
    private float maxHp;

    public void SetBossMaxHp(float value)
    {
        maxHp = value;
    }

    public void SetBossHp(float value)
    {
        bossHpBar.fillAmount = value / maxHp;
    }
}
