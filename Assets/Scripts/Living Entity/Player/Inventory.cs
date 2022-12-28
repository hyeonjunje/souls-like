using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    public Transform equipParent;
    public Transform rootM;

    public List<BaseWeapon> myWeapons;
    public List<ItemData> myWeaponsData;

    public List<ItemData> myUtilItems;
    public ItemData currentUtilItem;
    public List<int> utilItemCount;
    public int currentUtilSlot;

    public bool hasKey = false;

    public ItemData[] equipmentData = new ItemData[(int)EEquipmentType.Size];

    public BaseEquipment[] currentEquipment = new BaseEquipment[(int)EEquipmentType.Size];
    public BaseEquipment[] baseEquipment = new BaseEquipment[(int)EEquipmentType.Size];

    public List<BaseEquipment> allEquipment;

    private WeaponHolder _weaponHolder;
    private PlayerUI _playerUI;

    private void Start()
    {
        _weaponHolder = GameObject.Find("Player").GetComponent<WeaponHolder>();
        _playerUI = FindObjectOfType<PlayerUI>();

        for(int i = 0; i < currentEquipment.Length; i++)
        {
            currentEquipment[i] = baseEquipment[i];

            if (currentEquipment[i] != null)
            {
                currentEquipment[i].gameObject.SetActive(true);
                currentEquipment[i].Equip();
            }
        }

        for (int i = 0; i < DataManager.instance.currentItem.Count; i++)
        {
            SetItem(DataManager.instance.currentItem[i]);
        }
    }
    private void SetItem(ItemData item)
    {
        WorldSoundManager.instance.PlaySoundEffect(SE.PickUpItem);

        switch (item.itemType)
        {
            case Define.EItemType.Weapon:
                AddWeapon(item);
                break;
            case Define.EItemType.Equip:
                AddEquipment(item);
                break;
            case Define.EItemType.Utils:
                AddUtilItem(item);
                break;
        }
    }

    public void AddItem(ItemData item)
    {
        WorldSoundManager.instance.PlaySoundEffect(SE.PickUpItem);

        switch (item.itemType)
        {
            case Define.EItemType.Weapon:
                AddWeapon(item);
                break;
            case Define.EItemType.Equip:
                AddEquipment(item);
                break;
            case Define.EItemType.Utils:
                AddUtilItem(item);
                break;
        }
        DataManager.instance.currentItem.Add(item);
        DataManager.instance.Save();
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


    private void AddEquipment(ItemData item)
    {
        BaseEquipment equip = null;

        foreach(BaseEquipment e in allEquipment)
        {
            if(e.id == item.itemId)
            {
                equip = e;
                break;
            }
        }

        if(equip != null)
        {
            if (currentEquipment[(int)equip.equipmentType] != null)
            {
                currentEquipment[(int)equip.equipmentType].UnEquip();

                currentEquipment[(int)equip.equipmentType].gameObject.SetActive(false);

                if (equip.equipmentType == EEquipmentType.Pant)
                    if (currentEquipment[(int)EEquipmentType.Accessorie] != null)
                    {
                        currentEquipment[(int)EEquipmentType.Accessorie].gameObject.SetActive(false);
                        currentEquipment[(int)EEquipmentType.Accessorie] = null;
                    }

                if (equip.equipmentType == EEquipmentType.Chest)
                    if (currentEquipment[(int)EEquipmentType.Arms] != null)
                    {
                        currentEquipment[(int)EEquipmentType.Arms].gameObject.SetActive(false);
                        currentEquipment[(int)EEquipmentType.Arms] = null;
                    }
            }

            currentEquipment[(int)equip.equipmentType] = equip;

            currentEquipment[(int)equip.equipmentType].gameObject.SetActive(true);
            currentEquipment[(int)equip.equipmentType].Equip();
        }
    }


    private void AddUtilItem(ItemData item)
    {
        int maxCount = 0;

        if (item.itemName == "Æ÷¼Ç")
        {
            myUtilItems.Add(item);
            maxCount = 3;
            //_playerUI.SetUtillSlot(item, 3);
            utilItemCount.Add(3);

            _playerUI.utillImage.gameObject.SetActive(true);
            currentUtilItem = item;
            _playerUI.SetUtillSlot(item, maxCount);
        }
        else if (item.itemName == "¿­¼è")
        {
            hasKey = true;
        }
    }

    private void UnEquip(BaseEquipment equip)
    {
        Destroy(equip.gameObject);
    }
}
