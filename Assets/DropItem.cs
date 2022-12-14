using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour, IInteractable
{
    public ItemData item;

    public void EnterInteractZone()
    {
        Debug.Log("����");
    }

    public void ExitInteractZone()
    {
        Debug.Log("����");
    }

    public ItemData GetItemData()
    {
        return item;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public void Interact()
    {
        UIController.instance.ShowLootItemUI(item);
        switch(item.itemType)
        {
            case Define.EItemType.Equip:
                break;
            case Define.EItemType.Utils:
                break;
        }

        Destroy(gameObject);
    }
}
