using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _chestCap;
    [SerializeField] private ItemData itemData;
    public bool isActive = true;

    PlayerController _pc;
    private void Start()
    {
        _pc = FindObjectOfType<PlayerController>();
    }

    public void EnterInteractZone()
    {
        if (isActive)
            Debug.Log("E를 누르면 쉽니다.");
    }

    public void ExitInteractZone()
    {
        if (isActive)
            Debug.Log("나갑니다.");
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
            _pc.AnimationGathering();
            _chestCap.DOLocalRotate(new Vector3(0, 0, 0), 2);
            isActive = false;

            if(itemData == null)
            {

            }
            else
            {
                _pc.AddItem(itemData);
                UIController.instance.ShowLootItemUI(itemData);
            }
        }
    }
}
