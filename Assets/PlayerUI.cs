using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("WeaponSlot")]
    public Image weaponImage;
    public Text weaponSlotText;

    public void SetWeaponSlot(ItemData item, int index)
    {
        weaponImage.sprite = item.itemSprite;
        weaponSlotText.text = (index + 1).ToString();
    }
}
