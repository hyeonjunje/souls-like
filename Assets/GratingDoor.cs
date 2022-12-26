using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GratingDoor : MonoBehaviour, IInteractable
{

    public bool isActive = false;

    private Inventory _inventory;
    private Player _player;

    private void Start()
    {
        _inventory = GameObject.FindObjectOfType<Inventory>();
        _player = FindObjectOfType<Player>();
    }

    public void EnterInteractZone()
    {
        if (!isActive)
            UIController.instance.ShowInteractiveEnterText("����");
    }

    public void ExitInteractZone()
    {
        if (!isActive)
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
        if (!isActive)
        {
            if(_inventory.hasKey)
            {
                _inventory.hasKey = false;

                transform.DOLocalMoveY(transform.position.y + 10.0f, 3f);
                isActive = true;

                WorldSoundManager.instance.PlaySoundEffect(SE.DoorOpen);
            }
            else
            {
                UIController.instance.ShowInfoUI("���谡 �ʿ��մϴ�.");
            }

            UIController.instance.HideInteractiveExitText();
        }
    }
}
