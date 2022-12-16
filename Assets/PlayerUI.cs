using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Stat")]
    public Text opText;
    public Text dpText;

    [Header("WeaponSlot")]
    public Image weaponImage;
    public Text weaponSlotText;

    [Header("UtillSlot")]
    public Image utillImage;
    public Text amountText;

    public void SetWeaponSlot(ItemData item, int index)
    {
        weaponImage.sprite = item.itemSprite;
        weaponSlotText.text = (index + 1).ToString();
    }


    public void SetUtillSlot(ItemData item, int maxValue)
    {
        utillImage.sprite = item.itemSprite;
        amountText.text = maxValue.ToString(); ;
    }


    public void UtillSlotAmount(int amount)
    {
        amountText.text = amount.ToString();

        if(amount <= 0)
        {
            Color c = utillImage.color;
            c.a = 0.5f;
            utillImage.color = c;
        }
    }
}
