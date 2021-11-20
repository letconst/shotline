using UnityEngine;
using UnityEngine.UI;

public class WeaponPanelProperty : MonoBehaviour
{
    [SerializeField, Header("武器名のテキスト")]
    private Text weaponNameText;

    [SerializeField, Header("武器説明のテキスト")]
    private Text weaponInfoText;

    [SerializeField, Header("武器モデルを配置する親オブジェクト")]
    private Transform modelParentTrf;

    public Text      WeaponNameText => weaponNameText;
    public Transform ModelParentTrf => modelParentTrf;
    public Text      WeaponInfoText => weaponInfoText;
}
