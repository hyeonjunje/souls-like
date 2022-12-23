using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public bool isPermanence = false;
    public int maxAmount;
    public int currentAmount;


    public PlayerUI playerUI;

    public virtual void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
    }


    public abstract void Use();
}
