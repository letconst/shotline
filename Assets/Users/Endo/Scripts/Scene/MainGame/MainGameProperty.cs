using System.Collections.Generic;
using UnityEngine;

public class MainGameProperty : SingletonMonoBehaviour<MainGameProperty>
{
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

    [SerializeField, Header("CM BlendListオブジェクト")]
    private GameObject cmBlendListObject;

    private List<ItemPositionData> _itemSpawnPoints;

    public GameObject LineGaugeObject   => lineGaugeObject;
    public GameObject LineGaugeObject2  => lineGaugeObject2;
    public GameObject CmBlendListObject => cmBlendListObject;

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
