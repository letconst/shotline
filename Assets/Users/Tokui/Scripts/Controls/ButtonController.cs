using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField]
    Text WeaponNameText = null;

    [SerializeField]
    Text WeaponStatusText = null;

    WeaponDatas weaponDadas;

    // Start is called before the first frame update
    void Start()
    {
        weaponDadas = new WeaponDatas();
    }

    public void ClickWeapon1Button()
    {
        WeaponNameText.text = "WeaponName\n" + weaponDadas.WeaponName;
        WeaponStatusText.text = 
            "BulletSpeed:" + weaponDadas.BulletSpeed +
            "\nGaugeMax:" + weaponDadas.GaugeMax +
            "\nGaugeRecovery" + weaponDadas.GaugeRecovery;
    }

    public void ClickWeapon2Button()
    {
        WeaponNameText.text = "WeaponName\n" + weaponDadas.WeaponName;
        WeaponStatusText.text =
            "BulletSpeed:" + weaponDadas.BulletSpeed +
            "\nGaugeMax:" + weaponDadas.GaugeMax +
            "\nGaugeRecovery" + weaponDadas.GaugeRecovery;
    }

    public void ClickWeapon3Button()
    {
        WeaponNameText.text = "WeaponName\n" + weaponDadas.WeaponName;
        WeaponStatusText.text =
            "BulletSpeed:" + weaponDadas.BulletSpeed +
            "\nGaugeMax:" + weaponDadas.GaugeMax +
            "\nGaugeRecovery" + weaponDadas.GaugeRecovery;
    }
}
