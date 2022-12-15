using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<BaseWeapon> myWeapons;
    public List<ItemData> myWeaponsData;

    public ItemData utilItem;
    public int maxAmount;
    public int currentAmount;

    private WeaponHolder _weaponHolder;

    private void Start()
    {
        _weaponHolder = GameObject.Find("Player").GetComponent<WeaponHolder>();

        currentAmount = maxAmount;
    }


    public void AddItem(ItemData item)
    {
        switch(item.itemType)
        {
            case Define.EItemType.Equip:
                AddWeapon(item);
                break;
            case Define.EItemType.Utils:
                break;
        }
    }

    private void AddWeapon(ItemData item)
    {
        bool isNewWeapon = true;

        BaseWeapon weapon = Instantiate(item.itemPrefab, transform.position, Quaternion.identity).GetComponent<BaseWeapon>();
        _weaponHolder.HoldWeapon(weapon);

        foreach (BaseWeapon myWeapon in myWeapons)
            if (myWeapon == weapon)
                isNewWeapon = false;

        if (isNewWeapon)
        {
            _weaponHolder.HoldWeapon(weapon);
            myWeapons.Add(weapon);
            myWeaponsData.Add(item);
        }
        else
            Destroy(weapon.gameObject);
    }
}
