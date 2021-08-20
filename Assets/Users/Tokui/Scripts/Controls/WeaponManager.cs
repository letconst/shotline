using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WeaponManager : MonoBehaviour
{
    public static List<WeaponDatas> weaponDatas;

    // Start is called before the first frame update
    void Start()
    {
        weaponDatas = new List<WeaponDatas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
