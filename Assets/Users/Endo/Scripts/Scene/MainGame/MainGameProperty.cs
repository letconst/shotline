using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject rivalDirection1P;

    [SerializeField]
    private GameObject rivalDirection2P;

    [SerializeField, Header("CM BlendListオブジェクト")]
    private GameObject cmBlendListObject;

    [SerializeField]
    private Image roundTitleImg;

    [SerializeField]
    private Image p1PointImg;

    [SerializeField]
    private Image p2PointImg;

    [SerializeField]
    private CanvasGroup playerPointsCanvasGroup;

    [SerializeField]
    private Image battleResultImg;

    [SerializeField]
    private Image suddenDeathImg;

    [SerializeField]
    private Transform[] sotoWalls;

    [SerializeField, Header("弾丸消滅エフェクト")]
    private GameObject disappearanceBullet;

    public GameObject LineGaugeObject   => lineGaugeObject;
    public GameObject LineGaugeObject2  => lineGaugeObject2;
    public GameObject CmBlendListObject => cmBlendListObject;

    private List<ItemPositionData> _itemSpawnPoints;

    public static List<ItemPositionData> ItemSpawnPoints
    {
        get => Instance._itemSpawnPoints;
        private set => Instance._itemSpawnPoints = value;
    }

    public static GameObject  RivalDirection1P        => Instance.rivalDirection1P;
    public static GameObject  RivalDirection2P        => Instance.rivalDirection2P;
    public static Image       RoundTitleImg           => Instance.roundTitleImg;
    public static Image       P1PointImg              => Instance.p1PointImg;
    public static Image       P2PointImg              => Instance.p2PointImg;
    public static CanvasGroup PlayerPointsCanvasGroup => Instance.playerPointsCanvasGroup;
    public static Image       BattleResultImg         => Instance.battleResultImg;
    public static Image       SuddenDeathImg          => Instance.suddenDeathImg;
    public static Transform[] SotoWalls               => Instance.sotoWalls;
    public static GameObject  DisappearanceBullet     => Instance.disappearanceBullet;

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
