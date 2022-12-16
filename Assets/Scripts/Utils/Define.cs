using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum EWeaponType
    {
        None,
        OneHanded,
        TwoHanded,
    }

    public enum EEquipParts
    {
        None,
        Right,
        Left,
    }

    public enum EHoldParts
    {
        None,
        WaistR,
        WaistL,
        Back,
    }


    public enum EItemType
    {
        None,
        Weapon,
        Equip,
        Utils,
    }


    public enum EBossWallType
    {
        None,
        Enterance,
        Exit,
    }


    public enum EEnemyType
    {
        Common,
        Elite,
        Boss
    }


    public enum EEquipmentType
    {
        None,
        Accessorie,
        Helmet,
        Arms,
        Chest,
        Pant,
        Gloves,
        Boots,
        Size
    }
}
