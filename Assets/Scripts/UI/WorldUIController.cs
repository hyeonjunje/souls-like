using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WorldUIController : MonoBehaviour
{
    public static WorldUIController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [Header("Boss UI")]
    public GameObject bossUI;
    public Image bossHpBar;
    public Text bossName;

    private Image currentBossHpBar;

    [Header("World Name")]
    public Text worldNameText;


    private Boss _currentBoss = null;

    public void StartFightBoss(Boss boss)
    {
        _currentBoss = boss;

        bossUI.SetActive(true);
        currentBossHpBar = boss.enemy.hpBar;
        boss.enemy.hpBar = bossHpBar;
        boss.enemy.SetHp(boss.enemy.maxHp);
        bossName.text = boss.bossName;
    }

    public void EndFightBoss()
    {
        bossUI.SetActive(false);

        if(_currentBoss != null)
        {
            _currentBoss.enemy.hpBar = currentBossHpBar;

            _currentBoss = null;
        }
    }


    public void EnterOtherWorld(string worldName)
    {
        Sequence seq = DOTween.Sequence().OnPlay(() =>
        {
            WorldSoundManager.instance.PlaySoundEffect(SE.EnterTheNewWorld);
            worldNameText.gameObject.SetActive(true);
            worldNameText.text = worldName;
        }).Append(worldNameText.DOFade(1, 1).From(0))
        .AppendInterval(1f)
        .Append(worldNameText.DOFade(0, 1))
        .OnComplete(() =>
        {
            worldNameText.gameObject.SetActive(false);
        });
    }
}
