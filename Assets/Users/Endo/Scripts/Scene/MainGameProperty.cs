using UnityEngine;
using UnityEngine.UI;

public class MainGameProperty : SingletonMonoBehaviour<MainGameProperty>
{
    [SerializeField]
    private GameObject inputBlocker;

    [SerializeField]
    private Text statusText;

    [SerializeField]
    public Transform startPos1P;

    [SerializeField]
    public Transform startPos2P;

    public static GameObject InputBlocker => Instance.inputBlocker;
    public static Text       StatusText   => Instance.statusText;
}
