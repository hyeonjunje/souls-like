using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour, IInteractable
{
    public ItemData item;

    public void EnterInteractZone()
    {
        Debug.Log("µé¾î¿È");
    }

    public void ExitInteractZone()
    {
        Debug.Log("³ª°¨");
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public void Interact()
    {
        UIController.instance.ShowLootItemUI(item);
        Destroy(gameObject);
    }
}
