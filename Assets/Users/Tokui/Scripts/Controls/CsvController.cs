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
    GameObject WeaponPunel = null;

    public void ManagedStart()
    {
        foreach (WeaponDatas datas in WeaponManager.weaponDatas)
        {
            GameObject ButtonObj = Instantiate(WeaponPunel, CanvasTransform);
            Button button = ButtonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                WeaponNameText.text = "武器名\n" + datas.WeaponName;
                WeaponStatusText.text =
                    "　　　　弾速:" + datas.BulletSpeed +
                    "\nゲージ最大値:" + datas.GaugeMax +
                    "\nゲージ回復量;" + datas.GaugeRecovery;
            });
        }
    }

    public void ManagedUpdate()
    {
        
    }
}
