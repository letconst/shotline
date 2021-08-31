using UnityEngine;
using UnityEngine.UI;

public class CsvController : MonoBehaviour ,IManagedMethod
{
    [SerializeField]
    Transform CanvasTransform = null;

    [SerializeField]
    Text WeaponNameText = null;

    [SerializeField]
    Text WeaponStatusText = null;

    [SerializeField]
    GameObject ButtonPrefab = null;

    public void ManagedStart()
    {
        foreach (WeaponDatas datas in WeaponManager.weaponDatas)
        {
            GameObject ButtonObj = Instantiate(ButtonPrefab, CanvasTransform);
            Button button = ButtonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                WeaponNameText.text = "WeaponName\n" + datas.WeaponName;
                WeaponStatusText.text =
                    "BulletSpeed:" + datas.BulletSpeed +
                    "\nGaugeMax:" + datas.GaugeMax +
                    "\nGaugeRecovery" + datas.GaugeRecovery;
            });
        }
    }

    public void ManagedUpdate()
    {
        
    }
}
