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
}
