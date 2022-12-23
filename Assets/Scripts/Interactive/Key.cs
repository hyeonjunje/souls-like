using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    public float amount;

    Player player;

    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Player>();
    }

    public override void Use()
    {
        player.UsePotion(amount);

        playerUI.UtillSlotAmount(currentAmount);
    }
}
