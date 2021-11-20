using UnityEngine;
using UnityEngine.UI;

public class CsvController : MonoBehaviour ,IManagedMethod
{
    [SerializeField]
    public Transform CanvasTransform = null;

    [SerializeField]
    public GameObject WeaponPanel = null;

    public void ManagedStart()
    {
        foreach (WeaponDatas datas in WeaponManager.weaponDatas)
        {
            GameObject panelObject = Instantiate(WeaponPanel, CanvasTransform);
            var        panelProp   = panelObject.GetComponent<WeaponPanelProperty>();

            panelProp.WeaponNameText.text =
                "武器名\n" + datas.WeaponName+
                "\n\n　　　　弾速: " +datas.BulletSpeed +
                "\nゲージ最大値: " + datas.GaugeMax +
                "\nゲージ回復量: " + datas.GaugeRecovery;

            panelProp.WeaponInfoText.text =
                "説明\n" + datas.Weaponinfo;

            // モデル生成
            var modelPrefab = Resources.Load<GameObject>(datas.PrefabsName);

            if (modelPrefab == null)
            {
                Debug.LogError($"武器「{datas.WeaponName}」のプレハブが見つかりません: {datas.PrefabsName}");

                continue;
            }

            GameObject modelObject = Instantiate(modelPrefab, panelProp.ModelParentTrf);

            WeaponManager.weaponModels.Add(modelObject.transform);
        }
    }

    public void ManagedUpdate()
    {

    }
}
