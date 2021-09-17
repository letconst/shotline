using UnityEngine;
using UnityEngine.UI;

public class CsvController : MonoBehaviour ,IManagedMethod
{
    [SerializeField]
    public Transform CanvasTransform = null;

    [SerializeField]
    public GameObject WeaponPanel = null;

    public int Count;

    public void ManagedStart()
    {
        foreach (WeaponDatas datas in WeaponManager.weaponDatas)
        {
            GameObject ButtonObj = Instantiate(WeaponPanel, CanvasTransform);
            Button button = ButtonObj.GetComponent<Button>();

            button.transform.Find("WeaponNameText").GetComponent<Text>().text = 
                "武器名\n" + datas.WeaponName+
                "\n\n　　　　弾速:" +datas.BulletSpeed +
                "\nゲージ最大値:" + datas.GaugeMax +
                "\nゲージ回復量;" + datas.GaugeRecovery;
        }
        Count = WeaponManager.weaponDatas.Count;
    }

    public void ManagedUpdate()
    {
        
    }
}
