using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
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
        if (currentAmount <= 0 || player.currentHp == player.maxHp)
            return;

        player.UsePotion(amount);

        currentAmount--;
        playerUI.UtillSlotAmount(currentAmount);
    }
}
