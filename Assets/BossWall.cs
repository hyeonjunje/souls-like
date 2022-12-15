using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class BossWall : MonoBehaviour, IInteractable
{
    public EBossWallType bossWallType;

    public Action bossEnterAction;

    private bool _isBossFight;
    public bool isBossFight
    {
        get { return _isBossFight; }
        set
        {
            _isBossFight = value;

            if(_isBossFight)
            {
                bossEnterAction?.Invoke();
            }

        }
    }

    private BoxCollider _col;

    private void Start()
    {
        _col = GetComponent<BoxCollider>();
    }

    public void EnterInteractZone()
    {
        if (isBossFight)
            return;

        Debug.Log("E를 누르면 들어갑니다.");
    }

    public void ExitInteractZone()
    {
        if (isBossFight)
            return;
    }

    public ItemData GetItemData()
    {
        return null;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public void Interact()
    {
        if (isBossFight)
            return;

        EnterWall();
    }


    private void EnterWall()
    {
        // 입구일 때
        if(bossWallType == EBossWallType.Enterance)
        {
            StartCoroutine(CoEnterWall());
        }
        // 출구일 때
        else if(bossWallType == EBossWallType.Exit)
        {

        }
    }

    IEnumerator CoEnterWall()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        // 소리도 나야함

        pc.isControllable = false;

        _col.enabled = false;

        float timer = 0;
        pc.transform.rotation = transform.rotation;
        while(true)
        {
            InfiniteLoopDetector.Run();

            timer += Time.deltaTime;
            pc.EnterWall();

            if (timer >= 2f)
                break;

            yield return null;
        }

        _col.enabled = true;
        pc.isControllable = true;
        isBossFight = true;
    }
}
