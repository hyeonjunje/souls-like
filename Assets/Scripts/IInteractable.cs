using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public ItemData GetItemData();

    public Vector3 GetPos();

    public void EnterInteractZone();

    public void ExitInteractZone();

    public void Interact();
}
