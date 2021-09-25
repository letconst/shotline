using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour ,IManagedMethod
{
    public static List<WeaponDatas> weaponDatas;
    public static List<Transform>   weaponModels;

    public static WeaponDatas SelectWeapon;

    public void ManagedStart()
    {
        weaponDatas  = new List<WeaponDatas>();
        weaponModels = new List<Transform>();
        SelectWeapon = null;
    }

    public void ManagedUpdate()
    {

    }
}
