using System.Collections.Generic;
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

    [SerializeField]
    private GameObject[] itemSpawnPoints;

    [SerializeField]
    private GameObject lineGaugeObject;

    [SerializeField]
    private GameObject lineGaugeObject2;

    private List<ItemPositionData> _itemSpawnPoints;

    public static GameObject InputBlocker    => Instance.inputBlocker;
    public static Text       StatusText      => Instance.statusText;
    public        GameObject LineGaugeObject => lineGaugeObject;
    public GameObject LineGaugeObject2 => lineGaugeObject2;

    public static List<ItemPositionData> ItemSpawnPoints
    {
        get => Instance._itemSpawnPoints;
        private set => Instance._itemSpawnPoints = value;
    }

    protected override void Awake()
    {
        base.Awake();

        ItemSpawnPoints ??= new List<ItemPositionData>();
        ItemSpawnPoints.Clear();

        foreach (GameObject point in itemSpawnPoints)
        {
            _itemSpawnPoints.Add(point.GetComponent<ItemPositionData>());
        }
    }
}
