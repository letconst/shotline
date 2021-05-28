using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    [SerializeField]
    private Button itemBtn;

    [SerializeField]
    private Button shotBtn;

    [SerializeField]
    private Transform holdPos;

    public Image   ItemIcon { get; private set; }
    public Image   ShotIcon { get; private set; }
    public Button  ItemBtn  => itemBtn;
    public Button  ShotBtn  => shotBtn;
    public Vector3 HoldPos  => holdPos.position;

    protected override void Awake()
    {
        base.Awake();

        ItemIcon = ItemBtn.GetComponentsInChildren<Image>()[1];
        ShotIcon = ShotBtn.GetComponentsInChildren<Image>()[1];

        ItemIcon.sprite = null;
        ItemIcon.color  = Color.clear;
    }
}
