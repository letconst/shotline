using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour ,IManagedMethod
{
    public static List<WeaponDatas> weaponDatas;

    public static WeaponDatas SelectWeapon;

    public void ManagedStart()
    {
        weaponDatas = new List<WeaponDatas>();
    }

    public void ManagedUpdate()
    {
        
    }
}
