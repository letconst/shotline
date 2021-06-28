using UnityEngine;
using UnityEngine.UI;

public class TitleProperty : SingletonMonoBehaviour<TitleProperty>
{
    [SerializeField]
    private GameObject statusBgImage;

    [SerializeField]
    private Text statusText;

    public static GameObject StatusBgImage => Instance.statusBgImage;
    public static Text       StatusText    => Instance.statusText;
}
