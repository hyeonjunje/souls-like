using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Chair : MonoBehaviour, IInteractable
{
    public bool isActive = false;
    public Transform sitPos;

    public void EnterInteractZone()
    {
        if (isActive)
            UIController.instance.ShowInteractiveEnterText("엔딩보기");
    }

    public void ExitInteractZone()
    {
        if (isActive)
            UIController.instance.HideInteractiveExitText();
    }

    public Vector3 GetPos()
    {
        if (isActive)
            return transform.position;
        else
            return Vector3.zero;
    }

    public void Interact()
    {
        if (isActive)
        {
            isActive = false;

            UIController.instance.HideInteractiveExitText();

            Player player = FindObjectOfType<Player>();
            player.SitChair(sitPos);
        }
    }


    public void ActiveChair()
    {
        isActive = true;

        transform.DOMoveY(transform.position.y + 1f, 2f);
    }
}
